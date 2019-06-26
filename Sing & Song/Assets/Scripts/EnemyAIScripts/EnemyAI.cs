using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : GameCharacter
{
    #region EnemyVariables
    protected PlayerController playerController;

    [SerializeField] protected Vector2 attackRange;

    [SerializeField] protected Vector2 playerDetectionRange;
    protected bool isDetectPlayerWhilePatrolling;
    protected LayerMask playerLayer;

    [SerializeField] protected float restTime;
    [SerializeField] protected float retreatTime;
    protected float restTimeTimer;
    protected float retreatTimeTimer;
    #endregion


    // Enemy States Enumerators
    protected enum EnemyState
    {
        ENEMY_PATROLLING,
        ENEMY_CHASING,
        ENEMY_ATTACKING,
        ENEMY_RESTING,
        ENEMY_RETREATING,
        ENEMY_HIT
    };
    protected EnemyState enemyState;

    protected override void Initialise()
    {
        playerController = playerObject.GetComponent<PlayerController>();
        playerLayer = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        EnemySwitchState();
    }

    private void EnemySwitchState()
    {
        switch(enemyState)
        {
            case EnemyState.ENEMY_PATROLLING:
                EnemyPatrol();
                break;
            case EnemyState.ENEMY_CHASING:
                EnemyChase();
                break;
            case EnemyState.ENEMY_ATTACKING:
                EnemyAttack();
                break;
            case EnemyState.ENEMY_RESTING:
                EnemyRest();
                break;
            case EnemyState.ENEMY_RETREATING:
                EnemyRetreat();
                break;
            case EnemyState.ENEMY_HIT:
                EnemyHit();
                break;
        }
        print($"enemyState: {enemyState}");
    }

    protected virtual void EnemyPatrol()
    {
        isDetectPlayerWhilePatrolling = Physics2D.OverlapBox(this.colliderTransform.position, playerDetectionRange, 0f, playerLayer);
        if (isDetectPlayerWhilePatrolling)
        {
            enemyState = EnemyState.ENEMY_CHASING;
        }
    }

    protected virtual void EnemyChase()
    {

    }

    protected virtual void EnemyAttack()
    {

    }

    protected virtual void EnemyRest()
    {
        if(restTimeTimer >= restTime)
        {
            restTimeTimer = 0;
            enemyState = EnemyState.ENEMY_CHASING;
        }
        else
        {
            restTimeTimer += Time.deltaTime;
            transform.position = transform.position;
        }
    }

    protected virtual void EnemyRetreat()
    {

    }

    protected virtual void EnemyHit()
    {
        if (playerObject.transform.position.x > this.transform.position.x)
        {
            knockbackDirection.x = -knockbackForce.x;
        }
        else
        {
            knockbackDirection.x = knockbackForce.x;
        }

        if(!isHit)
        {
            isHit = true;
            
            if (currentHealth > 0)
            {
                currentHealth -= 1;
            }
            else
            {
                Destroy(this.gameObject);
            }
            //print($"isHit: {isHit}");
            //print($"currentHealth: {currentHealth}");

            StartCoroutine(EnemyKnockback());
            
            enemyState = EnemyState.ENEMY_RESTING;
        }
    }

    IEnumerator EnemyKnockback()
    {
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(knockbackDirection, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        this.rigidbody2D.velocity = Vector2.zero;
        isHit = false;
    }

    public void DamageEnemy()
    {
        print("DAMAGE ENEMY CALLED");
        enemyState = EnemyState.ENEMY_HIT;
    }

    // Enemy Hits Player
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.DamagePlayer(this.gameObject);
        }
    }
}
