using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class OptionSlider : OptionHandler
{
    [SerializeField] private string _KEY;
    [SerializeField] private float _defaultValue;
    [SerializeField] private Slider _slider;
    [SerializeField] private UnityEvent<float> _onInit;

    [Header("Visual")]
    [SerializeField] private TextMeshProUGUI _textValue;
    [SerializeField] private GameObject _iconOn;
    [SerializeField] private GameObject _iconOff;

    [Header("Audio")]
    [SerializeField] private AudioClip _onChangedClip;

    public override void Init()
    {
        _slider.value = PlayerPrefs.GetFloat(_KEY, _defaultValue);
        _onInit?.Invoke(_slider.value);
    }

    public void OnValueChanged(float value)
    {
        PlayerPrefs.SetFloat(_KEY, value);

        if(gameObject.activeInHierarchy)
            SoundManager.Instance.PlaySFX(_onChangedClip, 0.3f);

        if(_textValue != null)
            _textValue.text = value.ToString();

        if (_iconOn == null)
            return;

        _iconOn.SetActive(value > 0f);
        _iconOff.SetActive(value == 0f);
    }

    private void Reset()
    {
        _slider = GetComponent<Slider>();
    }
}