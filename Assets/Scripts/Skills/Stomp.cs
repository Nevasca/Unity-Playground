using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Stomp : Skill
{
    [Header("Stomp")]
    [SerializeField] private float damage = 100f;
    [SerializeField] private float stompRange = 4f;
    [SerializeField] private float cooldown = 4f;
    [SerializeField] private GameObject stompImpactVFX;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private Animator _animator;
    private Transform _camera;
    private Transform _skillOrigin;
    private bool _attackStarted;
    private float _cooldownTimer;

    public override void Init(Transform skillOrigin)
    {
        _animator = _author.GetComponent<Animator>();
        _skillOrigin = skillOrigin;
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        if (!_enable)
            return;

        if (!_attackStarted)
            _cooldownTimer += Time.deltaTime;
    }

    public override void Use(bool started)
    {
        if (started)
            StartStomp();
    }

    public override void UseSecondary(bool started) { }

    private void StartStomp()
    {
        if (_attackStarted || _cooldownTimer < cooldown)
            return;

        _attackStarted = true;
        _animator.SetTrigger("stomp");
    }

    public override void EventSkillCast()
    {
        Instantiate(stompImpactVFX, _skillOrigin.position, Quaternion.identity);
        impulseSource.GenerateImpulse(_camera.forward);
        SoundManager.Instance.PlaySFXAt(38, _skillOrigin.position, 1f, 0.5f, 0.05f, minDistance: 2f);

        //Adds damage to close enemies
        Collider[] colliders = Physics.OverlapSphere(_skillOrigin.position, stompRange);
        foreach (Collider c in colliders)
        {
            if (c.TryGetComponent(out IDamageable damageable))
                damageable.Damage(damage);
        }

        _cooldownTimer = 0f;
        _attackStarted = false;
    }
}
