using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableHandler : MonoBehaviour, IShootable
{
    [SerializeField] private Rigidbody _rigidbody;

    public void OnShoot(Vector3 force)
    {
        _rigidbody.AddForce(force);
    }
}