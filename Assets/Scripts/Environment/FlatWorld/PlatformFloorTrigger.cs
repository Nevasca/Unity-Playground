using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFloorTrigger : MonoBehaviour
{
    [SerializeField] private PhysicMaterial _defaultPlayerMaterial;
    [SerializeField] private PhysicMaterial _onPlataformMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            other.GetComponent<Collider>().material = _onPlataformMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            other.GetComponent<Collider>().material = _defaultPlayerMaterial;
    }
}