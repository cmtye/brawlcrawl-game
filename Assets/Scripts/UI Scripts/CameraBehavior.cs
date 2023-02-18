using UnityEngine;

namespace UI_Scripts
{
    public class CameraBehavior : MonoBehaviour
    {
        [Range(0, 1.0f)] [SerializeField] private float moveSmoothing = 0.5f;
        [SerializeField] private Vector3 offset;
        private Vector3 _playerOffset;
        [SerializeField] private Vector3 rayCastOffset;
        public Transform[] obstructions;
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _targetPosition;
        private int _oldHitsNumber;
        private Camera _parentCamera;
        private bool _isShaking;
        private Animator _animator;
    
        public Transform target;

        void Start()
        {
            _animator = GetComponentInParent<Animator>();
            _parentCamera = GetComponent<Camera>();
            _playerOffset = offset;
            _oldHitsNumber = 0;
        }

        private void Update()
        {
            _targetPosition = target.position + offset;
        }

        private void LateUpdate()
        {
            ViewObstructed();
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, moveSmoothing);
        }

        public void ShakeCamera()
        {
            _animator.SetTrigger("Shake");
        }
        public void ChangeTarget(Transform value, Vector3 newOffset, bool isPlayer)
        {
            target = value;
            offset = isPlayer ? _playerOffset : newOffset;
            _parentCamera.orthographicSize = isPlayer ? 4 : 5;
        }
        private void ViewObstructed()
        {
            float characterDistance = Vector3.Distance(transform.position + rayCastOffset, target.transform.position);
            int layerNumber = LayerMask.NameToLayer("Obstruction");
            int layerMask = 1 << layerNumber;
            RaycastHit[] hits = Physics.RaycastAll(transform.position + rayCastOffset, target.position - (transform.position + rayCastOffset), characterDistance, layerMask);
            Debug.DrawRay(transform.position + rayCastOffset, target.position - (transform.position + rayCastOffset), Color.red);
            if (hits.Length > 0)
            {   // Means that some stuff is blocking the view
                int newHits = hits.Length - _oldHitsNumber;

            
                if (obstructions != null && obstructions.Length > 0 && newHits <= 0)
                {
                    // Repaint all the previous obstructions. Because some of the stuff might be not blocking anymore
                    for (int i = 0; i < obstructions.Length; i++)
                    {
                        obstructions[i].gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                }
                obstructions = new Transform[hits.Length];
                // Hide the current obstructions 
                for (int i = 0; i < hits.Length; i++)
                {
                    Transform obstruction = hits[i].transform;
                    obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    obstructions[i] = obstruction;
                }
                _oldHitsNumber = hits.Length;
            }
            else
            {   // Mean that no more stuff is blocking the view and sometimes all the stuff is not blocking as the same time
                if (obstructions != null && obstructions.Length > 0)
                {
                    for (int i = 0; i < obstructions.Length; i++)
                    {
                        obstructions[i].gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                    _oldHitsNumber = 0;
                    obstructions = null;
                }
            }
        }
    }
}
