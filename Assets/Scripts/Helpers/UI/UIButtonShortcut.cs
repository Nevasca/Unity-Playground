using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIButtonShortcut : MonoBehaviour
{
    [SerializeField] private GameObject[] _overlays;
    [SerializeField] private InputActionReference _shortcutAction;

    private bool _performed;

    private void Perform(InputAction.CallbackContext input)
    {
        if (_performed)
            return;

        foreach (var overlay in _overlays)
            if (overlay.activeInHierarchy)
                return;

        GetComponent<Button>().onClick.Invoke();
    }

    private void OnEnable()
    {
        _performed = false;
        _shortcutAction.action.started += Perform;
    }

    private void OnDisable()
    {
        _shortcutAction.action.started -= Perform;
    }
}