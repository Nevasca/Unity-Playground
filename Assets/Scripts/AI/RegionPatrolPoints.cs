using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionPatrolPoints : MonoBehaviour
{
    [SerializeField] private List<Transform> points;

    public Transform GetRandomPoint()
    {
        int index = Random.Range(0, points.Count);
        Transform aux = points[index];
        points.RemoveAt(index);
        return aux;
    }

    public void AddPoint(Transform point)
    {
        points.Add(point);
    }
}