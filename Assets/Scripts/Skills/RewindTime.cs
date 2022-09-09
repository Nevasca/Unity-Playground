using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindTime : Skill
{
    public override void Init(Transform skillOrigin) { }

    public override void Use(bool started)
    {
        if (started)
            RewindManager.Instance.StartRewind();
        else
            RewindManager.Instance.StopRewind();
    }

    public override void UseSecondary(bool started) { }
}