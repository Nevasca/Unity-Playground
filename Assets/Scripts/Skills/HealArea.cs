using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;
using System;
using Tweening;

public class HealArea : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private float _healPerSecond = 10f;
    [SerializeField] private float _radius = 3f;
    [SerializeField] private AICategory _category;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private bool _autoEnable = false;

    private bool _healing;
    private float _startedTime;
    private List<IDamageable> _damageablesInRange = new List<IDamageable>();

    private void Awake()
    {
        if (_autoEnable)
            Init();
    }

    public void Init()
    {
        transform.DoScale(_radius * 0.5f, 0.3f).SetEase(Ease.OutBack);
        _startedTime = Time.time;
        _healing = true;
    }

    public void Init(float healPerSecond, float radius, float duration, AICategory category)
    {
        _healPerSecond = healPerSecond;
        _radius = radius;
        _duration = duration;
        _category = category;

        Init();
    }

    private void Update()
    {
        if (!_healing)
            return;

        Heal();
        CheckDuration();
    }

    private void Heal()
    {
        if (_damageablesInRange.Count == 0)
            return;

        foreach (var damagable in _damageablesInRange)
        {
            if (damagable.Equals(null))
                continue;

            if (damagable.GetCategory() != _category)
                continue;

            damagable.Heal(_healPerSecond * Time.deltaTime);
        }
    }

    private void CheckDuration()
    {
        if (Time.time - _startedTime >= _duration)
            DestroyArea();
    }

    private void DestroyArea()
    {
        _healing = false;
        transform.DoScale(0f, 0.3f).OnComplete(() => Destroy(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_healing)
            return;

        if (other.TryGetComponent(out IDamageable damageable))
            _damageablesInRange.Add(damageable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_healing)
            return;

        if (other.TryGetComponent(out IDamageable damageable))
            _damageablesInRange.Remove(damageable);
    }
}