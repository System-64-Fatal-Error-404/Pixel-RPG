using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyProperties _ep;

    private Rigidbody2D _body;
    private Transform plrTrans;
    
    [SerializeField] private enemyStatus EnemyStatus = enemyStatus.Idle;
    enum enemyStatus
    {
        Idle,
        Patrol,
        Targeting,
        Attacking
    }
    private void Awake()
    {
        if (gameObject != null)
        {
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
        switch (EnemyStatus)
        {
            case enemyStatus.Idle:
            {
                    
            }
                break;

            case enemyStatus.Patrol:
            {
                
            }
                break;
            
            case enemyStatus.Targeting:
            {
                
            }
                break;
            
            case enemyStatus.Attacking:
            {
                
            }
                break;
        }
    }
}
