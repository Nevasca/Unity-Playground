using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCameraFoward : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.forward * 30f, Color.red);

        var direction = player.position - transform.position;
        Debug.DrawLine(transform.position, direction * 30f, Color.cyan);

        direction = transform.position - player.position;
        Debug.DrawLine(player.position, direction * 30f, Color.blue);
    }
}
