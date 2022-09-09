using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyOnDestroy : MonoBehaviour
{
    public event Action<GameObject> Destroyed;

    private void OnDestroy()
    {
        Destroyed?.Invoke(gameObject);
    }
}