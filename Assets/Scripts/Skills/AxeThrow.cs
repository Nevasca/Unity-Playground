using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeThrow : Skill
{
    [SerializeField] private SkillInstance axe;
    [SerializeField] private float cooldown = 2f;

    private Animator _authorAnimator;
    private Transform _skillOrigin;

    private float _cooldownTimer;
    private bool _attackStarted = false;

    private SkillInstance _axeInstance;

    protected override void Awake()
    {
        base.Awake();

        _authorAnimator = _author.GetComponent<Animator>();
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
        _axeInstance = Instantiate(axe, _skillOrigin.position, Quaternion.identity, _skillOrigin);
        _authorAnimator.SetTrigger("throw");        
    }

    //Animation Event
    public override void EventSkillCast()
    {
        _axeInstance.transform.SetParent(null);
        Vector3 foward = Camera.main.transform.forward;
        _axeInstance.Init(foward, _author);
        //_axeInstance.Init(transform.forward, _author);

        _cooldownTimer = 0f;
        _attackStarted = false;
    }
}