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
    private CharacterMovement _movementScript;
    private Vector2 _moveDirection = Vector2.zero;

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
    
    private void Awake()
    {
        _playerControls = new PlayerInputActions();
        _movementScript = GetComponent<CharacterMovement>();
        _healthBehavior = GetComponent<HealthBehavior>();

        _punchBack = punchPoints[0];
        _punchFront = punchPoints[1];

        _kickBack = kickPoints[0];
        _kickFront = kickPoints[1];
    }
    private void OnEnable()
    {
        _move = _playerControls.Player.Move;
        _playerControls.Player.Punch.performed += _ => Punch();
        _playerControls.Player.Kick.performed += _ => Kick();
        _playerControls.Player.Counter.performed += _ => Counter();
        _playerControls.Player.Ability.performed += _ => Ability();
        
        _playerControls.Enable();
    }
    private void OnDisable()
    {
        _playerControls.Disable();
    }
    private void Update()
    {
        _moveDirection = _move.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        _movementScript.Move(_moveDirection);
    }
    
    private void Punch()
    {
        if (!(Time.time >= _actionDelay)) return;

        // Create capsule collider instead of sphere for better feeling Z-axis hit registration.
        var overlaps = Physics.OverlapCapsuleNonAlloc(_punchBack.position, 
            _punchFront.position, attackRange, _hitColliders, attackableLayers);
        
        // Increment combo if you hit an enemy.
        if (overlaps >= 1) GameManager.instance.IncrementCombo();
        // Iterate through array of enemies the attack overlapped with in method below.
        DamageCollided(_hitColliders, overlaps, attackDamage);
        
        _actionDelay = Time.time + 1f / attackRate;
    }
    private void Kick()
    {
        if (!(Time.time >= _actionDelay)) return;

        var overlaps = Physics.OverlapCapsuleNonAlloc(_kickBack.position,
            _kickFront.position, attackRange, _hitColliders, attackableLayers);
        if (overlaps >= 1) GameManager.instance.IncrementCombo();
        DamageCollided(_hitColliders, overlaps, attackDamage);
        
        _actionDelay = Time.time + 1f / attackRate;
    }
    private void Counter()
    {
        if (!(Time.time >= _actionDelay)) return;
        
        Debug.Log("Countering");
        
        // TODO: alter the timing for flow
        _actionDelay = Time.time + 1f / attackRate;
    }
    private void Ability()
    {
        if (!(Time.time >= _actionDelay)) return;

        var currentCombo = GameManager.instance.GetCombo();
        int overlaps;
        switch (currentCombo)
        {
            case var _ when currentCombo >= abilityThresholds[2]:
                // Gauge is at or past last threshold but not above
                currentCombo -= abilityThresholds[2];
                GameManager.instance.SetCombo(currentCombo);
                Debug.Log("Level 3");
                
                // Hard coded spherical explosion, may change or iterate on.
                overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 8,
                    _hitColliders, attackableLayers);
                DamageCollided(_hitColliders, overlaps, attackDamage * 8);
                break;
            
            case var _ when currentCombo >= abilityThresholds[1]:
                // Gauge is at or past second threshold but not above
                currentCombo -= abilityThresholds[1];
                GameManager.instance.SetCombo(currentCombo);
                Debug.Log("Level 2");
                
                overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 4,
                    _hitColliders, attackableLayers);
                DamageCollided(_hitColliders, overlaps, attackDamage * 4);
                break;
            
            case var _ when currentCombo >= abilityThresholds[0]:
                // Gauge is at or past first threshold but not above
                currentCombo -= abilityThresholds[0];
                GameManager.instance.SetCombo(currentCombo);
                Debug.Log("Level 1");
                
                overlaps = Physics.OverlapSphereNonAlloc(transform.position, attackRange * 2,
                    _hitColliders, attackableLayers);
                DamageCollided(_hitColliders, overlaps, attackDamage * 2);
                break;
            
            default:
                Debug.Log("None");
                // Gauge isn't high enough
                break;
        }

        // TODO: alter the timing for flow
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
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            _healthBehavior.TakeDamage(1f);
        }
    }
}
