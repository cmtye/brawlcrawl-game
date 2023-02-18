using UnityEngine;

namespace Character_Scripts
{
    public class ImpactReceiver : MonoBehaviour
    {
        [SerializeField] private float mass = 3f; // defines the character mass
        private Vector3 _impact = Vector3.zero;
        private CharacterController _characterController;
 
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
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
            if (_impact.magnitude > 0.2) _characterController.Move(_impact * Time.deltaTime);
            // consumes the impact energy each cycle:
            _impact = Vector3.Lerp(_impact, Vector3.zero, 5*Time.deltaTime);
        }
    }
}
