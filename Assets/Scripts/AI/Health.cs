using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private AICategory category;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private GameObject dieVFX;

    public event Action<float, float> OnHealthChanged;

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

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void Damage(float value)
    {
        CurrentHealth -= value;

        if(CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            if(TryGetComponent(out StateMachine stateMachine))
            {
                if(stateMachine.SwitchToNewState(AIState.Dead))
                    return;
            }
                
            Instantiate(dieVFX, transform.position, dieVFX.transform.rotation);
            Destroy(gameObject);
        }
    }

    public void Heal(float value)
    {
        CurrentHealth = Mathf.Min(_currentHealth + value, maxHealth);
    }

    public AICategory GetCategory()
    {
        return category;
    }

    public void SetCategory(AICategory category)
    {
        this.category = category;
    }
}