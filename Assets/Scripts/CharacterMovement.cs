using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float hSpeed = 10f;
    [SerializeField] private float vSpeed = 6f;
    [SerializeField] private bool canMove;
    private bool _facingRight = true;
    private Vector3 _velocity = Vector3.zero;
    private Rigidbody _rigidbody;
    [Range(0, 1.0f)] [SerializeField] private float moveSmoothing = 0.5f;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 moveDirection)
    {
        if (!canMove) return;
        
        var targetDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
        if (targetDirection.magnitude > 1) targetDirection = targetDirection.normalized;

        var targetVelocity = new Vector3(targetDirection.x * hSpeed, targetDirection.y, targetDirection.z * vSpeed);
        _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _velocity, moveSmoothing);

        /*switch (hMove)
        {
            case > 0 when !_facingRight:
            case < 0 when _facingRight:
                Flip();
                break;
        }*/
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        transform.Rotate(0,180,0);
    }
}
