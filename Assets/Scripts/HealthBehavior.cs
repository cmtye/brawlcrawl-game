using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthBehavior : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float armorMultiplier = 1f;
    private float _currentHealth;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _rigidbody = GetComponent<Rigidbody>();
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
        // Give sprite actors the appearance of falling over when dying
        if (_rigidbody)
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
            _rigidbody.velocity = new Vector3(0, 0, 1);
            Destroy(gameObject, 1);
        }
        else
        {
            // TODO: Add particle explosion choice for both enemies and destructible objects.
            Destroy(gameObject);
        }
    }
}
