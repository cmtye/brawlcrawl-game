using Player_Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace AI_Scripts
{
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
        private CharacterMovement _characterMovement;
        private Animator _animator;
        private GameObject _outline;
        private GameObject _progress;
        private Vector3 _progressVector = Vector3.zero;

        private void Start()
        {
        
            foreach (Transform child in transform)
            {
                if (child.name == "Outline")
                {
                    _outline = child.gameObject;
                }
                if (child.name == "Progress")
                {
                    _progress = child.gameObject;
                }
            }
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
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
            Gizmos.DrawWireSphere(transform.position, 1.7f);
        }

        public ref Vector3 GetProgressVector() { return ref _progressVector; }
        public GameObject GetOutline() { return _outline; }
        public GameObject GetProgress() { return _progress; }
        public Animator GetAnimator() { return _animator; }
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

    }
}
