using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Scripts
{
    public class PlayerController : MonoBehaviour
    {
        // The players movement and input variables.
        private PlayerInputActions _playerControls;
        private CharacterMovement _characterMovement;
        private CharacterController _characterController;
        private Vector2 _moveDirectionInput;
        private Vector3 _moveDirection;
        private bool _isMovementPressed;
        private bool _isPausePressed;

        // The players combat values and attacking state variables.
        [SerializeField] private float attackRate = 2f;
        [SerializeField] private float attackRange = 0.5f;
        [SerializeField] private int attackDamage = 2;
        [SerializeField] private List<Transform> punchPoints;
        [SerializeField] private List<Transform> kickPoints;
        [SerializeField] private LayerMask attackableLayers;
        public List<int> abilityThresholds;
        private HealthBehavior _healthBehavior;
        private IEnumerator _counterCoroutine;
        private float _actionDelay;

        // Collision array to store attacks landing on an enemy/object.
        // Pre-allocation saves garbage collecting time later.
        private readonly Collider[] _hitColliders = new Collider[40];
    
        // The players animation variables.
        private Animator _playerAnimator;
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int Punch1 = Animator.StringToHash("Punch");
        private static readonly int Kick1 = Animator.StringToHash("Kick");
        
        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
            _characterController = GetComponent<CharacterController>();
            _healthBehavior = GetComponent<HealthBehavior>();
            _playerAnimator = GetComponent<Animator>();

            _playerControls = new PlayerInputActions();
            _playerControls.Player.Move.started += OnMovementInput;
            _playerControls.Player.Move.canceled += OnMovementInput;
            _playerControls.Player.Move.performed += OnMovementInput;
            
            _playerControls.Player.Punch.performed += _ => Punch(false);
            _playerControls.Player.Kick.performed += _ => Kick(false);
            _playerControls.Player.Counter.performed += _ => CounterAttack();
            _playerControls.Player.Ability.performed += _ => Ability();
            _playerControls.Player.Pause.performed += _ => OnPausePressed();
            _isPausePressed = false;
        }
        
        private void Update()
        {
            //if (_moveDirection.magnitude > 0)
            //{
                _characterMovement.Move(_moveDirection);
            //}

            // Set animator to run Vira's running animation if Vira is not
            // countering and she is in process of moving while not paused.
            if (Time.timeScale != 0)
            {
                _playerAnimator.SetBool(IsRunning, !_characterMovement.isCountering && _isMovementPressed);
            }

            // Must set stored coroutine to null to avoid bad references.
            if (_characterMovement.coroutineEnded)
            {
                _counterCoroutine = null;
            }

            // End counter coroutine if we've countered an attack.
            if (!_healthBehavior.counteredAttack) return;
            StopCoroutine(_counterCoroutine);
            _characterMovement.EndCounter();
        
            // Kick and Punch for counter retaliation.
            Punch(true);
            Kick(true);
        }

        private void Punch(bool noDelay)
        {
            // Check if the player can take action on call. noDelay boolean allows the counter to bypass action delay.
            if (!(Time.time >= _actionDelay) && !noDelay) return;
            if (_characterMovement.isCountering && !noDelay) return;

            // If time is paused, don't take action.
            if (Time.timeScale == 0) return;
        
            // Set trigger for player punch animation
            _playerAnimator.SetTrigger(Punch1);
        
            // Create capsule collider instead of sphere for better feeling Z-axis hit registration.
            var overlaps = Physics.OverlapCapsuleNonAlloc(punchPoints[0].position, 
                punchPoints[1].position, attackRange, _hitColliders, attackableLayers);
        
            // Increment combo if you hit an enemy.
            if (overlaps >= 1)
            {
                GameManager.instance.IncrementCombo();
            }

            // Iterate through array of enemies the attack overlapped with in method below.
            TryDamageCollided(overlaps, attackDamage);
        
            // This method is called after a successful counter, so set variable to false when punch is complete.
            _healthBehavior.counteredAttack = false;
            _actionDelay = Time.time + 1f / attackRate;
        }
    
        private void Kick(bool noDelay)
        {
            if (!(Time.time >= _actionDelay) && !noDelay) return;
            if (_characterMovement.isCountering && !noDelay) return;
            if (Time.timeScale == 0) return;
        
            // Set trigger for player kick animation
            _playerAnimator.SetTrigger(Kick1);
        
            var overlaps = Physics.OverlapCapsuleNonAlloc(kickPoints[0].position,
                kickPoints[1].position, attackRange, _hitColliders, attackableLayers);
            if (overlaps >= 1)
            {
                GameManager.instance.IncrementCombo();
            }

            TryDamageCollided(overlaps, attackDamage);
            _healthBehavior.counteredAttack = false;
            _actionDelay = Time.time + 1f / attackRate;
        }
    
        private void CounterAttack()
        {
            // Check if the player is in an actionable state.
            if (!_characterController.isGrounded || _characterMovement.isCountering 
                                                 || !(Time.time >= _actionDelay) || Time.timeScale == 0) return;

            // If coroutine isn't null, we are already countering and can't begin a new one.
            if (_counterCoroutine != null) return;

            // Instantiate new counter coroutine and store it.
            _counterCoroutine = _characterMovement.Counter();
            StartCoroutine(_counterCoroutine);
            _actionDelay = Time.time + 1f / attackRate;
        }
        
        private void Ability()
        {
            // Check if the player is in an actionable state.
            if (!(Time.time >= _actionDelay) || _characterMovement.isCountering || Time.timeScale == 0) 
                return;
        
            var currentCombo = GameManager.instance.GetCombo();
            int overlaps;
            switch (currentCombo)
            {
                // Gauge is at max, give player massive damaging explosion.
                case var _ when currentCombo >= abilityThresholds[2]:
                    currentCombo -= abilityThresholds[2];
                    GameManager.instance.SetCombo(currentCombo);
                    
                    overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 8,
                        _hitColliders, attackableLayers);
                    TryDamageCollided(overlaps, attackDamage * 8);
                    break;
            
                // Gauge is at tier 2.
                case var _ when currentCombo >= abilityThresholds[1]:
                    currentCombo -= abilityThresholds[1];
                    GameManager.instance.SetCombo(currentCombo);
                
                    overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 4,
                        _hitColliders, attackableLayers);
                    TryDamageCollided(overlaps, attackDamage * 2);
                    break;
            
                // Gauge is at tier 1.
                case var _ when currentCombo >= abilityThresholds[0]:
                    currentCombo -= abilityThresholds[0];
                    GameManager.instance.SetCombo(currentCombo);
                
                    overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 3,
                        _hitColliders, attackableLayers);
                    TryDamageCollided(overlaps, attackDamage);
                    break;
            
                // Gauge is at tier 0.
            }
            _actionDelay = Time.time + 1f / attackRate;
        }

        private void TryDamageCollided(int amountHit, int damage)
        {
            // Make a list to store already struck enemies during this attack.
            var alreadyHit = new List<int>();
            for (var i = 0; i < amountHit; i++)
            {
                if (alreadyHit.Contains(_hitColliders[i].gameObject.GetInstanceID()))
                    continue;
            
                // If the collider has an attached health behavior, deal damage.
                var healthBar = _hitColliders[i].GetComponent<HealthBehavior>();
                if (!healthBar) continue;
                healthBar.TakeDamage(damage);
                alreadyHit.Add(_hitColliders[i].gameObject.GetInstanceID());
            }
        }
        
        private void OnPausePressed()
        {
            if (_isPausePressed)
            {
                GameManager.instance.TogglePause(false);
                _isPausePressed = false;
            }
            else
            {
                GameManager.instance.TogglePause(true);
                _isPausePressed = true;
            }
        }
    
        private void OnMovementInput(InputAction.CallbackContext context)
        {
            _moveDirectionInput = context.ReadValue<Vector2>();
            _moveDirection.x = _moveDirectionInput.x;
            _moveDirection.z = _moveDirectionInput.y;
            _isMovementPressed = _moveDirectionInput.x != 0 || _moveDirectionInput.y != 0;
        }
    
        private void OnEnable()
        {
            _playerControls.Player.Enable();
        }
        
        private void OnDisable()
        {
            _playerControls.Player.Disable();
        }
        
        // Allows the transforms of attack points to be shown in the editor for designers. Debug only.
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(punchPoints[0].position, attackRange);
            Gizmos.DrawWireSphere(punchPoints[1].position, attackRange);
            Gizmos.DrawWireSphere(kickPoints[0].position, attackRange);
            Gizmos.DrawWireSphere(kickPoints[1].position, attackRange);

            var currentPosition = transform.position;
            Gizmos.DrawWireSphere(currentPosition, attackRange * 3);
            Gizmos.DrawWireSphere(currentPosition, attackRange * 4);
            Gizmos.DrawWireSphere(currentPosition, attackRange * 8);
        }
        public Animator GetAnimator() { return _playerAnimator; }
    }
}
