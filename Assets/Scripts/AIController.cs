using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float visionRadius = 3f;
    [SerializeField] private float wanderRadius = 1f;
    [SerializeField] private int navMeshMask = NavMesh.AllAreas;
    [SerializeField] private float attackDelay = 3f;
    public AttackState attackState;
    public WanderState wanderState;
    public ChaseState chaseState;
    
    [SerializeField] private AIState currentState;
    private float _countdown;
    private NavMeshAgent _navMeshAgent;
    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;

        currentState = wanderState;
    }

    private void Update()
    {
        currentState.Execute(this);
    }

    public float GetCountdown() { return _countdown; }
    public void SetCountdown(float value) { _countdown = value; }
    public NavMeshAgent GetNavMeshAgent() { return _navMeshAgent; }
    public float GetAttackRange() { return attackRange; }
    public float GetWanderRadius() { return wanderRadius; }
    public int GetNavMeshMask() { return navMeshMask; }
    public void SetCurrentState(AIState value) { currentState = value; }
    public float GetAttackDelay() { return attackDelay; }
    public float GetVisionRadius() { return visionRadius; }
}