using System.Collections;
using AI_Scripts;
using UI_Scripts;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Character_Scripts
{
    public class HealthBehavior : MonoBehaviour
    {
        // The characters health and retaliation related variables.
        [SerializeField] private int maxHealth;
        [SerializeField] private int incomingDamageMultiplier = 1;
        public int currentHealth;
        public bool counteredAttack;
    
        // Get character movement scripts if available to inform about retaliation.
        private CharacterMovement _characterMovement;
        private CharacterController _characterController;
        private PlayerController _playerController;
        private ImpactReceiver _impactReceiver;
        private AIController _aiController;

        // Animations and particle systems added as the objects child and set in editor
        // to show an object getting hit and losing all its health points.
        [SerializeField] private ParticleSystem damageFX;
        [SerializeField] private ParticleSystem destroyFX;
        [SerializeField] private AudioSource deathSound;
        private static readonly int Hurt = Animator.StringToHash("Hurt");

        private void Awake()
        {
            _aiController = GetComponent<AIController>();
            _characterMovement = GetComponent<CharacterMovement>();
            _characterController = GetComponent<CharacterController>();
            _playerController = GetComponent<PlayerController>();
            _impactReceiver = GetComponent<ImpactReceiver>();
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage, Vector3 damageSource, bool isPunch)
        {
            // Set variable for retaliation if in counter state.
            if (_characterMovement)
            {
                if (_characterMovement.isCountering)
                {
                    counteredAttack = true;
                    return;
                }

                if (_aiController)
                {
                    if (!isPunch)
                    {
                        _aiController.GetNavMeshAgent().enabled = false;
                        _aiController.GetAnimator().SetTrigger(Hurt);
                        StartCoroutine(TurnOnAgent());
                        
                        var knockback = (transform.position - damageSource);
                        knockback = knockback.normalized * 5;
                        _impactReceiver.AddImpact(new Vector3(knockback.x, 5f, knockback.z), 20);
                    }
                }
            }

            if (isPunch)
            {
                currentHealth -= damage * incomingDamageMultiplier;
            }

            if (!_playerController && !_aiController)
            {
                currentHealth -= damage * incomingDamageMultiplier;
            }
            EmitDamageFX();
            if (deathSound)
            {
                deathSound.Play();   
            }

            // Alter game state if the hit object is the player.
            if (_playerController)
            {
                var animator = _playerController.GetAnimator();
                animator.SetTrigger(Hurt);
                var knockback = (transform.position - damageSource);
                knockback = knockback.normalized * 5;
                _impactReceiver.AddImpact(new Vector3(knockback.x, 5f, knockback.z), 20);
                
                GameManager.instance.ResetCombo();
                GameManager.instance.UpdateHealthUI(currentHealth);
                GameManager.instance.SetShakeCamera();
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            // If an enemy dies, destroy their in progress components and ability renders.
            if (CompareTag("Enemy"))
            {
                _aiController.enabled = false;
                _aiController.GetComponent<CharacterController>().enabled = false;
                _aiController.GetComponent<CapsuleCollider>().enabled = false;
                foreach (Transform t in transform)
                {
                    if (t.name is "Progress" or "Outline")
                    {
                        Destroy(t.gameObject);
                    }
                }

                foreach (var a in GetComponents<AudioSource>())
                {
                    a.enabled = false;
                }
            }
            
            // When the object dies, we need the particle system to
            // function before destroying them.
            if (_characterMovement)
            {
                _impactReceiver.enabled = false;
                _characterController.enabled = false;
                Invoke(nameof(EmitDestroyedFX), 0.2f);
                Invoke(nameof(DeactivateRenderer), 0.3f);
                Destroy(gameObject, 1.2f);
            }
            // Breakables handles slightly differently than characters.
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
            
            // Send player to game over screen when they die.
            if (_playerController)
            {
                _playerController.enabled = false;
                StartCoroutine(TurnOnEnd(true));
            }
        }

        public float GetHealth() { return currentHealth; }
        public float GetMaxHealth() { return maxHealth; }
        public void GainHealth(int healing)
        {
            currentHealth += healing;
            GameManager.instance.UpdateHealthUI(currentHealth);
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        
        // When a player dies, let destruction particles emit before pausing the game.
        private IEnumerator TurnOnEnd(bool value)
        {
            var elapsed = 0.0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                yield return 0;
            }
            GameManager.instance.ToggleEnd(value);
        }

        private IEnumerator TurnOnAgent()
        {
            yield return new WaitForSeconds(1f);
            _aiController.GetNavMeshAgent().enabled = true;
        }
        private void DeactivateRenderer()
        {
            foreach (var r in GetComponents<Renderer>())
            {
                r.enabled = false;
            }
        }
        private void EmitDamageFX() { if (damageFX) damageFX.Play(); }
        private void EmitDestroyedFX() { if (destroyFX) destroyFX.Play(); }
    }
}
