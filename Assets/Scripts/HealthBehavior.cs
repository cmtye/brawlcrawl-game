using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBehavior : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float armorMultiplier;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage * armorMultiplier;
        Debug.Log(name + " has " + _currentHealth + " health.");
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    public void GainHealth(float healing)
    {
        _currentHealth += healing;
        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }
    }
    private void Die()
    {
        Debug.Log(name + " has died.");
        Destroy(gameObject);
    }
}
