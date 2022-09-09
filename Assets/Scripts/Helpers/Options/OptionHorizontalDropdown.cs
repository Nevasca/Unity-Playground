using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionHorizontalDropdown : OptionHandler
{
    [SerializeField] private string _KEY;
    [SerializeField] private int _defaultValue;
    [SerializeField] private UIHorizontalDropdown _dropdown;

    public override void Init()
    {
        _dropdown.Value = PlayerPrefs.GetInt(_KEY, _defaultValue);
    }

    public virtual void OnValueChanged(int value)
    {
        PlayerPrefs.SetInt(_KEY, value);
    }

    private void Reset()
    {
        _dropdown = GetComponent<UIHorizontalDropdown>();
    }
}