using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WindHandler : MonoBehaviour, IWindReceiver
{
    [SerializeField] private Rigidbody _rigidbody;

    public void AddWindForce(Vector3 wind)
    {
        _rigidbody.AddForce(wind);
    }

    private void OnEnable()
    {
        WindManager.OnWind += AddWindForce;
    }

    private void OnDisable()
    {
        WindManager.OnWind -= AddWindForce;
    }
    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
}