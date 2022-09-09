using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public Portal[] Portals { get; set; }

    private void Awake()
    {
        Portals = GetComponentsInChildren<Portal>();
        if (Portals.Length != 2)
            Debug.LogError("PortalPair does not contain 2 portals");
    }
}