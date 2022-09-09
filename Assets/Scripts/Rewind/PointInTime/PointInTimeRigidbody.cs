using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTimeRigidbody : PointInTime
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public bool isKinematic;

    public PointInTimeRigidbody(Vector3 position, Quaternion rotation, 
        Vector3 velocity, bool isKinematic, bool destroyed = false) : base(destroyed)
    {
        this.position = position;
        this.rotation = rotation;
        this.velocity = velocity;
        this.isKinematic = isKinematic;
    }
}