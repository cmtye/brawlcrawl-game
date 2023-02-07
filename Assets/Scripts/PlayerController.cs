using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private CharacterMovement _movementScript;
    
    private PlayerInputActions _playerControls;
    private InputAction _move;
    private InputAction _fire;
    
    private Vector2 _moveDirection = Vector2.zero;

    private void Awake()
    {
        _playerControls = new PlayerInputActions();
        _movementScript = GetComponent<CharacterMovement>();
    }
    private void OnEnable()
    {
        _move = _playerControls.Player.Move;
        _move.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        _moveDirection = _move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        _movementScript.Move(_moveDirection);
    }
}
