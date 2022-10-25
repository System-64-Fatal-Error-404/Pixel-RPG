using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyProperties _ep;

    protected Rigidbody2D _body;
    protected Transform plrTrans;

    protected Animator m_Animator;
    
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
        if (gameObject.activeSelf)
        {
            m_Animator = GetComponent<Animator>();
            _body = GetComponent<Rigidbody2D>();
            plrTrans = GameManager.plrTrans;    
            gameObject.tag = "Enemy";
            
            if (_ep.canFly)
            {
                _body.constraints = RigidbodyConstraints2D.FreezePositionY;
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

    protected void SetIdle()
    {
        m_Animator.SetBool("isIdle", true);
    }
}
