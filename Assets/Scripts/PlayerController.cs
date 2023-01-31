using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement movementScript;
    private float _hMove;
    private float _vMove;
    
    // Start is called before the first frame update
    private void Awake()
    {
        movementScript = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        _hMove = Input.GetAxis("Horizontal");
        _vMove = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        movementScript.Move(_hMove, _vMove);
    }
}
