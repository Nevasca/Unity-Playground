using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] private Transform lookTarget;

    public Transform GetLookTarget()
    {
        if (lookTarget != null)
            return lookTarget;

        return transform;
    }
}