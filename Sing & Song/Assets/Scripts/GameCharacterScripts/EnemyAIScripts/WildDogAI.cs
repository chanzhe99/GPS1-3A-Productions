using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildDogAI : EnemyAI
{
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
    }
    protected override void EnemyChase()
    {
        animator.SetBool("isResting", false);
        animator.SetBool("isRunning", true);
        canLunge = Physics2D.OverlapBox(this.colliderTransform.position, attackRange, 0f, playerLayer);
        if(canLunge)
        {
            animator.SetBool("isRunning", false);
            if (playerTransform.position.x > this.transform.position.x) { lungeDirection.x = lungeForce.x; }
            else { lungeDirection.x = -lungeForce.x; }
            enemyState = EnemyState.ENEMY_ATTACKING;
        }
        else { this.transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), moveSpeed * 2 * Time.deltaTime); }
    }
    protected override void EnemyAttack()
    {
        animator.SetTrigger("attack");
        if(!isLunge) { isLunge = true; StartCoroutine(Lunge()); SoundManagerScripts.PlaySound("wilddogAttack"); }//wild dog attack sound
    }
    protected override void EnemyRetreat()
    {
        if(retreatTimeTimer >= retreatTime)
        {
            animator.SetBool("isRunning", false);
            FlipCharacter();
            retreatTimeTimer = 0;
            enemyState = EnemyState.ENEMY_RESTING;
        }
        else
        {
            retreatTimeTimer += Time.deltaTime;
            this.rigidbody2D.velocity = new Vector3(lungeDirection.x * -0.5f, transform.position.y, transform.position.z);
        }
    }
    protected override void EnemyRest()
    {
        animator.SetBool("isResting", true);
        this.rigidbody2D.velocity = new Vector2(Vector2.zero.x, this.rigidbody2D.velocity.y);
        base.EnemyRest();
    }
    IEnumerator Lunge()
    {
        rigidbody2D.AddForce(lungeDirection, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        rigidbody2D.velocity = Vector2.zero;
        FlipCharacter();
        animator.SetBool("isRunning", true);
        enemyState = EnemyState.ENEMY_RETREATING;
        isLunge = false;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(this.colliderTransform != null)
        {
            Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange);
            Gizmos.DrawWireCube(this.colliderTransform.position, attackRange);
        }
    }
}
