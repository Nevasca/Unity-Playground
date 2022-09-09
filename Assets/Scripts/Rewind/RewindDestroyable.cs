using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindDestroyable : RewindObject
{
    private int _pointsWhileDestroyed;

    protected override void AddPointInTime(PointInTime point)
    {
        if (point != null && point.destroyed)
        {
            _pointsWhileDestroyed++;
            if (_pointsWhileDestroyed > RewindManager.Instance.MaxStackSize)
                Destroy(gameObject);
        }
        else
        {
            _pointsWhileDestroyed = 0;
        }

        base.AddPointInTime(point);
    }

    protected override PointInTime CreateTickData()
    {
        return new PointInTime(!gameObject.activeSelf);
    }

    protected override void Rewind() {

        if (HasPointInTime())
        {
            PointInTime point = PopPointInTime();
            gameObject.SetActive(!point.destroyed);
        }
    }

    protected override void StartRewind() { }

    protected override void StopRewind() { }
}