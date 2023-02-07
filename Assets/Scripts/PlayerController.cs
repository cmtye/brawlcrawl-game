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

    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDamage = 2f;
    private float _actionDelay;

    [SerializeField] private Transform punchBack;
    [SerializeField] private Transform punchFront;
    [SerializeField] private Transform kickBack;
    [SerializeField] private Transform kickFront;
    
    [SerializeField] private LayerMask enemyLayers;
    private readonly Collider[] _hitColliders = new Collider[20];
    
    private void Awake()
    {
        _playerControls = new PlayerInputActions();
        _movementScript = GetComponent<CharacterMovement>();
        _healthBehavior = GetComponent<HealthBehavior>();
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

        // Create capsule collider instead of sphere for better feeling hit registration.
        var overlaps = Physics.OverlapCapsuleNonAlloc(punchBack.position, 
            punchFront.position, attackRange, _hitColliders, enemyLayers);
        // Iterate through pre-allocated array of overlapping enemies. Saves garbage collection time.
        for (var i = 0; i < overlaps; i++)
        {
            Debug.Log("Punched " + _hitColliders[i]);
            _hitColliders[i].GetComponent<HealthBehavior>().TakeDamage(attackDamage);
        }
        _actionDelay = Time.time + 1f / attackRate;
    }
    private void Kick()
    {
        if (!(Time.time >= _actionDelay)) return;
        
        var overlaps = Physics.OverlapCapsuleNonAlloc(kickBack.position, 
            kickFront.position, attackRange, _hitColliders, enemyLayers);
        for (var i = 0; i < overlaps; i++)
        {
            Debug.Log("Kicked " + _hitColliders[i]);
            _hitColliders[i].GetComponent<HealthBehavior>().TakeDamage(attackDamage);
        }
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
        
        Debug.Log("Ability");
        
        // TODO: alter the timing for flow
        _actionDelay = Time.time + 1f / attackRate;
    }
    
    // Allows the transforms of attack points to be shown in the editor for designers.
    private void OnDrawGizmosSelected()
    {
        if (punchBack == null || kickBack == null || punchFront == null || kickFront == null)
        {
            return;
        }
        
        Gizmos.DrawWireSphere(punchBack.position, attackRange);
        Gizmos.DrawWireSphere(punchFront.position, attackRange);
        Gizmos.DrawWireSphere(kickBack.position, attackRange);
        Gizmos.DrawWireSphere(kickFront.position, attackRange);
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            _healthBehavior.TakeDamage(1f);
        }
    }
}
