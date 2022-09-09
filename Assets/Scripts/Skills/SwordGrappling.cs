using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Tweening;

public class SwordGrappling : Skill
{
    [Header("Sword")]
    [SerializeField] private float swordDamage = 60f;
    [SerializeField] private Collider swordTrigger;

    [Header("Grappling")]
    [SerializeField] private float maxHookDistance = 50f;
    [SerializeField] private LayerMask hookMask;
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private float hookAttachSpeed = 10f;
    [SerializeField] private Transform hook;
    [SerializeField] private AnimationCurve hookCurve;

    [Header("Quick Time Event")]
    [SerializeField] private QuickTimeEvent quickTimeEventPerformed;
    [SerializeField] private EnemyTarget enemyTarget;

    [Header("Feedback")]
    [SerializeField] private GameObject vfxHookHit;

    public const string ANIM_HOOKED = "isHooked";
    public const string ANIM_HOLDING_SWORD = "isHoldingSword";
    public const string ANIM_SWORD_ATTACK = "swordAttack";
    public const string ANIM_AIR_ATTACK = "airAttack";

    //Refs
    private Rigidbody _rigidbody;
    private PlayerAim _aim;
    private Animator _animator;
    private PlayerMovement _movement;

    //Aux
    private Transform _camera;
    private Transform _skillOrigin;

    //Sword
    private bool _swordAttackStarted;

    //Hook
    private RaycastHit _hookHit;
    private Vector3 _hookDirection;
    private float _hookDistance;

    public override void Init(Transform skillOrigin)
    {
        _rigidbody = _author.GetComponent<Rigidbody>();
        _aim = _author.GetComponent<PlayerAim>();
        _animator = _author.GetComponent<Animator>();
        _movement = _author.GetComponent<PlayerMovement>();

        _camera = Camera.main.transform;
        _skillOrigin = skillOrigin;
        transform.parent = skillOrigin;
        transform.localPosition = Vector3.zero;
        ropeLine.enabled = false;
        hook.gameObject.SetActive(false);
        swordTrigger.enabled = false;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_enable)
            return;

        if (!ropeLine.enabled)
            return;

        ropeLine.SetPosition(0, _skillOrigin.position);
        ropeLine.SetPosition(1, hook.position);
    }

    public override void Use(bool started)
    {
        if(started && !_swordAttackStarted && _movement.IsGrounded())
        {
            _swordAttackStarted = true;
            _movement.Enable(false);
            _animator.SetTrigger(ANIM_SWORD_ATTACK);
        }
    }

    public override void UseSecondary(bool started)
    {
        if(started)
        {
            _movement.Enable(false);
            _aim.EnableAim(slowMotion: true);
        }
        else
        {
            Hook();
        }
    }

    #region Sword
    public override void EventSkillCast()
    {
        swordTrigger.enabled = true;
    }

    public override void EventSkillEnded()
    {
        _movement.Enable(true);
        swordTrigger.enabled = false;
        _swordAttackStarted = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _author)
            return;

        if (!other.TryGetComponent(out Health health))
            return;

        health.Damage(swordDamage);
        SoundManager.Instance.PlaySFXAt(20, transform.position);
    }
    #endregion

    #region Hook
    private void Hook()
    {
        if(Physics.Raycast(_camera.position, _camera.forward, out _hookHit, maxHookDistance, hookMask))
        {
            _hookDirection = _hookHit.point - _skillOrigin.position;
            _hookDistance = _hookDirection.magnitude;
            
            _hookDirection.Normalize();

            StartCoroutine(AttachHook());
        }

        _aim.DisableAim();
        _movement.Enable(true);
    }

    IEnumerator AttachHook()
    {
        //Reseta posicao
        ropeLine.SetPosition(0, _skillOrigin.position);
        ropeLine.SetPosition(1, _skillOrigin.position);
        hook.position = _skillOrigin.position;

        hook.parent = null;
        hook.forward = _hookDirection;
        hook.gameObject.SetActive(true);
        ropeLine.enabled = true;

        SoundManager.Instance.PlaySFXAt(41, _skillOrigin.position, pitchRange: 0.1f);

        float timerAttach = 0f;
        float timeToAttach = _hookDistance / hookAttachSpeed;
        while (timerAttach < timeToAttach)
        {
            timerAttach += Time.deltaTime;
            hook.position += hookAttachSpeed * Time.deltaTime * _hookDirection;
            yield return null;
        }

        hook.position = _hookHit.point;

        //Feedback
        //hook.DOShakePosition(0.15f, 0.2f, 15);
        Instantiate(vfxHookHit, _hookHit.point, Quaternion.identity);
        SoundManager.Instance.PlaySFXAt(39, _hookHit.point, pitchRange: 0.1f, minDistance: 1.5f);

        StartCoroutine(HookAuthor());
    }

    private IEnumerator HookAuthor()
    {
        _movement.Enable(false);
        _rigidbody.isKinematic = true;
        _animator.SetBool(ANIM_HOOKED, true);
        SoundManager.Instance.PlaySFXAt(42, _skillOrigin.position, pitchRange: 0.1f);

        float duration = Mathf.Lerp(1, 0.3f, 1f - _hookDistance / maxHookDistance);
        float timer = 0f;
        Vector3 direction = _hookHit.point - _author.position;
        direction.Normalize();

        Vector3 startPosition = _author.position;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            _author.position = Vector3.Lerp(startPosition, _hookHit.point, hookCurve.Evaluate(timer / duration));
            yield return null;
        }
        _author.position = _hookHit.point;

        ropeLine.enabled = false;
        hook.gameObject.SetActive(false);
        EnablePlayer();
    }

    private void EnablePlayer()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = false;
        _movement.Enable(true);
        _animator.SetBool(ANIM_HOOKED, false);
        
    }
    #endregion

    private void Slice()
    {
        StopAllCoroutines();
        ropeLine.enabled = false;
        hook.gameObject.SetActive(false);
        _animator.SetBool(ANIM_AIR_ATTACK, true);
        _animator.SetTrigger(ANIM_SWORD_ATTACK);

        TweenSequence sliceSeq = Tweener.Sequence();
        
        sliceSeq.Append(_author.DoMove(enemyTarget.AimTarget.position, 0.2f));
        sliceSeq.AppendCallback(() =>
        {
            enemyTarget.EnemyHealth.Damage(99999);
            SoundManager.Instance.PlaySFXAt(43, enemyTarget.AimTarget.position, minDistance: 2f);
            _animator.SetBool(ANIM_AIR_ATTACK, false);
            EnablePlayer();
        });

        sliceSeq.Play();
    }

    private void OnEnable()
    {
        _animator?.SetBool(ANIM_HOLDING_SWORD, true);
        quickTimeEventPerformed.AddListener(Slice);
    }

    private void OnDisable()
    {
        _animator?.SetBool(ANIM_HOLDING_SWORD, false);
        quickTimeEventPerformed.RemoveListener(Slice);
    }
}