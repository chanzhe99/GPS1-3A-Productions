using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildDogAI : EnemyAI
{
    #region Patrol Variables
    [Header("Wild Dog Patrol Variables")]
    [SerializeField] private Transform patrolRayOrigin;
    [SerializeField] private float patrolDistance;
    #endregion
    #region Attack Variables
    [Header("Wild Dog Attack Variables")]
    [SerializeField] private Vector2 lungeForce;
    private Vector2 lungeDirection;
    private bool canLunge;
    private bool isLunge;
    #endregion
    protected override void Initialise()
    {
        base.Initialise();
        lungeDirection.y = lungeForce.y;
    }
    
    protected override void EnemyPatrol()
    {
        base.EnemyPatrol();
        this.rigidbody2D.velocity = new Vector2(-transform.right.x * moveSpeed, this.rigidbody2D.velocity.y);
        Debug.DrawRay(patrolRayOrigin.position, -transform.right * 0.2f, Color.red);
        //else { rigidbody2D.velocity = new Vector2(moveSpeed, rigidbody2D.velocity.y); }
    }
    protected override void EnemyChase()
    {
        canLunge = Physics2D.OverlapBox(this.colliderTransform.position, attackRange, 0f, playerLayer);
        if(canLunge)
        {
            if(playerTransform.position.x > this.transform.position.x) { lungeDirection.x = lungeForce.x; }
            else { lungeDirection.x = -lungeForce.x; }
            enemyState = EnemyState.ENEMY_ATTACKING;
        }
        else { transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), moveSpeed * Time.deltaTime); }
    }
    protected override void EnemyAttack()
    {
        if(!isLunge)
        {
            isLunge = true;
            StartCoroutine(Lunge());
        }
    }
    protected override void EnemyRetreat()
    {
        if (retreatTimeTimer >= retreatTime)
        {
            retreatTimeTimer = 0;
            enemyState = EnemyState.ENEMY_RESTING;
        }
        else
        {
            retreatTimeTimer += Time.deltaTime;
            transform.Translate(new Vector3(lungeDirection.x * -0.25f, transform.position.y, transform.position.z) * Time.deltaTime);
        }
    }
    IEnumerator Lunge()
    {
        rigidbody2D.AddForce(lungeDirection, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        rigidbody2D.velocity = Vector2.zero;
        enemyState = EnemyState.ENEMY_RETREATING;
        isLunge = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (this.colliderTransform != null)
        {
            Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange); // do if check != NULL
            Gizmos.DrawWireCube(this.colliderTransform.position, attackRange);
        }
    }
}
