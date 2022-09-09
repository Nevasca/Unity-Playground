using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Nixtor.AI;
using System;

public class PlayerHeath : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float _startHealth = 50f;

    public event Action<float, float> OnHealthChanged;

    private PlayerMovement _movement;
    private Animator _animator;
    private CinemachineImpulseSource _cinemachineImpulse;

    private float _currentHealth;
    public float CurrentHealth
    { 
        get => _currentHealth; 
        set 
        { 
            _currentHealth = value;
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }
    }

    public const string ANIM_DEAD = "isDead";

    public bool IsDead { get; private set; }

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
        _cinemachineImpulse = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        CurrentHealth = _startHealth;
    }

    public void Damage(float value)
    {
        if (IsDead)
            return;

        CurrentHealth -= value;
        _cinemachineImpulse.GenerateImpulse(Camera.main.transform.forward);
        //transform.DOShakePosition(0.1f);

        if (CurrentHealth <= 0f)
            Die();
    }

    public void Heal(float value)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + value, maxHealth);
    }

    public AICategory GetCategory() => AICategory.Allie;

    public void Die()
    {
        CurrentHealth = 0f;
        TogglePlayer(false);
        GameManager.Instance.OnPlayerDead();
        RewindManager.Instance.AddEvent(Revive);
    }

    private void TogglePlayer(bool enable)
    {
        IsDead = !enable;
        _animator.SetBool(ANIM_DEAD, IsDead);

        if(IsDead)
            _movement.Enable(false);
            _animator.SetBool(PlayerMovement.ANIM_GROUNDED, true);
    }

    private void Revive()
    {
        TogglePlayer(true);
    }
}