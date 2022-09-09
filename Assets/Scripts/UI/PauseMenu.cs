using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Tweening;

public class PauseMenu : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private OptionHandler[] _options;

    [Header("Visual")]
    [SerializeField] private UIBeautify _menuContainer;
    [SerializeField] private Volume _menuPostProcessing;
    [SerializeField] private Toggle _firstTabToggle;

    public static event Action<bool> OnPaused;

    public bool Paused 
    { 
        get => _paused;
        private set
        {
            if (_paused == value)
                return;

            _paused = value;
            OnPaused?.Invoke(_paused);
        }
    }

    private bool _paused;
    private PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Player.Pause.started += OnPausePressed;
    }

    private void Start()
    {
        foreach (var option in _options)
            option.Init();
    }

    #region Input
    private void OnPausePressed(InputAction.CallbackContext input)
    {
        if (Paused)
            Close();
        else
            Open();
    }
    #endregion

    public void Open()
    {
        Paused = true;
        CursorLock.ToggleCursor(true);
        _menuContainer.Show();
        _firstTabToggle.isOn = true;
        _menuPostProcessing.DoWeight(1f, 0.15f);
    }

    public void Close()
    {
        Paused = false;
        CursorLock.ToggleCursor(false);
        _menuContainer.Hide();
        _menuPostProcessing.DoWeight(0f, 0.15f);
    }

    private void OnEnable()
    {
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    private void OnDestroy()
    {
        _controls.Player.Pause.started += OnPausePressed;
    }

    #region UNITY_EDITOR
    [ContextMenu("Find Options")]
    private void FindOptionsInChildren()
    {
        _options = GetComponentsInChildren<OptionHandler>(true);
    }
    #endregion
}