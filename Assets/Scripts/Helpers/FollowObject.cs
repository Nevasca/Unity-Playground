using System.Collections;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private Vector3 _positionOffset;
    private void Awake()
    {
        _positionOffset = _target.position - transform.position;
    }

    private void Update()
    {
        transform.position = _target.position + _positionOffset;
        transform.rotation = _target.rotation;
    }
}