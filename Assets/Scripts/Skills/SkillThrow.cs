using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillThrow : Skill
{
    [Header("General")]
    [SerializeField] private SkillInstance _throwable;
    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private Vector3 _direction = new Vector3(0f, 1f, 1f);
    [SerializeField] private float _force = 5f;
    [SerializeField] private bool _throwOnRelease = false;

    [Header("Trajectory")]
    [SerializeField] private bool _showTrajectory = true;
    [SerializeField] private TrajectoryPrediction _trajectoryPrediction;
    [SerializeField] private Color _trajectoryColor = new Color(0.76f, 0.1f, 0.1f);

    private const string ANIM_THROW = "throw";
    private const string ANIM_HOLDING_THROW = "isHoldingThrow";

    private Animator _authorAnimator;
    private Transform _skillOrigin;

    private float _lastThrowTime;
    private bool _attackStarted = false;
    private bool _attackReleased = false;

    private SkillInstance _throwableInstance;
    private Transform _camera;

    public override void Init(Transform skillOrigin)
    {
        _direction = _direction.normalized;
        _authorAnimator = _author.GetComponent<Animator>();
        _skillOrigin = skillOrigin;
        _camera = Camera.main.transform;

        if(_showTrajectory)
            _trajectoryPrediction.SetTrajectoryColor(_trajectoryColor);
    }

    public override void Use(bool started)
    {
        if (started)
            StartAttack();
        else if(_throwOnRelease)
            ReleaseAndThrow();
    }

    private void FixedUpdate()
    {
        UpdateTrajectory();
    }

    private void UpdateTrajectory()
    {
        if (!_showTrajectory)
            return;

        if (!_attackStarted)
            return;

        if (_attackReleased)
            return;

        _trajectoryPrediction.ShowTrajectory(_throwableInstance.transform.position, GetVelocity());
    }

    public override void UseSecondary(bool started) { }

    private void StartAttack()
    {
        if (_attackStarted)
            return;

        if (Time.time - _lastThrowTime < _cooldown)
            return;

        _attackStarted = true;
        _throwableInstance = Instantiate(_throwable, _skillOrigin.position, Quaternion.identity, _skillOrigin);

        if (_throwOnRelease)
            _authorAnimator.SetBool(ANIM_HOLDING_THROW, true);
        else
            ReleaseAndThrow();
    }

    private void ReleaseAndThrow()
    {
        if (!_attackStarted)
            return;

        if (_attackReleased)
            return;

        _attackReleased = true;
        _authorAnimator.SetTrigger(ANIM_THROW);
        _trajectoryPrediction.HideTrajectory();
        ThrowObject();
    }

    //Animation Event
    public override void EventSkillCast()
    {
        _authorAnimator.ResetTrigger(ANIM_THROW);
        _authorAnimator.SetBool(ANIM_HOLDING_THROW, false);
    }

    private void ThrowObject()
    {
        _throwableInstance.transform.SetParent(null);
        _throwableInstance.Init(GetVelocity(), _author);

        _lastThrowTime = Time.time;
        _attackStarted = false;
        _attackReleased = false;
    }

    private Vector3 GetVelocity()
    {
        return _camera.TransformDirection(_direction) * _force;
    }
}