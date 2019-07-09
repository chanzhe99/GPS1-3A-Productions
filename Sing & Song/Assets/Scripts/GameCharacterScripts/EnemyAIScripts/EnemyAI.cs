using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : GameCharacter
{
    #region Component Variables
    [Header("Component Variables")]
    [SerializeField] protected GameObject playerObject;
    [SerializeField] protected Transform enemySpriteTransform;
    protected Transform playerTransform;
    protected SingScript playerScript;
    #endregion
    #region Wall & Edge Raycast Variables
    private Vector2 rayOrigin;
    private float wallRayLength;
    private float edgeRayLength;
    protected bool hitWall;
    protected bool hitEdge;
    #endregion
    #region Spirit Armour Variables
    [Header("Spirit Armour Variables")]
    [SerializeField] protected GameObject spiritArmour;
    [SerializeField] private bool haveSpiritArmour = false;
    [SerializeField] private float spiritArmourRecharge = 10f;
    private float spiritArmourRechargeTimer;
    #endregion
    #region Patrol Variables
    [Header("Patrol Variables")]
    [SerializeField] protected bool doesPatrol;
    protected int patrolSpeed;
    #endregion
    #region Aggro Detection Variables
    [Header("Aggro Range")]
    [SerializeField] protected Vector2 playerDetectionRange;
    protected bool isDetectPlayer;
    private float stopAggroTime = 10f;
    private float stopAggroTimeTimer;
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
        #region Initialise Wall & Edge Raycast Variables
        wallRayLength = capsuleCollider2D.size.x * 0.05f;
        edgeRayLength = capsuleCollider2D.size.y * 1.5f;
        #endregion
        #region Initialise Spirit Armour Variables
        if (haveSpiritArmour) { spiritArmour.SetActive(true); }
        else { spiritArmour.SetActive(false); }
        #endregion
        #region Initialise Patrol Variables
        patrolSpeed = (doesPatrol) ? 1 : 0;
        #endregion
        enemyState = EnemyState.ENEMY_PATROLLING;
    }
    private void Update()
    {
        #region Check Health
        if (currentHealth <= 0f) { this.gameObject.SetActive(false); }
        //if respawn, spawn at spawnposition
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
        #region Check Ground & Ceiling
        UpdateRaycastOrigins();
        VerticalCollisionDetection();
        #endregion
        #region Check Walls & Edges
        UpdateWallRaycast();
        #endregion
        #region Check Aggro
        isDetectPlayer = Physics2D.OverlapBox(this.colliderTransform.position, playerDetectionRange, 0f, playerLayer);
        if(enemyState != EnemyState.ENEMY_PATROLLING)
        {
            if(isDetectPlayer) { stopAggroTimeTimer = 0; }
            else
            {
                if(stopAggroTimeTimer >= stopAggroTime)
                {
                    enemyState = EnemyState.ENEMY_PATROLLING;
                    stopAggroTimeTimer = 0;
                }
                else { stopAggroTimeTimer += Time.deltaTime; }
            }
        }
        #endregion
        
        EnemySwitchState();
        //print($"enemyState: {enemyState}");
        print($"enemyCurrentHealth: {currentHealth}");
        //print($"hitWall: {hitWall}");
        //print($"hitEdge: {hitEdge}");
    }
    private void EnemySwitchState()
    {
        switch(enemyState)
        {
            case EnemyState.ENEMY_PATROLLING:
                EnemyPatrol();
                break;
            case EnemyState.ENEMY_CHASING:
                if(playerTransform.position.x > this.transform.position.x && !facingRight || playerTransform.position.x < this.transform.position.x && facingRight) { FlipCharacter(); }
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
    private void UpdateWallRaycast()
    {
        rayOrigin = colliderTransform.position + (-transform.right * capsuleCollider2D.size.x * 0.5f);
        hitWall = Physics2D.Raycast(rayOrigin, -transform.right, wallRayLength, terrainLayer);
        hitEdge = !Physics2D.Raycast(rayOrigin, -transform.up, edgeRayLength, terrainLayer);
        Debug.DrawRay(rayOrigin, -transform.right * wallRayLength, Color.red);
        Debug.DrawRay(rayOrigin, -transform.up * edgeRayLength, Color.red);
    }
    protected virtual void EnemyPatrol()
    {
        this.rigidbody2D.velocity = new Vector2(-this.transform.right.x * moveSpeed * patrolSpeed, this.rigidbody2D.velocity.y);
        if(hitWall || hitEdge) { FlipCharacter(); }
        if(isDetectPlayer) { this.rigidbody2D.velocity = Vector2.zero; enemyState = EnemyState.ENEMY_CHASING; }
    }
    protected virtual void EnemyChase() {}
    protected virtual void EnemyAttack() {}
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
        }
    }
    protected virtual void EnemyRetreat() {}
    public virtual void DamageEnemyMelee()
    {
        if(playerTransform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; }
        else { knockbackDirection.x = knockbackForce.x; }
        if(enemyState != EnemyState.ENEMY_HIT)
        {
            enemyState = EnemyState.ENEMY_HIT;
            StartCoroutine(EnemyKnockback());
            if(!spiritArmour.activeSelf)
            {
                if(currentHealth > 0) { currentHealth -= 1; }
            }
        }
    } // Gets called when enemy is melee attacked by player
    protected virtual void DamageEnemySpirit()
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
    protected virtual IEnumerator EnemyKnockback()
    {
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(knockbackDirection, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        this.rigidbody2D.velocity = Vector2.zero;
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
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }
}
