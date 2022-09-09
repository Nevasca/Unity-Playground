using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorToggler : MonoBehaviour
{
    [SerializeField] private Color _colorOn = Color.white;
    [SerializeField] private Color _colorOff = Color.gray;
    [SerializeField] private Graphic _target;

    public void ToggleColor(bool isOn)
    {
        _target.color = isOn ? _colorOn : _colorOff;
    }

    private void Reset()
    {
        _target = GetComponent<Graphic>();
    }
}