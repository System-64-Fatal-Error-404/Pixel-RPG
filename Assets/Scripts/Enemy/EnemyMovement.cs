using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyProperties _ep;
    [SerializeField] private List<Transform> pathPoints;
    public LayerMask targetLayer;
    public LayerMask obstructLayer;
    
    public bool canSeePlayer { get; private set; }

    protected Animator m_Animator;
    protected Rigidbody2D _body;
    public float currentHP;
    public float attackPower;
    
    [SerializeField] private GameObject plr;
    private Vector2 targetPos;
    private Vector2 plrPos;

    private Vector2 vec2Facing;
    
    float angle;

    [SerializeField] protected EnemyStatus enemyStatus = EnemyStatus.Patrol;
    
    //Path Finder
    private static float t = 0.0f;
    private static float td = 9.0f;
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
        plrPos = plr.transform.position;
        currentHP = _ep.enemyHP;
        attackPower = _ep.enemyAP;
        
        Debug.Log("current Eagle HP: " + currentHP);
        
        if (gameObject.activeSelf)
        {
            m_Animator = GetComponent<Animator>();
            _body = GetComponent<Rigidbody2D>();
            plrPos = GameManager.plrTrans.position;
            
            gameObject.tag = "Enemy";
            
            if (_ep.canFly)
            {
                _body.constraints = RigidbodyConstraints2D.FreezeRotation;
                _body.gravityScale = 0.0f;
            }
            else
            {
                _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        if (_ep.canFly)
        {
            //angle
            angle = 90;
        }
        else
        {
            //angle
            angle = 45;
        }
        
        StartCoroutine(FOVCheck());
    }

    protected virtual void Update()
    {
        CheckStatus();
        
        if (_body.velocity.x > 1.0f)
        {
            float faceRight = transform.localScale.x;
            
            if (faceRight > 0)
            {
                faceRight = -faceRight;
            }
            vec2Facing = Vector2.right;
            transform.localScale = new Vector3(faceRight, transform.localScale.y, transform.localScale.z);
        }
        else if (_body.velocity.x < -1.0f)
        {
            float faceLeft = transform.localScale.x;
            
            if (faceLeft < 0)
            {
                faceLeft = -faceLeft;
            }
            vec2Facing = Vector2.left;
            transform.localScale = new Vector3(faceLeft, transform.localScale.y, transform.localScale.z);
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
        if (canSeePlayer)
            enemyStatus = EnemyStatus.Targeting;
        
        float ct = t / td;
        Debug.Log("currentTime: " + ct);

        plrPos = pathPoints[i].position;
        targetPos = Lerp(plrPos, pathPoints[i + 1].position, ct);

        Vector2 dirNormalized = (targetPos - new Vector2(transform.position.x, transform.position.y)).normalized;
        
        //float mag = math.sqrt(math.exp2(dir.x) + math.exp2(dir.y));
        //dirNormalized = dir/mag;
        
        //Move/Fly Towards Target
        if (_ep.canFly)
        {
            _body.AddForce(dirNormalized * _ep.enemySpeed, ForceMode2D.Force);
        }
        else
        {
            _body.AddForce(Vector2.right * dirNormalized * _ep.enemySpeed, ForceMode2D.Force);
            
            //setLimit
            if (Vector2.Distance(targetPos, transform.position) < 0.5)
            {
                _body.velocity = new Vector2(0.02f, _body.velocity.y);
            }
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
        if (plr.transform.position.x - transform.position.x > 5)
        {
            if (!_ep.canFly)
            {
                _body.AddForce(new Vector2(Vector3.Normalize(plr.transform.position - transform.position).x, 0)* 5.0f, ForceMode2D.Force);
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
        TargetPlayer();

        Vector2 dirNormalized = (targetPos - new Vector2(transform.position.x, transform.position.y)).normalized;
        
        m_Animator.SetBool("isAttacking", true);

        if (_ep.canFly)
        {
            _body.AddForce(dirNormalized * _ep.enemySpeed, ForceMode2D.Force);
        }
        else
        {
            _body.AddForce(new Vector2(dirNormalized.x * _ep.enemySpeed, 1), ForceMode2D.Impulse);
        }

        if (Vector2.Distance(transform.position, targetPos) < 1)
        {
            enemyStatus = EnemyStatus.Patrol;  
        }
        
        
        m_Animator.SetBool("isAttacking", true);
        yield return null;
    }
    
    //Methods
    protected void TargetPlayer()
    {
        targetPos = plr.transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            currentHP -= 0.8f;
            Destroy(other.gameObject);

            if (currentHP <= 0.0f)
            {
                StartCoroutine(Die());
            }
        }

        if (other.gameObject.tag == "Player")
        {
            enemyStatus = EnemyStatus.Patrol;
            
            _body.AddForce(-vec2Facing * 6.0f, ForceMode2D.Impulse);
            currentHP--;
        }
    }

    private IEnumerator Die()
    {
        m_Animator.SetBool("isDead", true);
        _body.constraints = RigidbodyConstraints2D.None;
        yield return new WaitForSeconds(3.5f);
        
        Destroy(gameObject);
    }

    void FOV()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, _ep.range, targetLayer);

        if (rangeCheck.Length > 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(transform.up, dirToTarget) < angle / 2)
            {
                float disToTarget = Vector2.Distance(transform.position, target.position);

                if (Physics2D.Raycast(transform.position, dirToTarget, disToTarget, obstructLayer))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
            {
                canSeePlayer = false;
            }
        }else if (canSeePlayer)
            canSeePlayer = false;
    }

    IEnumerator FOVCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);;
            FOV();
        }
    }
    
    //Lerp Function
    public Vector2 Lerp(Vector2 start, Vector2 end, float t)
    {
        return (1-t) * start + (t) * end;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.forward, _ep.range);

        Vector3 angle1 = DirFromAngle(-transform.eulerAngles.x, -angle / 2);
        Vector3 angle2 = DirFromAngle(-transform.eulerAngles.x, angle / 2);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle1 * _ep.range);
        Gizmos.DrawLine(transform.position, transform.position + angle2 * _ep.range);
    }

    Vector2 DirFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
