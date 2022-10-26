using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyProperties _ep;
    [SerializeField] private List<Transform> pathPoints;

    protected Animator m_Animator;
    protected Rigidbody2D _body;
    public float currentHP;
    public float attackPower;
    
    [SerializeField] private Transform plrTrans;
    private Vector2 targetPos;
    private Vector2 plrPos;
    private Vector2 dirNormalized;
    
    [SerializeField] protected EnemyStatus enemyStatus = EnemyStatus.Patrol;
    
    //Path Finder
    private static float t = 0.0f;
    private static float td = 15.0f;
    private static float rd;
    private int i;
    
    protected enum EnemyStatus
    {
        Idle,
        Patrol,
        Targeting,
        Attacking
    }
    private void Awake()
    {
        rd = Random.Range(0f, td);
        t = math.round(rd);
        Debug.Log("Starting Time: " + t);
        
        TargetPlayer();
        plrPos = transform.position;
        currentHP = _ep.enemyHP;
        attackPower = _ep.enemyAP;
        
        Debug.Log("current Eagle HP: " + currentHP);
        if (gameObject.activeSelf)
        {
            m_Animator = GetComponent<Animator>();
            _body = GetComponent<Rigidbody2D>();
            plrTrans = GameManager.plrTrans;    
            gameObject.tag = "Enemy";
            
            if (_ep.canFly)
            {
                _body.constraints = RigidbodyConstraints2D.FreezeRotation;
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
        
        if (_body.velocity.x > 0.5f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
        }
        else if (_body.velocity.x < -0.5f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
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
                Patrol();
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
                StartCoroutine(Attack());
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
                m_Animator.SetBool("isRunning", false);
                m_Animator.SetBool("isWalking", true);
            }
            
            if(_body.velocity.x < _ep.enemyMaximumSpeed && _body.velocity.x > -_ep.enemyMaximumSpeed)
            {
                m_Animator.SetBool("isRunning", true);
                m_Animator.SetBool("isWalking", false);
            }
        }
    }

    //Set Enum Actions
    protected virtual void SetIdle()
    {
        m_Animator.SetBool("isIdle", true);
    }
    
    protected virtual void Patrol()
    {
        float ct = t / td;
        Debug.Log("currentTime: " + ct);

        plrPos = pathPoints[i].position;
        targetPos = Utility.Lerp(plrPos, pathPoints[i + 1].position, ct);

        Vector2 dir = targetPos - new Vector2(transform.position.x, transform.position.y);
        float mag = math.sqrt(math.exp2(dir.x) + math.exp2(dir.y));
        dirNormalized = dir/mag;
        
        //Move/Fly Towards Target
        if (_ep.canFly)
        {
            _body.AddForce(dirNormalized * _ep.enemySpeed, ForceMode2D.Force);
        }
        else
        {
            _body.AddForce(Vector2.right * dirNormalized * _ep.enemySpeed, ForceMode2D.Force);
        }
        

        if (_body.velocity.x >= _ep.enemyMinimumSpeed || _body.velocity.x <= -_ep.enemyMinimumSpeed)
        {
            _body.velocity = new Vector2(_ep.enemyMinimumSpeed, _body.velocity.y);
        }

        if (t >= td || Single.IsNaN(t))
        {
            t = 0;
            
            if (i + 1 >= pathPoints.Count - 1)
            {
                i = 0;
                Debug.Log("i Resets");
            }
            else
            {
                i++;
            }
        }
        else
        {
            t += Time.deltaTime;
        }
    }
    protected virtual void Targeting()
    {
        if (plrTrans.position.x - transform.position.x > 5)
        {
            if (!_ep.canFly)
            {
                _body.AddForce(new Vector2(Vector3.Normalize(plrTrans.position - transform.position).x, 0)* 5.0f, ForceMode2D.Force);
            }
        }
        else
        {
            _body.velocity = Vector2.zero;
            enemyStatus = EnemyStatus.Attacking;
        }
    }
    protected virtual IEnumerator Attack()
    {
        m_Animator.SetBool("isAttacking", true);
        
        if (Utility.InRangeDebug(transform, plrTrans, _ep.range ,_ep.pov))
        {
            
        }
        
        enemyStatus = EnemyStatus.Patrol;
        
        m_Animator.SetBool("isAttacking", true);
        yield return null;
    }
    
    //Methods
    protected void TargetPlayer()
    {
        targetPos = plrTrans.position;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            currentHP--;
            Destroy(other.gameObject, 1.0f);

            if (currentHP <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator Die()
    {
        m_Animator.SetBool("isDead", true);
        _body.constraints = RigidbodyConstraints2D.None;
        yield return new WaitForSeconds(3.5f);
        
        Destroy(gameObject);
    }
}
