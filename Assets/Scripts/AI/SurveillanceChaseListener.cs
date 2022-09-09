using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;

public class SurveillanceChaseListener : MonoBehaviour
{
    [SerializeField] private ChaseTrigger _chaseTrigger;

    private void OnEnable()
    {
        SurveillanceCamera.OnTargetSpotted += OnTargetSpotted;
    }

    private void OnTargetSpotted(Transform target, IDamageable damageable)
    {
        _chaseTrigger.SetToChase(target, damageable);
    }

    private void OnDisable()
    {
        SurveillanceCamera.OnTargetSpotted -= OnTargetSpotted;
    }
}