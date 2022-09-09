using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<CinemachineImpulseSource>().GenerateImpulse(Camera.main.transform.forward);
    }
}