using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RewindLookAtNearby : RewindObject
{
    private LookAtNearby _look;
    private Transform _aimTransform;

    protected override void Awake()
    {
        base.Awake();
        _look = GetComponent<LookAtNearby>();
        _aimTransform = _look.GetAimTarget();
    }

    protected override PointInTime CreateTickData()
    {
        return new PointInTimeLookAtNearby(_aimTransform.position, _look.GetAimWeight());
    }

    protected override void StartRewind()
    {
        _look.Active = false;
    }

    protected override void Rewind()
    {
        if (!HasPointInTime())
            return;

        PointInTimeLookAtNearby point = (PointInTimeLookAtNearby)PopPointInTime();
        _aimTransform.position = point.aimPosition;
        _look.SetWeight(point.aimWeight);
    }

    protected override void StopRewind()
    {
        _look.Active = true;
    }
}
