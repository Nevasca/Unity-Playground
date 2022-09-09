using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDoor : MonoBehaviour
{
    [SerializeField] private FlatWall _flatWall;
    [SerializeField] private bool _enterDoor = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (_enterDoor)
                _flatWall.EnterWall(other.gameObject);
            else
                _flatWall.ExitWall();
        }
    }
}