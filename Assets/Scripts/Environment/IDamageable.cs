using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Nixtor.AI;

public interface IDamageable
{
    event Action<float, float> OnHealthChanged;

    void Damage(float value);
    void Heal(float value);
    AICategory GetCategory();
}