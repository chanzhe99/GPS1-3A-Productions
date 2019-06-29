using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingLemurAI : EnemyAI
{
    #region Ground Check Variables
    [SerializeField] protected Transform groundCheck;
    protected bool isGrounded;
    protected Vector2 checkSize;
    #endregion

    private Vector3 lemurPerchPosition;
    private Vector3 lemurDefaultPosition;
    private Vector3 playerDivePosition;
    private bool canDive;
    private float diveDirection;
    [SerializeField] private float diveSpeed;
    [SerializeField] private float retreatingSpeed;
    [SerializeField] private float diveTime;
    private float diveTimeTimer;
    

    protected override void Initialise()
    {
        base.Initialise();
        lemurPerchPosition = transform.position;
        lemurDefaultPosition.y = transform.position.y;

        checkSize = new Vector2(this.capsuleCollider2D.size.x * 0.95f, 0.1f);
    }

    protected override void EnemyPatrol()
    {

        base.EnemyPatrol();
        
        if(transform.position != lemurPerchPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, lemurPerchPosition, moveSpeed * Time.deltaTime);
        }
    }

    protected override void EnemyChase()
    {
        animator.SetBool("isChasing", true);
        canDive = Physics2D.OverlapCircle(this.colliderTransform.position, attackRange.x, playerLayer);

        if(canDive)
        {
            lemurDefaultPosition.x = transform.position.x; // Stores Lemur's X Position as for Retreating
            playerDivePosition = playerObject.transform.position; // Stores Player's Position at the moment of Attacking
            enemyState = EnemyState.ENEMY_ATTACKING; // Changes Lemur's State to Attacking
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerObject.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    protected override void EnemyAttack()
    {
        animator.SetBool("isAttacking", true);
        isGrounded = Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, terrainLayer);

        if (diveTimeTimer >= diveTime || isGrounded)
        {
            diveTimeTimer = 0;
            enemyState = EnemyState.ENEMY_RESTING;
        }
        else if(diveTimeTimer < diveTime)
        {
            diveTimeTimer += Time.deltaTime;
            transform.Translate((playerDivePosition - transform.position) * diveSpeed * Time.deltaTime);
        }
        
    }

    protected override void EnemyRest()
    {
        if (restTimeTimer >= restTime)
        {
            restTimeTimer = 0;
            enemyState = EnemyState.ENEMY_RETREATING;
        }
        else
        {
            restTimeTimer += Time.deltaTime;
            transform.position = transform.position;
        }
    }

    protected override void EnemyRetreat()
    {
        if(retreatTimeTimer >= retreatTime)
        {
            retreatTimeTimer = 0;
            enemyState = EnemyState.ENEMY_CHASING;
        }
        else
        {
            retreatTimeTimer += Time.deltaTime;
            transform.Translate((lemurDefaultPosition - transform.position) * retreatingSpeed * Time.deltaTime);
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (this.colliderTransform != null)
        {
            Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange); // do if check != NULL
            Gizmos.DrawWireSphere(this.colliderTransform.position, attackRange.x);
        }
    }
}
