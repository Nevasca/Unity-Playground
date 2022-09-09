using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShellCasing : MonoBehaviour {

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _randomForce = 2f;
    [SerializeField] private float _destroyTime = 2f;

    private bool _collided = false;

    private void OnEnable()
    {
        _collided = false;
        _rigidbody.velocity = Random.insideUnitSphere * _randomForce;
        Destroy(gameObject, _destroyTime);
        //print("Oi");
        //Debug.Break();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_collided)
            return;

        _collided = true;
        SoundManager.Instance.PlaySFXAt(47, transform.position, 0.5f, pitchRange: 0.15f, minDistance: 0.5f);
    }
}
