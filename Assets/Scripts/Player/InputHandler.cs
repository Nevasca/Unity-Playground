using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInput _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    private void OnPaused(bool paused)
    {
        _input.enabled = !paused;
    }

    private void OnEnable()
    {
        PauseMenu.OnPaused += OnPaused;
    }

    private void OnDisable()
    {
        PauseMenu.OnPaused -= OnPaused;
    }
}