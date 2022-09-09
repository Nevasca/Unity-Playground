using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTracer : MonoBehaviour {

    public float speed = 100f;
    public ParticleSystem particle;    

    public void DoEffect(float dist)
    {
        var ma = particle.main;
        ma.startSpeed = speed;
        ma.startLifetime = dist / speed;
    }
}
