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
    [SerializeField] private bool canMove;
    
    private SpriteRenderer _spriteRenderer;
    private Rigidbody _rigidbody;

    private bool _facingRight = true;
    private Vector3 _velocity = Vector3.zero;

    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Takes in a 2D vector representing up and down movement, and translates it to 3D velocity on
    // a GameObjects rigidbody.
    public void Move(Vector2 moveDirection)
    {
        if (!canMove) return;
        
        var targetDirection = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
        var targetVelocity = new Vector3(targetDirection.x * hSpeed, targetDirection.y, targetDirection.z * vSpeed);
        _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _velocity, moveSmoothing);

        switch (moveDirection.x)
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
        if (attackColliders)
        {
            attackColliders.transform.Rotate(0, 180, 0);
        }
    }
}
