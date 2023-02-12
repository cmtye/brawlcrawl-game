using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthBehavior : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    // Multiplies incoming damage by this value. 0 = no damage, 1 = full damage, 2 = double damage.
    [SerializeField] private float armorMultiplier = 1f;
    public float currentHealth;
    public bool counteredAttack;
    
    private CharacterMovement _characterMovement;
    private CharacterController _characterController;

    // Particle systems added as the objects child and set in editor.
    [SerializeField] private ParticleSystem damageFX;
    [SerializeField] private ParticleSystem destroyFX;

    private void Awake()
    {
        currentHealth = maxHealth;
        _characterMovement = GetComponent<CharacterMovement>();
        _characterController = GetComponent<CharacterController>();
    }
    public void TakeDamage(float damage)
    {
        if (_characterMovement)
            if (_characterMovement.isCountering)
            {
                // TODO: Clean up this mechanic. Multiple hits give counter chain currently. Not happy with it.
                counteredAttack = true;
                return;
            }

        // Only resets combo if the hit object has a player controller.
        if (GetComponent<PlayerController>()) GameManager.instance.ResetCombo();
        
        currentHealth -= damage * armorMultiplier;
        EmitDamageFX();
        
        if (currentHealth <= 0) 
            Die();
    }

    public float GetHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }
    public void GainHealth(float healing)
    {
        currentHealth += healing;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    private void Die()
    {
        // Give sprite actors the appearance of falling over when dying.
        if (_characterMovement)
        {
            _characterController.enabled = false;
            Invoke(nameof(EmitDestroyedFX), 0.2f);
            Invoke(nameof(DeactivateRenderer), 0.3f);
            Destroy(gameObject, 1.2f);
        }
        // Breakables handles slightly differently than actors.
        else
        {
            EmitDestroyedFX();
            DeactivateRenderer();
            foreach (var c in GetComponents<Collider>())
            {
                c.enabled = false;
            }
            
            Destroy(gameObject, 1f);
        }
    }
    
    
    private void EmitDamageFX() { if (damageFX) damageFX.Play(); }
    private void EmitDestroyedFX() { if (destroyFX) destroyFX.Play(); }
    
    private void DeactivateRenderer()
    {
        foreach (var r in GetComponents<Renderer>())
        {
            r.enabled = false;
        }
    }
}
