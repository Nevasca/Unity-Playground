using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTimePlayerAttack : PointInTime
{
    public bool isHoldingMagic;

    public PointInTimePlayerAttack(bool isHoldingMagic, bool destroyed = false) : base (destroyed)
    {
        this.isHoldingMagic = isHoldingMagic;
    }
}