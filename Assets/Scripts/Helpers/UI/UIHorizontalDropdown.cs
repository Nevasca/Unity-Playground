using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Tweening;
using System;
using TMPro;

public class UIHorizontalDropdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private UIButtonBeautify _buttonNext;
    [SerializeField] private UIButtonBeautify _buttonPrevious;

    [Space(10)]
    [SerializeField] private List<Dropdown.OptionData> _options;

    [Space(10)]
    [SerializeField] private Dropdown.DropdownEvent _onValueChanged;

    public int Value { get { return _index; } set { SetValue(value); } }
    public List<Dropdown.OptionData> Options => _options;
    public Dropdown.DropdownEvent OnValueChanged => _onValueChanged;

    private int _index;

    private void Awake()
    {
        _buttonNext.GetComponent<Button>().onClick.AddListener(NextValue);
        _buttonPrevious.GetComponent<Button>().onClick.AddListener(PreviousValue);
    }

    public void AddOptions(List<string> options)
    {
        foreach (string option in options)
            this._options.Add(new Dropdown.OptionData(option));
    }

    private void SetValue(int value, bool notify = true)
    {
        _index = value;

        if(notify)
            _onValueChanged?.Invoke(_index);

        RefreshShownValue();
        RefreshNavigationButtons();
    }

    public void SetValueWithoutNotify(int value)
    {
        SetValue(value, false);
    }

    private void NextValue()
    {
        if (Value < _options.Count - 1)
            Value++;
    }

    private void PreviousValue()
    {
        if (Value > 0)
            Value--;
    }

    public void RefreshShownValue()
    {
        _label.text = _options[Value].text;
        if (_label.gameObject.activeInHierarchy)
        {
            TweenSequence seq = Tweener.Sequence();
            seq.Append(_label.transform.DoScale(1.2f, 0.15f));
            seq.Append(_label.transform.DoScale(1f, 0.15f));
            seq.Play();
        }
    }

    private void RefreshNavigationButtons()
    {
        _buttonNext.gameObject.SetActive(Value < _options.Count - 1);
        _buttonPrevious.gameObject.SetActive(Value > 0);
    }

    private void OnEnable()
    {
        RefreshNavigationButtons();
    }
}