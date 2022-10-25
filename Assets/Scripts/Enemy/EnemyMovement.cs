using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyProperties _ep;
    
    protected Animator m_Animator;
    protected Rigidbody2D _body;
    public float currentHP => _ep.enemyHP;
    
    [SerializeField] private Transform plrTrans;
    private Vector2 targetPos;
    
    [SerializeField] protected EnemyStatus enemyStatus = EnemyStatus.Patrol;
    protected enum EnemyStatus
    {
        Idle,
        Patrol,
        Targeting,
        Attacking
    }
    private void Awake()
    {
        Debug.Log("current Eagle HP: " + currentHP);
        if (gameObject.activeSelf)
        {
            m_Animator = GetComponent<Animator>();
            _body = GetComponent<Rigidbody2D>();
            plrTrans = GameManager.plrTrans;    
            gameObject.tag = "Enemy";
            
            if (_ep.canFly)
            {
                _body.constraints = RigidbodyConstraints2D.FreezePositionY;
                _body.gravityScale = 0.0f;
            }
            else
            {
                _body.constraints = RigidbodyConstraints2D.None;
            }
        }
    }

    protected virtual void Update()
    {
        CheckStatus();
    }

    protected void CheckStatus()
    {
        switch (enemyStatus)
        {
            case EnemyStatus.Idle:
            {
                SetIdle();
            }
                break;

            case EnemyStatus.Patrol:
            {
                EnemySetActive();
            }
                break;
            
            case EnemyStatus.Targeting:
            {
                EnemySetActive();
                Targeting();
            }
                break;
            
            case EnemyStatus.Attacking:
            {
                
            }
                break;
        }
    }

    protected void EnemySetActive()
    {
        m_Animator.SetBool("isIdle", false);
        if (_ep.canFly)
        {
            m_Animator.SetBool("isFlying", true);
        }
        else
        {
            if (_body.velocity.x < _ep.enemyMinimumSpeed && _body.velocity.x > -_ep.enemyMinimumSpeed)
            {
                m_Animator.SetBool("isWalking", true);
                m_Animator.SetBool("isRunning", false);
            }
            else
            {
                m_Animator.SetBool("isWalking", false);
                m_Animator.SetBool("isRunning", true);
            }
                    
        }
    }

    protected virtual void SetIdle()
    {
        m_Animator.SetBool("isIdle", true);
    }
    protected virtual void Patrol()
    {
        
    }
    protected virtual void Targeting()
    {
        TargetPlayer();
    }
    protected virtual void Attack()
    {
        
    }
    protected virtual void TargetPlayer()
    {
        targetPos = plrTrans.position;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            
        }
    }
}
