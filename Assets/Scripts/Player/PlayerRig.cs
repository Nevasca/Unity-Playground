using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRig : MonoBehaviour
{
    [SerializeField] private Transform _spineMiddle;
    [SerializeField] private RigArm _rightArmRig;
    [SerializeField] private RigArm _leftArmRig;

    public Transform SpineMiddle => _spineMiddle;
    public RigArm RightArmRig => _rightArmRig;
    public RigArm LeftArmRig => _leftArmRig;
}