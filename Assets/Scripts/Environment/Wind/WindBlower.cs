using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBlower : MonoBehaviour
{
    [SerializeField] private float _windForce = 1f;

    private Vector3 _wind;

    private List<IWindReceiver> _windReceivers = new List<IWindReceiver>();
    private List<IWindReceiver> _destroyedReceivers = new List<IWindReceiver>();

    private void FixedUpdate()
    {
        if (_windReceivers.Count == 0)
            return;

        _wind = transform.forward * _windForce;
        ApplyWind();
    }

    private void ApplyWind()
    {
        foreach (var receiver in _windReceivers)
        {
            //A destroyed object with an interface returns false if using receiver == null
            if (receiver.Equals(null))
            {
                _destroyedReceivers.Add(receiver);
                continue;
            }

            receiver.AddWindForce(_wind);
        }

        RemoveDestroyed();
    }

    private void RemoveDestroyed()
    {
        if (_destroyedReceivers.Count == 0)
            return;

        foreach (var destroyed in _destroyedReceivers)
            _windReceivers.Remove(destroyed);

        _destroyedReceivers.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IWindReceiver windReceiver))
            _windReceivers.Add(windReceiver);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IWindReceiver windReceiver))
            _windReceivers.Remove(windReceiver);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _windForce);
    }
}