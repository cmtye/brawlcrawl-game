using Character_Scripts;
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
        private ImpactReceiver _impactReceiver;
        private NavMeshAgent _navMeshAgent;
        private CharacterMovement _characterMovement;
        private CharacterController _characterController;
        private Animator _animator;
        private GameObject _outline;
        private GameObject _progress;
        private Vector3 _progressVector = Vector3.zero;

        private void Start()
        {
        
            foreach (Transform child in transform)
            {
                switch (child.name)
                {
                    case "Outline":
                        _outline = child.gameObject;
                        break;
                    case "Progress":
                        _progress = child.gameObject;
                        break;
                }
            }

            _characterController = GetComponent<CharacterController>();
            _impactReceiver = GetComponent<ImpactReceiver>();
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
                _animator.SetBool("attackCooldown", true);
                if (GetAttackStateRemaining() < attackStateCooldown)
                {
                    SetAttackStateRemaining(GetAttackStateRemaining() + Time.deltaTime);
                    return;
                }
            }

            _attackStateCooldown = false;
            _animator.SetBool("attackCooldown", false);
            SetAttackStateRemaining(0);
            currentState.Execute(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, 2.2f);
        }
        
        public ImpactReceiver GetImpactReceiver() { return _impactReceiver; }
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
