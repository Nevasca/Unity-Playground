using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTimeParent : PointInTime
{
    public Transform parent;

    public PointInTimeParent(Transform parent, bool destroyed = false) : base(destroyed)
    {
        this.parent = parent;
    }
}