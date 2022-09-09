using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatCheckpoint : MonoBehaviour
{
    [SerializeField] private FlatWall _wall;
    [SerializeField] private Transform _checkPointPosition;
    [SerializeField] private Transform _originCorner;
    [SerializeField] private Transform _targetCorner;

    public Transform StartPoint => _checkPointPosition;
    public Vector3 OrginCornerPosition => _originCorner.position;
    public Vector3 TargetCornerPosition => _targetCorner.position;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _wall.SetCheckpoint(this);
    }
}
