using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindRigidbody : RewindObject
{
    private Rigidbody _rb;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
    }

    protected override void StartRewind()
    {
        _rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    protected override void StopRewind()
    {
        GetComponent<Collider>().enabled = true;
    }

    protected override void Rewind()
    {
        if (HasPointInTime())
        {            
            PointInTimeRigidbody point = (PointInTimeRigidbody)PopPointInTime();
            transform.position = point.position;
            transform.rotation = point.rotation;
            _rb.velocity = point.velocity;
            _rb.isKinematic = point.isKinematic;
        }
    }

    protected override PointInTime CreateTickData()
    {
        return new PointInTimeRigidbody(transform.position, transform.rotation, _rb.velocity, _rb.isKinematic);
    }
}