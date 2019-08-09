using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : GameCharacter
{
    protected bool isAwayChangePlayer = true;

    #region Component Variables
    [Header("Component Variables")]
    [SerializeField] protected Transform enemySpriteTransform;
    protected GameObject playerObject;
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
    #region Death Variables
    protected float deathFadeOutTime = 3f;
    protected float deathFadeOutTimeTimer;
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
        ENEMY_HIT,
        ENEMY_DEAD
    };
    protected EnemyState enemyState;
    #endregion
    protected override void Initialise()
    {
        base.Initialise();
        #region Initialise Player Component Variables
        playerObject = GameObject.FindGameObjectWithTag("Player");
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
        this.patrolSpeed = (this.doesPatrol) ? 1 : 0;
        #endregion
        this.enemyState = EnemyState.ENEMY_PATROLLING;
    }
    private void Update()
    {
        #region Check Health
        if (this.currentHealth <= 0f) { this.enemyState = EnemyState.ENEMY_DEAD; }
        //if respawn, spawn at spawnposition
        #endregion
        #region Recharge Spirit Armour
        if (haveSpiritArmour && !spiritArmour.activeSelf)
        {
            if (spiritArmourRechargeTimer >= spiritArmourRecharge)
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
        if (isAwayChangePlayer)
        {
            this.isDetectPlayer = Physics2D.OverlapBox(this.colliderTransform.position, this.playerDetectionRange, 0f, playerLayer);
            if (this.enemyState != EnemyState.ENEMY_PATROLLING)
            {
                if (this.isDetectPlayer) { this.stopAggroTimeTimer = 0; }
                else
                {
                    if (this.stopAggroTimeTimer >= this.stopAggroTime)
                    {
                        this.enemyState = EnemyState.ENEMY_PATROLLING;
                        this.stopAggroTimeTimer = 0;
                    }
                    else { this.stopAggroTimeTimer += Time.deltaTime; }
                }
            }

        }
        
        #endregion
        
        EnemySwitchState();
        //print($"enemyState: {enemyState}");
        //print($"enemyCurrentHealth: {currentHealth}");
        //print($"hitWall: {hitWall}");
        //print($"hitEdge: {hitEdge}");
    }
    private void EnemySwitchState()
    {
        switch (enemyState)
        {
            case EnemyState.ENEMY_PATROLLING:
                this.EnemyPatrol();
                break;
            case EnemyState.ENEMY_CHASING:
                //Debug.LogError("Remember to set it back");
                if (isAwayChangePlayer)
                {
                    if (playerTransform.position.x > this.transform.position.x && this.facingRight == false || playerTransform.position.x < this.transform.position.x && this.facingRight) { this.FlipCharacter(); }
                }
                this.EnemyChase();
                break;
            case EnemyState.ENEMY_ATTACKING:
                this.EnemyAttack();
                break;
            case EnemyState.ENEMY_RESTING:
                this.EnemyRest();
                break;
            case EnemyState.ENEMY_RETREATING:
                this.EnemyRetreat();
                break;
            case EnemyState.ENEMY_HIT:
                break;
            case EnemyState.ENEMY_DEAD:
                if(this.deathFadeOutTimeTimer == 0)
                {
                    this.animator.SetTrigger("die");
                    EnemyDieSound();
                    //capsuleCollider2D.enabled = false; // for testing
                    //rigidbody2D.gravityScale = 0.0f; // for testing
                }
                if (this.deathFadeOutTimeTimer >= this.deathFadeOutTime)
                {
                    this.deathFadeOutTimeTimer = 0f;
                    this.gameObject.SetActive(false);
                    RhinoDie();
                }
                else
                {
                    this.deathFadeOutTimeTimer += Time.deltaTime;
                }
                break;
        }
    }

    protected virtual void EnemyDieSound() { }
    protected virtual void RhinoDie() { }

    private void UpdateWallRaycast()
    {
        //change this.colliderTransform.position to this.capsuleCollider2D.bounds.center
        //this.rayOrigin = this.colliderTransform.position + (-this.transform.right * this.capsuleCollider2D.size.x * 0.5f);
        this.rayOrigin = this.capsuleCollider2D.bounds.center + (-this.transform.right * this.capsuleCollider2D.size.x * 0.5f);
        this.hitWall = Physics2D.Raycast(this.rayOrigin, -this.transform.right, this.wallRayLength, terrainLayer);
        this.hitEdge = !Physics2D.Raycast(this.rayOrigin, -this.transform.up, this.edgeRayLength, terrainLayer);
        Debug.DrawRay(rayOrigin, -transform.right * wallRayLength, Color.red);
        Debug.DrawRay(rayOrigin, -transform.up * edgeRayLength, Color.red);
    }
    protected virtual void EnemyPatrol()
    {
        this.rigidbody2D.velocity = new Vector2(-this.transform.right.x * this.moveSpeed * this.patrolSpeed, this.rigidbody2D.velocity.y);
        if(this.hitWall || this.hitEdge) { this.FlipCharacter(); }
        if(this.isDetectPlayer) { this.rigidbody2D.velocity = Vector2.zero; this.enemyState = EnemyState.ENEMY_CHASING; }
    }
    protected virtual void EnemyChase() {}
    protected virtual void EnemyAttack() {}
    protected virtual void EnemyRest()
    {
        if(this.restTimeTimer >= this.restTime)
        {
            this.restTimeTimer = 0;
            this.enemyState = EnemyState.ENEMY_CHASING;
        }
        else
        {
            this.restTimeTimer += Time.deltaTime;
        }
    }
    protected virtual void EnemyRetreat() {}
    public virtual void DamageEnemyMelee()
    {
        if(playerTransform.position.x >= this.transform.position.x) { this.knockbackDirection.x = -this.knockbackForce.x; }
        else { this.knockbackDirection.x = this.knockbackForce.x; }
        if(this.enemyState != EnemyState.ENEMY_HIT)
        {
            this.enemyState = EnemyState.ENEMY_HIT;
            StartCoroutine(this.EnemyKnockback());
            if(!spiritArmour.activeSelf)
            {
                if(this.currentHealth > 0) { this.currentHealth -= 1; }
            }
        }
    } // Gets called when enemy is melee attacked by player
    protected virtual void DamageEnemySpirit()
    {
        if(spiritArmour.activeSelf)
        {
            this.knockbackDirection = new Vector2(0f, 0f);
            spiritArmour.SetActive(false);
            StartCoroutine(this.EnemyKnockback());
        }
        else
        {
            if(playerTransform.position.x >= this.transform.position.x) { this.knockbackDirection.x = -this.knockbackForce.x; }
            else { this.knockbackDirection.x = this.knockbackForce.x; }
            StartCoroutine(this.EnemyKnockback());
            if(this.currentHealth > 0) { this.currentHealth -= 2; }
        }
    } // Gets called when enemy is spirit attacked by player
    protected virtual IEnumerator EnemyKnockback()
    {
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(this.knockbackDirection, ForceMode2D.Impulse);
        yield return new WaitForSeconds(this.knockbackTime);
        this.rigidbody2D.velocity = Vector2.zero;
        this.enemyState = EnemyState.ENEMY_CHASING;
    } // Coroutine that runs when enemy is attacked
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) { collision.gameObject.GetComponent<SingScript>().DamagePlayer(this.transform); }
    } // Enemy hits player collision check
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("SpiritAttack") && this.enemyState != EnemyState.ENEMY_HIT)
        {
            this.enemyState = EnemyState.ENEMY_HIT;
            this.DamageEnemySpirit();
        }
    } // Enemy is hit by spirit attack collision check
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }
}
