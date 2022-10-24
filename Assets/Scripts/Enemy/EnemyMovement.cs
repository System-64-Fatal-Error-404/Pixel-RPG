using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyProperties _ep;

    private Rigidbody2D _body;
    private Transform plrTrans;

    private Animator m_Animator;
    
    [SerializeField] private EnemyStatus enemyStatus = EnemyStatus.Patrol;
    enum EnemyStatus
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

    private void Update()
    {
        
        switch (enemyStatus)
        {
            case EnemyStatus.Idle:
            {
                
            }
                break;

            case EnemyStatus.Patrol:
            {
                
            }
                break;
            
            case EnemyStatus.Targeting:
            {
                
            }
                break;
            
            case EnemyStatus.Attacking:
            {
                
            }
                break;
        }
    }
}
