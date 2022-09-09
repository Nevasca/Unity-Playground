using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    [SerializeField] private FlatWall _wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _wall.RestartFromLastCheckpoint();
    }
}