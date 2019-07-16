using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class PangolinAI : EnemyAI
{
    #region CapsuleCollider2D Size Variables
    private Vector2 defaultColliderOffset;
    private Vector2 defaultColliderSize;
    private Vector2 rollColliderSize = new Vector2(0.5f, 1f);
    #endregion
    #region Spin Variables
    [Header("Spin Variables")]
    [SerializeField] private float spinTime = 0.5f;
    [SerializeField] private float spinSpeed = 500f;
    private float spinTimeTimer = 0f;
    private bool spinBounce;
    #endregion
    #region Roll Variables
    [Header("Roll Variables")]
    [SerializeField] private Vector2 rollForce = new Vector2(5f, 10f);
    [SerializeField] private float rollTime = 1f;
    private Vector2 rollDirection;
    private float rollTimeTimer = 0f;
    #endregion

    protected override void Initialise()
    {
        base.Initialise();
        defaultColliderOffset = capsuleCollider2D.offset;
        defaultColliderSize = capsuleCollider2D.size;
        rollDirection.y = rollForce.y;
    }
    protected override void EnemyPatrol()
    {
        animator.SetBool("resting", false);
        capsuleCollider2D.offset = defaultColliderOffset;
        capsuleCollider2D.size = defaultColliderSize;
        base.EnemyPatrol();
    }
    protected override void EnemyChase()
    {
        animator.SetBool("resting", false);
        animator.SetTrigger("spin");
        capsuleCollider2D.offset = Vector2.zero;
        capsuleCollider2D.size = rollColliderSize;
        enemySpriteTransform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        if(playerTransform.position.x > this.transform.position.x) { rollDirection.x = rollForce.x; }
        else { rollDirection.x = -rollForce.x; }
        if(!spinBounce)
        {
            this.rigidbody2D.AddForce(new Vector2(0f, rollDirection.y), ForceMode2D.Impulse);
            spinBounce = true;
        }
        else
        {
            if (spinTimeTimer >= spinTime)
            {
                spinTimeTimer = 0f;
                enemyState = EnemyState.ENEMY_ATTACKING;
            }
            else{ spinTimeTimer += Time.deltaTime; }
        }
    }
    protected override void EnemyAttack()
    {
        spinBounce = false;
        enemySpriteTransform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        if(rollTimeTimer >= rollTime)
        {
            rollTimeTimer = 0f;
            animator.SetTrigger("uncurl");
            enemyState = EnemyState.ENEMY_RESTING;
        }
        else
        {
            rollTimeTimer += Time.deltaTime;
            if(hitWall)
            {
                rollTimeTimer = 0f;
                this.rigidbody2D.velocity = Vector2.zero;
                this.rigidbody2D.AddForce(new Vector2(-rollDirection.x, rollDirection.y), ForceMode2D.Impulse);
                enemyState = EnemyState.ENEMY_RESTING;
            }
            else { this.rigidbody2D.AddForce(new Vector2(rollDirection.x * 3, this.rigidbody2D.velocity.y), ForceMode2D.Force); }
        }
    }
    protected override void EnemyRest()
    {
        animator.SetBool("resting", true);
        enemySpriteTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        capsuleCollider2D.offset = defaultColliderOffset;
        capsuleCollider2D.size = defaultColliderSize;
        base.EnemyRest();
    }
    public override void DamageEnemyMelee()
    {
        rollTimeTimer = 0f;
        if(playerTransform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; }
        else { knockbackDirection.x = knockbackForce.x; }
        if(enemyState == EnemyState.ENEMY_RESTING)
        {
            StartCoroutine(EnemyKnockback());
            if(!spiritArmour.activeSelf)
            {
                if(currentHealth > 0) { currentHealth -= 1; }
            }
        }
        else if(enemyState != EnemyState.ENEMY_HIT)
        {
            enemyState = EnemyState.ENEMY_HIT;
            StartCoroutine(EnemyKnockback());
        }
    }
    protected override void DamageEnemySpirit()
    {
        if(spiritArmour.activeSelf)
        {
            knockbackDirection = new Vector2(0f, 0f);
            spiritArmour.SetActive(false);
            StartCoroutine(EnemyKnockback());
        }
        else
        {
            rollTimeTimer = 0f;
            if (playerTransform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; }
            else { knockbackDirection.x = knockbackForce.x; }
            StartCoroutine(EnemyKnockback());
            if (currentHealth > 0) { currentHealth -= 2; }
        }
    }
    protected override IEnumerator EnemyKnockback()
    {
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(knockbackDirection, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        this.rigidbody2D.velocity = Vector2.zero;
        if(enemyState != EnemyState.ENEMY_RESTING) { enemyState = EnemyState.ENEMY_CHASING; }
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(this.colliderTransform != null) { Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange); }
    }
}