using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointInTimeEvent : PointInTime
{
    public Action[] callbacks;

    public PointInTimeEvent(Action[] callbacks, bool destroyed = false) : base (destroyed)
    {
        this.callbacks = callbacks;
    }
}