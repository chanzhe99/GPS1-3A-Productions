using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingLemurAI : EnemyAI
{
    #region Dive Variables
    [Header("Dive Variables")]
    [SerializeField] private float diveRotateTime;
    [SerializeField] private float diveTime;
    private float diveRotateTimeTimer;
    private float diveTimeTimer;
    private float diveAngle;
    private bool canDive;
    #endregion


    #region 
    #endregion


    private Vector3 lemurDefaultPosition;
    private Vector3 playerDivePosition;
    
    private float diveDirection;
    [SerializeField] private float diveSpeed;
    [SerializeField] private float retreatingSpeed;
    


    protected override void Initialise()
    {
        base.Initialise();
        //lemurDefaultPosition.y = transform.position.y;
    }

    protected override void EnemyPatrol()
    {
        //this.transform.rotation = Quaternion.Euler();
        if(isDetectPlayer) { this.rigidbody2D.velocity = Vector2.zero; enemyState = EnemyState.ENEMY_CHASING; }
        if((Vector2)transform.position != spawnPosition) { transform.position = Vector2.MoveTowards(transform.position, spawnPosition, moveSpeed * Time.deltaTime); }
    }

    protected override void EnemyChase()
    {
        animator.SetBool("isChasing", true);
        canDive = Physics2D.OverlapCircle(this.colliderTransform.position, attackRange.x, playerLayer);
        diveAngle = Vector2.Angle(this.transform.position, playerTransform.position);
        diveRotateTimeTimer = 0f;
        if(canDive) { StartCoroutine(RotateTowardsPlayer()); }
        else { transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime); }
        //if(canDive)
        //{
        //    //turn lemur here
        //    lemurDefaultPosition.x = transform.position.x; // Stores Lemur's X Position as for Retreating
        //    playerDivePosition = playerTransform.position; // Stores Player's Position at the moment of Attacking
        //    enemyState = EnemyState.ENEMY_ATTACKING; // Changes Lemur's State to Attacking
    }

    protected override void EnemyAttack()
    {
        animator.SetBool("isAttacking", true);

        if (diveTimeTimer >= diveTime || isGrounded)
        {
            diveTimeTimer = 0;
            enemyState = EnemyState.ENEMY_RESTING;
        }
        else if(diveTimeTimer < diveTime)
        {
            diveTimeTimer += Time.deltaTime;
            transform.Translate((playerDivePosition - transform.position) * diveSpeed * Time.deltaTime);
            SoundManagerScripts.PlaySound("flyinglemurAttack");//flyinglemur attack sound
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

    private IEnumerator RotateTowardsPlayer()
    {
        print(diveAngle);
        yield return null;
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (this.colliderTransform != null)
        {
            Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange);
            Gizmos.DrawWireSphere(this.colliderTransform.position, attackRange.x);
        }
    }
}
