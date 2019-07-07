using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class PangolinAI : EnemyAI
{
    [SerializeField] private float rollChargeTime;
    [SerializeField] private float rollTime;
    [SerializeField] private Vector2 rollForce;
    [SerializeField] private float spinSpeed = 500f;
    private float rollChargeTimeTimer;
    private float rollTimeTimer;
    private Vector2 rollDirection;
    private bool bounceCharge;

    protected override void Initialise()
    {
        base.Initialise();
        rollDirection.y = rollForce.y;
    }
    protected override void EnemyPatrol()
    {
        base.EnemyPatrol();
    }
    protected override void EnemyChase()
    {
        enemySpriteTransform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        if (playerTransform.position.x > this.transform.position.x) { rollDirection.x = rollForce.x; }
        else { rollDirection.x = -rollForce.x; }
        if (!bounceCharge)
        {
            this.rigidbody2D.AddForce(new Vector2(0f, rollDirection.y), ForceMode2D.Impulse);
            if(isGrounded) { bounceCharge = true; }
        }
        else
        {
            if(rollChargeTimeTimer >= rollChargeTime)
            {
                rollChargeTimeTimer = 0f;
                enemyState = EnemyState.ENEMY_ATTACKING;
            }
            else{ rollChargeTimeTimer += Time.deltaTime; }
        }
    }
    protected override void EnemyAttack()
    {
        bounceCharge = false;
        enemySpriteTransform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        if(rollTimeTimer >= rollTime)
        {
            rollTimeTimer = 0f;
            enemyState = EnemyState.ENEMY_RESTING;
        }
        else
        {
            rollTimeTimer += Time.deltaTime;
            if(hitWall)
            {
                this.rigidbody2D.velocity = Vector2.zero;
                this.rigidbody2D.AddForce(new Vector2(-rollDirection.x * 3, rollDirection.y), ForceMode2D.Force);
                enemyState = EnemyState.ENEMY_RESTING;
            }
            else if(hitEdge)
            {
                this.rigidbody2D.velocity = Vector2.zero;
                this.rigidbody2D.AddForce(new Vector2(-rollDirection.x * 3, rollDirection.y), ForceMode2D.Force);
                enemyState = EnemyState.ENEMY_RESTING;
            }
            else
            {
                this.rigidbody2D.AddForce(new Vector2(rollDirection.x * 3, this.rigidbody2D.velocity.y), ForceMode2D.Force);
            }
        }
    }
    protected override void EnemyRest()
    {
        base.EnemyRest();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (this.colliderTransform != null)
        {
            Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange);
            Gizmos.DrawWireCube(this.colliderTransform.position, attackRange);
        }
    }
}
