using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollision : MonoBehaviour
{
    [SerializeField] private int clipID;
    [SerializeField] private float pitch = 1f;
    [SerializeField] private float minDistance = 1f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);
        float volume = 0.2f;

        if (_rb != null)
            volume += _rb.velocity.sqrMagnitude * 0.8f;

        SoundManager.Instance.PlaySFXAt(clipID, contactPoint.point, volume, pitch, 0.1f, minDistance: minDistance);
    }
}