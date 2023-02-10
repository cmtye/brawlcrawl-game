using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Range(0, 1.0f)] [SerializeField] private float moveSmoothing = 0.5f;
    [SerializeField] private GameObject attackColliders;
    [SerializeField] private float hSpeed = 10f;
    [SerializeField] private float vSpeed = 6f;
    [SerializeField] private float counterDuration = 2f;
    [SerializeField] private float gravityMultiplier = 1f;
    private float _gravityVelocity;
    private Vector3 _currentVelocity;
    private float _gravity;
    public bool isCountering;
    public bool canMove;
    
    private SpriteRenderer _spriteRenderer;
    private CharacterController _characterController;

    private bool _facingRight = true;
    private Vector3 _currentInput;
    private Vector3 _smoothInput;

    // Start is called before the first frame update
    private void Awake()
    {
        _gravity = 9.81f;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _characterController = GetComponent<CharacterController>();
    }

    // Takes in a 2D vector representing up and down movement, and translates it to 3D velocity on
    // a GameObjects rigidbody.
    public void Move(Vector3 moveDirection)
    {
        if (!canMove) return;

        _currentInput = Vector3.SmoothDamp(_currentInput, 
            moveDirection, ref _smoothInput, moveSmoothing);

        if (_characterController.isGrounded && _gravityVelocity < 0.0f)
        {
            _gravityVelocity = -1.0f;
        }
        _gravityVelocity -= _gravity * gravityMultiplier * Time.deltaTime;
        
        var moveVector = new Vector3(_currentInput.x * hSpeed, _gravityVelocity, _currentInput.z * vSpeed);
        _currentVelocity = moveVector;
        _characterController.Move(_currentVelocity * Time.deltaTime);
        
        switch (moveVector.x)
        {
            case > 0 when !_facingRight:
            case < 0 when _facingRight:
                Flip();
                break;
        }
    }

    // Mirror the sprite and its colliders when moving left, revert when moving right.
    private void Flip()
    {
        _facingRight = !_facingRight;
        _spriteRenderer.flipX = !_spriteRenderer.flipX;

        // If attack colliders are set, rotate them along with the sprite.
        if (attackColliders) attackColliders.transform.Rotate(0, 180, 0);
    }
    
    // The character jumps back and enters a state where they can't move, but will retaliate if hit during duration.
    public IEnumerator Counter()
    {
        if (isCountering) yield return 0;

        isCountering = true;
        var elapsed = 0.0f;
        var holdColor = _spriteRenderer.color;
        _spriteRenderer.color = Color.red;
        while (elapsed < counterDuration)
        {
            canMove = false;
            elapsed += Time.deltaTime;
            yield return 0;
        }
        Debug.Log("Done countering");
        _spriteRenderer.color = holdColor;
        isCountering = false;
        canMove = true;

    }
}
