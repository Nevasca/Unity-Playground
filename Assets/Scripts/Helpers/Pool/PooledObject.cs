using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool Pool { get; set; }

    private void OnDisable()
    {
        if (Pool == null)
            return;

        transform.position = Vector3.zero;
        Pool.AddToAvailableObjects(gameObject);
    }
}