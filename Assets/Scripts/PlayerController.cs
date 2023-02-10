using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 2019 Unity input system components. Allows easy gamepad configuration.
    private PlayerInputActions _playerControls;
    private InputAction _move;

    // General character movement script that we feed axis values into.
    private CharacterMovement _characterMovement;
    private CharacterController _characterController;
    private Vector2 _moveDirectionInput;
    private Vector3 _moveDirection;
    private bool _isMovementPressed;
    //private Vector2 _currentInput;
    //private Vector2 _smoothInput;
    //private bool _groundedPlayer;
    //private Vector3 _playerVelocity;

    private HealthBehavior _healthBehavior;

    // Generalized attack stats. Can easily be altered for designers.
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDamage = 2f;
    private float _actionDelay;

    // Transform points for attack colliders. The two points form a capsule collider.
    [SerializeField] private List<Transform> punchPoints;
    [SerializeField] private List<Transform> kickPoints;
    private Transform _punchBack;
    private Transform _punchFront;
    private Transform _kickBack;
    private Transform _kickFront;

    [SerializeField] private List<int> abilityThresholds;
    
    // Collision variables for attacks landing on an enemy/object. Pre-allocation saves garbage collecting time later.
    [SerializeField] private LayerMask attackableLayers;
    private readonly Collider[] _hitColliders = new Collider[20];

    [SerializeField] private List<GameObject> abilityIndicators;

    private void Awake()
    {
        _playerControls = new PlayerInputActions();
        _characterMovement = GetComponent<CharacterMovement>();
        _characterController = GetComponent<CharacterController>();
        _healthBehavior = GetComponent<HealthBehavior>();

        _playerControls.Player.Move.started += OnMovementInput;
        _playerControls.Player.Move.canceled += OnMovementInput;
        _playerControls.Player.Move.performed += OnMovementInput;
        
        _playerControls.Player.Punch.performed += _ => Punch(false);
        _playerControls.Player.Kick.performed += _ => Kick();
        _playerControls.Player.Counter.performed += _ => Counter();
        _playerControls.Player.Ability.performed += _ => Ability();
        
        _punchBack = punchPoints[0];
        _punchFront = punchPoints[1];
        _kickBack = kickPoints[0];
        _kickFront = kickPoints[1];
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
    private void FixedUpdate()
    {
        // TODO: Move and alter attack delay logic to update for more persistent accessing.
        
        _characterMovement.Move(_moveDirection);
        if (!_healthBehavior.counteredAttack) return;
        
        Punch(true);
        _healthBehavior.counteredAttack = false;
    }

    private void Punch(bool noDelay)
    {
        if (!(Time.time >= _actionDelay) || noDelay) return;

        if (_characterMovement.isCountering) return;
        abilityIndicators[0].GetComponent<MeshRenderer>().enabled = true;
        // Create capsule collider instead of sphere for better feeling Z-axis hit registration.
        var overlaps = Physics.OverlapCapsuleNonAlloc(_punchBack.position, 
            _punchFront.position, attackRange, _hitColliders, attackableLayers);
        
        // Increment combo if you hit an enemy.
        if (overlaps >= 1) GameManager.instance.IncrementCombo();
        // Iterate through array of enemies the attack overlapped with in method below.
        DamageCollided(_hitColliders, overlaps, attackDamage);
        Invoke(nameof(DeactivateRenderer), 0.2f);
        _actionDelay = Time.time + 1f / attackRate;
    }
    private void Kick()
    {
        if (!(Time.time >= _actionDelay)) return;

        if (_characterMovement.isCountering) return;
        abilityIndicators[1].GetComponent<MeshRenderer>().enabled = true;
        var overlaps = Physics.OverlapCapsuleNonAlloc(_kickBack.position,
            _kickFront.position, attackRange, _hitColliders, attackableLayers);
        if (overlaps >= 1) GameManager.instance.IncrementCombo();
        DamageCollided(_hitColliders, overlaps, attackDamage);
        Invoke(nameof(DeactivateRenderer), 0.2f);
        _actionDelay = Time.time + 1f / attackRate;
    }
    private void Counter()
    {
        if (!(Time.time >= _actionDelay)) return;
        if (!_characterController.isGrounded) return;
        
        // Movement script handles setting counter bool. If enemy hits you during this, health script initiates *TODO*
        if (_characterMovement.isCountering) return;
        StartCoroutine(_characterMovement.Counter());
        
        _actionDelay = Time.time + 1f / attackRate;
    }
    private void Ability()
    {
        if (!(Time.time >= _actionDelay)) return;

        if (_characterMovement.isCountering) return;
        var currentCombo = GameManager.instance.GetCombo();
        int overlaps;
        switch (currentCombo)
        {
            case var _ when currentCombo >= abilityThresholds[2]:
                // Gauge is at or past last threshold but not above
                abilityIndicators[4].GetComponent<MeshRenderer>().enabled = true;
                currentCombo -= abilityThresholds[2];
                GameManager.instance.SetCombo(currentCombo);
                Debug.Log("Level 3");
                
                // Hard coded spherical explosion, may change or iterate on.
                overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 8,
                    _hitColliders, attackableLayers);
                DamageCollided(_hitColliders, overlaps, attackDamage * 8);
                Invoke(nameof(DeactivateRenderer), 0.2f);
                break;
            
            case var _ when currentCombo >= abilityThresholds[1]:
                // Gauge is at or past second threshold but not above
                abilityIndicators[3].GetComponent<MeshRenderer>().enabled = true;
                currentCombo -= abilityThresholds[1];
                GameManager.instance.SetCombo(currentCombo);
                Debug.Log("Level 2");
                
                overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 5,
                    _hitColliders, attackableLayers);
                DamageCollided(_hitColliders, overlaps, attackDamage * 4);
                Invoke(nameof(DeactivateRenderer), 0.2f);
                break;
            
            case var _ when currentCombo >= abilityThresholds[0]:
                // Gauge is at or past first threshold but not above
                abilityIndicators[2].GetComponent<MeshRenderer>().enabled = true;
                currentCombo -= abilityThresholds[0];
                GameManager.instance.SetCombo(currentCombo);
                Debug.Log("Level 1");
                
                overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 3,
                    _hitColliders, attackableLayers);
                DamageCollided(_hitColliders, overlaps, attackDamage * 2);
                Invoke(nameof(DeactivateRenderer), 0.2f);
                break;
            
            default:
                Debug.Log("None");
                // Gauge isn't high enough
                break;
        }

        _actionDelay = Time.time + 1f / attackRate;
    }

    private void DamageCollided(Collider[] hit, int amountHit, float damage)
    {
        for (var i = 0; i < amountHit; i++)
        {
            hit[i].GetComponent<HealthBehavior>().TakeDamage(damage);
        }
    }
    // Allows the transforms of attack points to be shown in the editor for designers. Debug only.
    private void OnDrawGizmosSelected()
    {
        if (_punchBack == null || _kickBack == null || _punchFront == null || _kickFront == null)
        {
            return;
        }
        
        Gizmos.DrawWireSphere(_punchBack.position, attackRange);
        Gizmos.DrawWireSphere(_punchFront.position, attackRange);
        Gizmos.DrawWireSphere(_kickBack.position, attackRange);
        Gizmos.DrawWireSphere(_kickFront.position, attackRange);
        
        Gizmos.DrawWireSphere(transform.position, attackRange * 8);
    }
    
    private void DeactivateRenderer()
    {
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = false;
        }
    }

}
