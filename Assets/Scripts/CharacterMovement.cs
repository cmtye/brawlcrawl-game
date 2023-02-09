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
    public bool isCountering;
    public bool canMove;
    
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
        if (!_rigidbody) return;
        
        moveDirection = moveDirection.normalized;
        var currentVelocity = _rigidbody.velocity;
        var targetVelocity = new Vector3(moveDirection.x * hSpeed, currentVelocity.y, moveDirection.y * vSpeed);
        
        _rigidbody.velocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref _velocity, moveSmoothing);
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
        if (attackColliders) attackColliders.transform.Rotate(0, 180, 0);
    }
    
    // The character jumps back and enters a state where they can't move, but will retaliate if hit during duration.
    public IEnumerator Counter()
    {
        if (isCountering) yield return 0;
        
        isCountering = true;
        // Little hop back for eye candy.
        _rigidbody.velocity = _facingRight ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);

        var elapsed = 0.0f;
        while (elapsed < counterDuration)
        {
            Debug.Log("Countering");
            canMove = false;
            elapsed += Time.deltaTime;
            yield return 0;
        }
        Debug.Log("Done countering");
        isCountering = false;
        canMove = true;

    }
}
