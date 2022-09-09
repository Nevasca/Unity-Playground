using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTimeLookAtNearby : PointInTime
{
    public Vector3 aimPosition;
    public float aimWeight;

    public PointInTimeLookAtNearby(Vector3 aimPosition, float rigWeight, bool destroyed = false) : base(destroyed)
    {
        this.aimPosition = aimPosition;
        this.aimWeight = rigWeight;
    }
}