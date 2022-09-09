using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RewindController : MonoBehaviour
{
    public void OnRewind(InputAction.CallbackContext inputAction)
    {
        if (inputAction.started)
            RewindManager.Instance.StartRewind();
        else if (inputAction.canceled)
            RewindManager.Instance.StopRewind();
    }
}