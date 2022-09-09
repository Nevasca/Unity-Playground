using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindPlayerAttack : RewindObject
{
    private Animator _playerAnimator;

    protected override void Awake()
    {
        base.Awake();
        _playerAnimator = GetComponent<Animator>();
    }

    protected override PointInTime CreateTickData()
    {
        return new PointInTimePlayerAttack(_playerAnimator.GetBool(EnergyAttack.ANIM_HOLDING_MAGIC));
    }

    protected override void StartRewind() { }

    protected override void Rewind()
    {
        if (!HasPointInTime())
            return;

        PointInTimePlayerAttack point = (PointInTimePlayerAttack)PopPointInTime();
        _playerAnimator.SetBool(EnergyAttack.ANIM_HOLDING_MAGIC, point.isHoldingMagic);
    }

    protected override void StopRewind() { }
}
