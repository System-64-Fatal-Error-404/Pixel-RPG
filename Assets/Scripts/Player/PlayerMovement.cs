using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _body;
    private Collider2D _collider;
    private Animator _animator;
    private InputActionProperty _input;
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction sprintAction;
    [SerializeField] InputAction jumpAction;
    [SerializeField] public float _hp = 10;
    [SerializeField] float moveSpeed = 5;
    [SerializeField] float sprintSpeed = 10;
    [SerializeField] float addSpeed = 5;
    [SerializeField] Rigidbody2D rock;
    [SerializeField] int addDamage = 2;
    [SerializeField] float rockSpeed = 5;
    [SerializeField] float jumpPower = 5;
    [SerializeField] float jumpDuration = 2f;

    private Rocks _rocks;

    private float rcRange = 6.5f;

    private bool isTouchingGrass = true;
    private bool canJump = true;
    
    Vector2 vec2Facing = Vector2.right;

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
    }
    
    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float moveX = moveInput.x;
        float moveY = moveInput.y;

        if (moveX == 1.0f)
        {
            vec2Facing = Vector2.right;
            transform.localScale = new Vector3(0.4f, transform.localScale.y, transform.localScale.z);
        }
        else if (moveX == -1.0f)
        {
            vec2Facing = Vector2.left;
            transform.localScale = new Vector3(-0.4f, transform.localScale.y, transform.localScale.z);
        }

        bool movement = Mathf.Abs(moveInput.x) > Mathf.Epsilon;
        bool sprint = sprintAction.IsPressed();

        float curTempSpeed = sprint ? sprintSpeed : moveSpeed;

        //_body.velocity = new Vector2(yaw * curTempSpeed, _body.velocity.y);
        
        _body.AddForce(Vector2.right * moveX * curTempSpeed, ForceMode2D.Force);
        Debug.Log("velocity.x: " + _body.velocity.x);
        
        //Cap Speed Limit
            if (_body.velocity.x >= curTempSpeed || _body.velocity.x <= -curTempSpeed && movement)
            {
                _body.velocity = new Vector2(moveX * curTempSpeed, _body.velocity.y);
                Debug.Log("Cap x velocity to: " + curTempSpeed);
            }
            
        if (moveY < 0.0f)
        {
            _body.AddForce(Vector2.down * curTempSpeed, ForceMode2D.Force);
        }

        if (isTouchingGrass && !movement)
        {
            _body.velocity = new Vector2(_body.velocity.x / 2, _body.velocity.y);
            Debug.Log("velocity = 0");
        }

        if (_body.velocity.x >= 0.5f || _body.velocity.x <= -0.5f)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }
        
        if (jumpAction.triggered && isTouchingGrass)
        {
            StartCoroutine(Jump());
        }

        if (attackAction.triggered && !hasShot && rockCount != 0)
        {
            StartCoroutine(Attack());
        }
        
        IEnumerator Jump()
        {
            Debug.Log("Jumped");
            isTouchingGrass = false;
            _animator.SetBool("IsJumping", true);
            
            _body.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            
            float elaspedTime = 0.0f;

            yield return new WaitForSeconds(2.0f);

            /*
            while(elaspedTime < jumpDuration / 2)
            {
                _body.velocity = new Vector3(_body.velocity.x, Mathf.Lerp(jumpPower, 0, elaspedTime / (jumpDuration / 2)));
                elaspedTime += Time.deltaTime;
                yield return null;
                
                _body.AddForce(Vector2.up * jumpPower);
                elaspedTime += Time.deltaTime;
                yield return null;
            }
            elaspedTime = 0.0f;

            while (elaspedTime < jumpDuration / 2)
            {
                _body.velocity = new Vector3(_body.velocity.x, Mathf.Lerp(0, -jumpPower, elaspedTime / (jumpDuration / 2)));
                elaspedTime += Time.deltaTime;
                yield return null;
                
                _body.AddForce(Vector2.up * -jumpPower);
                elaspedTime += Time.deltaTime;
                yield return null;
            }
            */
        }

        if (_hp <= 0) // so when the player dies they go to the loss screen
        {
            SceneManager.LoadScene("WinGame");
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor")
        {
            isTouchingGrass = true;
            Debug.Log("Touching Grass.");
            _animator.SetBool("IsJumping", false);
        }

        if (other.gameObject.tag == "Enemy")
        {
            _hp -= other.gameObject.GetComponent<EnemyProperties>().enemyAP;
        }

        if (other.gameObject.name == "Speed Powerup")
        {
            StartCoroutine(SpeedUp());
            Destroy(other.gameObject);
        }

        if (other.gameObject.name == "Damage Powerup")
        {
            StartCoroutine(DamageUp());
            Destroy(other.gameObject);
        }

        if (other.gameObject.name == "Rock Item")
        {
            rockCount++;

            Destroy(other.gameObject);
        }
    }

    IEnumerator Attack()
    {
        hasShot = true;
        rockCount--;

        var fireballInst = Instantiate(rock, transform.position, Quaternion.Euler(new Vector2(0, 0)));
        fireballInst.velocity = new Vector2(rockSpeed, 0);

        yield return new WaitForSeconds(1);
        hasShot = false;
    }

    IEnumerator SpeedUp()
    {
        moveSpeed += addSpeed;
        sprintSpeed += addSpeed;

        yield return new WaitForSeconds(10);

        moveSpeed -= addSpeed;
        sprintSpeed -= addSpeed;
    }

    IEnumerator DamageUp()
    {
        Debug.LogError("Test 1");
        _rocks.rockDamage += addDamage;

        yield return new WaitForSeconds(1);

        _rocks.rockDamage -= addDamage;
        Debug.LogError("Test 2");
    }
}
