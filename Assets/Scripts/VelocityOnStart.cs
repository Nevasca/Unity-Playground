using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityOnStart : MonoBehaviour
{
    public Vector3 initialVelocity;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        GetComponent<Rigidbody>().velocity = initialVelocity;
    }
}
