using System.Collections;
using UnityEngine;

namespace Character_Scripts
{
    public class CharacterMovement : MonoBehaviour
    {
        // The characters movement related variables.
        [Range(0, 1.0f)] [SerializeField] private float moveSmoothing = 0.5f;
        [SerializeField] private GameObject attackColliders;
        [SerializeField] private float hSpeed = 10f;
        [SerializeField] private float vSpeed = 6f;
        private CharacterController _characterController;
        private Vector3 _currentVelocity;
        private Vector3 _currentInput;
        private Vector3 _smoothInput;

        // The characters variables related to calculated gravity.
        [SerializeField] private float gravityMultiplier = 1f;
        private float _gravityVelocity;
        private float _gravity;
        
        // The characters counter/movement impairing variables.
        [SerializeField] private float counterDuration = 2f;
        [HideInInspector] public bool coroutineEnded;
        public bool isCountering;
        public bool canMove;
    
        // The characters sprite related variables.
        private SpriteRenderer _spriteRenderer;
        public bool facingRight = true;
        private Color _baseColor;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _characterController = GetComponent<CharacterController>();

            _baseColor = _spriteRenderer.color;
            _gravity = 9.81f;
        }

        // Takes in a 2D vector representing up and down movement, and
        // translates it to 3D velocity on a GameObjects rigidbody.
        public void Move(Vector3 moveDirection)
        {
            if (!canMove) return;

            // Smoothly interpolate to our desired direction.
            _currentInput = Vector3.SmoothDamp(_currentInput, 
                moveDirection, ref _smoothInput, moveSmoothing);

            // Give the player small downward velocity to keep them grounded.
            if (_characterController.isGrounded && _gravityVelocity < 0.0f)
            {
                _gravityVelocity = -1.0f;
            }

            // Downward velocity continues to grow as fall to simulate terminal velocity.
            _gravityVelocity -= _gravity * gravityMultiplier * Time.deltaTime;
        
            // Translate our 2D movement vector into a 3D vector with gravity.
            var moveVector = new Vector3(_currentInput.x * hSpeed, _gravityVelocity, _currentInput.z * vSpeed);
            _currentVelocity = moveVector;
            
            // Use built in character controller to move the character in our desired direction.
            _characterController.Move(_currentVelocity * Time.deltaTime);
        
            // We must swap our sprite and colliders depending on which direction we are facing.
            switch (moveVector.x)
            {
                case > 0 when !facingRight:
                case < 0 when facingRight:
                    Flip();
                    break;
            }
        }

        // Mirror the sprite and its colliders when moving left, revert when moving right.
        public void Flip()
        {
            facingRight = !facingRight;
            _spriteRenderer.flipX = !_spriteRenderer.flipX;

            // If attack colliders are set, rotate them along with the sprite.
            if (attackColliders)
            {
                attackColliders.transform.Rotate(0, 180, 0);
            }
        }

        // Revert the counters effects.
        public void EndCounter()
        {
            coroutineEnded = true;
            _spriteRenderer.color = _baseColor;
            isCountering = false;
            canMove = true;
        }
        
        // Freeze the character in place for the counters duration.
        public IEnumerator Counter()
        {
            coroutineEnded = false;
            if (isCountering)
            {
                yield return 0;
            }

            isCountering = true;
            var elapsed = 0.0f;
            _spriteRenderer.color = Color.cyan;
            while (elapsed < counterDuration)
            {
                canMove = false;
                elapsed += Time.deltaTime;
                yield return 0;
            }
            EndCounter();
        }
    }
}
