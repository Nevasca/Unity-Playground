using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] private Slider _sliderHealth;
    [SerializeField] private TextMeshProUGUI _textHealth;
    [SerializeField] private GameObject _target;
    [SerializeField] private bool _hide = true;
    [SerializeField] private float _hideDelay = 3f;

    private IDamageable _damageable;
    private bool _init;
    private float _lastChange;

    private void Awake()
    {
        if (_target == null)
            return;

        if (_target.TryGetComponent(out IDamageable damageable))
            Init(damageable);
    }

    public void Init(IDamageable damageable)
    {
        _damageable = damageable;
        _damageable.OnHealthChanged += OnHealthChanged;
        _init = true;
    }

    private void Update()
    {
        if (!_init)
            return;

        if (!gameObject.activeInHierarchy)
            return;

        if (Time.time - _lastChange >= _hideDelay)
            Hide();
    }

    private void OnHealthChanged(float current, float max)
    {
        _sliderHealth.value = current / max;
        _textHealth.text = $"{current:0}/{max}";

        if (!_hide)
            return;

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        _lastChange = Time.time;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _damageable.OnHealthChanged -= OnHealthChanged;
    }
}