using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTimePlayerMovement : PointInTime
{
    public bool isRunning;
    public bool isGrounded;
    public bool isLoosingBalance;

    public PointInTimePlayerMovement(bool isRunning,
        bool isGrounded, bool isLoosingBalance, bool destroyed = false) : base (destroyed)
    {
        this.isRunning = isRunning;
        this.isGrounded = isGrounded;
        this.isLoosingBalance = isLoosingBalance;
    }
}