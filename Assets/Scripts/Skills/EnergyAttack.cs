using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnergyAttack : Skill
{
    [SerializeField] private SkillInstance energySpike;
    [SerializeField] private float cooldown = 2f;

    public const string ANIM_HOLDING_MAGIC = "isHoldingMagic";
    public const string ANIM_MAGIC_ATTACK = "magicAttack";

    private PlayerMovement _playerMovement;
    private PlayerAim _playerAim;
    private Animator _playerAnimator;
    private SkillInstance _energyInstance;
    private float _cooldownTimer;
    private bool _attackStarted = false;
    private bool _attackEnded = false;
    private Transform _skillOrigin;

    protected override void Awake()
    {
        base.Awake();

        _playerMovement = _author.GetComponent<PlayerMovement>();
        _playerAim = _author.GetComponent<PlayerAim>();
        _playerAnimator = _author.GetComponent<Animator>();
        _cooldownTimer = cooldown;
    }

    public override void Init(Transform skillOrigin)
    {
        _skillOrigin = skillOrigin;
    }

    public override void Use(bool started)
    {
        if (started)
            StartAttack();
        else
            EndAttack();
    }

    public override void UseSecondary(bool started) { }

    private void Update()
    {
        if (!_enable)
            return;

        if (!_attackStarted)
            _cooldownTimer += Time.deltaTime;
    }    

    private void StartAttack()
    {
        if (_attackStarted || _cooldownTimer < cooldown)
            return;

        _attackStarted = true;
        _attackEnded = false;
        _playerMovement.SetCustomTurn(true, transform.position - Camera.main.transform.position);
        _playerAim.EnableAim(80f);
        _playerMovement.SmoothVelocityScale(0f, 0.5f);

        _playerAnimator.SetBool(ANIM_HOLDING_MAGIC, true);
        SoundManager.Instance.PlaySFXAt(7, _skillOrigin.position, 0.7f);
        _energyInstance = Instantiate(energySpike, _skillOrigin.position, Quaternion.identity, _skillOrigin);
    }

    //Animation Event
    public override void EventSkillCast()
    {
        _playerMovement.SmoothVelocityScale(1f, 0.5f);

        _energyInstance.transform.SetParent(null);
        Vector3 foward = Camera.main.transform.forward;
        _energyInstance.Init(foward, _author);
        _playerAnimator.SetBool(ANIM_HOLDING_MAGIC, false);
        _playerMovement.SetCustomTurn(false, Vector3.zero);
        _playerAim.DisableAim();

        _cooldownTimer = 0f;
        _attackStarted = false;
    }

    private void EndAttack()
    {
        if (_attackEnded)
            return;

        _playerAnimator.SetTrigger(ANIM_MAGIC_ATTACK);
        _attackEnded = true;        
    }
}