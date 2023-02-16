using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class AIController : MonoBehaviour
{
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackStateCooldown = 2f;
    [SerializeField] private float visionRadius = 3f;
    [SerializeField] private float wanderRadius = 1f;
    [SerializeField] private int navMeshMask = NavMesh.AllAreas;
    [SerializeField] private float attackDelay = 3f;
    public AttackState attackState;
    public WanderState wanderState;
    public ChaseState chaseState;

    [SerializeField] private AIState currentState;
    [SerializeField] private float timeBetweenWaypoints = 2f;
    private float _attackStateRemaining;
    private bool _attackStateCooldown;
    private float _remainingTime;
    private float _countdown;
    private NavMeshAgent _navMeshAgent;
    private SpriteRenderer _spriteRenderer;
    private CharacterMovement _characterMovement;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _characterMovement = GetComponent<CharacterMovement>();
        _navMeshAgent.updateRotation = false;
        _attackStateCooldown = false;

        currentState = wanderState;
    }

    private void Update()
    {
        if (_attackStateCooldown)
        {
            if (GetAttackStateRemaining() < attackStateCooldown)
            {
                SetAttackStateRemaining(GetAttackStateRemaining() + Time.deltaTime);
                return;
            }
        }

        _attackStateCooldown = false;
        SetAttackStateRemaining(0);
        currentState.Execute(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 2);
    }

    private float GetAttackStateRemaining() { return _attackStateRemaining; }

    private void SetAttackStateRemaining(float value) { _attackStateRemaining = value; }
    public void SetAttackStateCooldown(bool value) { _attackStateCooldown = value; }
    public float GetTimeBetweenWaypoints() { return timeBetweenWaypoints; }
    public float GetRemainingTime() { return _remainingTime; }
    public void SetRemainingTime(float value) { _remainingTime = value; }
    public CharacterMovement GetCharacterMovement() { return _characterMovement; }

    public float GetCountdown() { return _countdown; }
    public void SetCountdown(float value) { _countdown = value; }
    public NavMeshAgent GetNavMeshAgent() { return _navMeshAgent; }
    public float GetAttackRange() { return attackRange; }
    public float GetWanderRadius() { return wanderRadius; }
    public int GetNavMeshMask() { return navMeshMask; }
    public void SetCurrentState(AIState value) { currentState = value; }
    public float GetAttackDelay() { return attackDelay; }
    public float GetVisionRadius() { return visionRadius; }

    public void DisplayRange(float value)
    {
        StartCoroutine(nameof(DisplayRangeEnum), value);
    }
    private IEnumerator DisplayRangeEnum(float value)
    {
        var holdColor = _spriteRenderer.color;
        _spriteRenderer.color = Color.red;
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            if (r.CompareTag("EditorOnly")) r.enabled = true;
        }
        var elapsed = 0.0f;
        while (elapsed < value)
        {
            elapsed += Time.deltaTime;
            yield return 0;
        }
       
        _spriteRenderer.color = holdColor;
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            if (r.CompareTag("EditorOnly")) r.enabled = false;
        }

    }
    
}
