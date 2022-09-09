using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTarget", menuName = "Events/Enemy Target")]
public class EnemyTarget : ScriptableObject
{
    public Health EnemyHealth { get; private set; }
    public Transform AimTarget { get; private set; }

    public void SetEnemy(Health health, Transform target)
    {
        EnemyHealth = health;
        AimTarget = target;
    }
}