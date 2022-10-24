using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D _body;
    public InputActionProperty _input;
    
    public bool isMoving => _body.velocity.sqrMagnitude > 0.05f;
    
    void Start()
    {
        _body.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
