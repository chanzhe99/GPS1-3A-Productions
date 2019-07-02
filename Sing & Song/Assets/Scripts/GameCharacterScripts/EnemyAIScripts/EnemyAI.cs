﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : GameCharacter
{
    #region Player Component Variables
    [Header("Player Object Variable")]
    [SerializeField] protected GameObject playerObject;
    protected Transform playerTransform;
    protected SingScript playerScript;
    #endregion
    #region Spirit Armour Variables
    [Header("Spirit Armour Variables")]
    [SerializeField] private GameObject spiritArmour;
    [SerializeField] private bool haveSpiritArmour = false;
    [SerializeField] private float spiritArmourRecharge = 15f;
    private float spiritArmourRechargeTimer;
    #endregion
    #region Aggro Detection Variables
    [Header("Aggro Range")]
    [SerializeField] protected Vector2 playerDetectionRange;
    protected bool isDetectPlayerWhilePatrolling;
    #endregion
    #region State Variables
    [Header("Rest Time Variable")]
    [SerializeField] protected float restTime;
    protected float restTimeTimer;
    [Header("Retreat Time Variable")]
    [SerializeField] protected float retreatTime;
    protected float retreatTimeTimer;
    #endregion
    #region EnemyState Enums
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
    #endregion
    protected override void Initialise()
    {
        base.Initialise();
        #region Initialise Player Component Variables
        playerTransform = playerObject.GetComponent<Transform>();
        playerScript = playerObject.GetComponent<SingScript>();
        #endregion
        #region Initialise Spirit Armour Variables
        if(haveSpiritArmour) { spiritArmour.SetActive(true); }
        else { spiritArmour.SetActive(false); }
        #endregion
        enemyState = EnemyState.ENEMY_PATROLLING;
    }
    private void Update()
    {
        #region Check Health
        if (currentHealth <= 0f) { this.gameObject.SetActive(false); }
        #endregion
        #region Recharge Spirit Armour
        if (haveSpiritArmour && !spiritArmour.activeSelf)
        {
            if(spiritArmourRechargeTimer >= spiritArmourRecharge)
            {
                spiritArmourRechargeTimer = 0f;
                spiritArmour.SetActive(true);
            }
            else { spiritArmourRechargeTimer += Time.deltaTime; }
        }
        #endregion
        
        EnemySwitchState();
        print($"enemyState: {enemyState}");
        print($"enemyCurrentHealth: {currentHealth}");
    }
    private void EnemySwitchState()
    {
        switch(enemyState)
        {
            case EnemyState.ENEMY_PATROLLING:
                EnemyPatrol();
                break;
            case EnemyState.ENEMY_CHASING:
                EnemyFlip();
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
                break;
        }
    }
    private void EnemyFlip()
    {
        if(playerTransform.position.x > this.transform.position.x && !facingRight || playerTransform.position.x < this.transform.position.x && facingRight) { FlipCharacter(); }
    }
    protected virtual void EnemyPatrol()
    {
        isDetectPlayerWhilePatrolling = Physics2D.OverlapBox(this.colliderTransform.position, playerDetectionRange, 0f, playerLayer);
        if(isDetectPlayerWhilePatrolling) { enemyState = EnemyState.ENEMY_CHASING; }
    }
    protected virtual void EnemyChase() { }
    protected virtual void EnemyAttack() { }
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
    protected virtual void EnemyRetreat() { }
    public void DamageEnemyMelee()
    {
        if(playerTransform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; }
        else { knockbackDirection.x = knockbackForce.x; }
        if(enemyState != EnemyState.ENEMY_HIT)
        {
            enemyState = EnemyState.ENEMY_HIT;
            StartCoroutine(EnemyKnockback());
            if(!spiritArmour.activeSelf)
            {
                if (currentHealth > 0) { currentHealth -= 1; }
            }
        }
    } // Gets called when enemy is melee attacked by player
    private void DamageEnemySpirit()
    {
        if(spiritArmour.activeSelf)
        {
            knockbackDirection = new Vector2(0f, 0f);
            spiritArmour.SetActive(false);
            StartCoroutine(EnemyKnockback());
        }
        else
        {
            if(playerTransform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; }
            else { knockbackDirection.x = knockbackForce.x; }
            StartCoroutine(EnemyKnockback());
            if(currentHealth > 0) { currentHealth -= 2; }
        }
    } // Gets called when enemy is spirit attacked by player
    IEnumerator EnemyKnockback()
    {
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(knockbackDirection, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        enemyState = EnemyState.ENEMY_CHASING;
    } // Coroutine that runs when enemy is attacked
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) { playerScript.DamagePlayer(this.transform); }
    } // Enemy hits player collision check
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("SpiritAttack") && enemyState != EnemyState.ENEMY_HIT)
        {
            enemyState = EnemyState.ENEMY_HIT;
            DamageEnemySpirit();
        }
    } // Enemy is hit by spirit attack collision check
}
