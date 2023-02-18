using AI_Scripts;
using UnityEngine;

namespace Character_Scripts
{
    public class ImpactReceiver : MonoBehaviour
    {
        [SerializeField] private float mass = 3f; // defines the character mass
        private Vector3 _impact = Vector3.zero;
        private CharacterController _characterController;
        private AIController _aiController;
        private float _gravityVelocity;
        private float _gravity;
 
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _aiController = GetComponent<AIController>();

            _gravity = 2.81f;
        }
        
        public void AddImpact(Vector3 direction, float force){
            direction.Normalize();
            if (direction.y < 0) 
            {
                direction.y = -direction.y; // reflect down force on the ground
            }
            _impact += direction.normalized * force / mass;
        }
 
        private void Update()
        {
            // apply the impact force:
            if (_aiController)
            {
                if (_impact.magnitude > 1f)
                {
                    if (_characterController.isGrounded && _gravityVelocity < 0.0f)
                    {
                        _gravityVelocity = 0f;
                    }

                    // Downward velocity continues to grow as fall to simulate terminal velocity.
                    _gravityVelocity -= _gravity * Time.deltaTime;
        
                    // Translate our 2D movement vector into a 3D vector with gravity.
                    _impact = new Vector3(_impact.x, _impact.y + _gravityVelocity, _impact.z);
                    Debug.Log(_impact.magnitude);
                    _characterController.Move(_impact * Time.deltaTime);
                }
            }
            else
            {
                if (_impact.magnitude > 0.2) _characterController.Move(_impact * Time.deltaTime);
            }
            // consumes the impact energy each cycle:
            _impact = Vector3.Lerp(_impact, Vector3.zero, 5*Time.deltaTime);
        }
    }
}
