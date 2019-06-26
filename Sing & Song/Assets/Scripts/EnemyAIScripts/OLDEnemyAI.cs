using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLDEnemyAI : MonoBehaviour
{
    // Enemy Component + Other Objects Variables
    protected Rigidbody2D enemyRigidbody2D;
    protected Transform enemyColliderTransform;
    protected Transform playerColliderTransform;
    private PlayerController playerController;
    private float directionFloat;

    // Enemy Patrolling Variables
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected bool moveRightOnAwake;
    [SerializeField] protected Vector2 playerDetectionRange;
    [SerializeField] private float stopChasingTime;
    protected bool isDetectPlayerWhilePatrolling;
    protected bool isDetectPlayerWhileChasing;
    protected LayerMask playerLayer;
    private float stopChasingTimeTimer;

    // Enemy Sprite Flip Variables
    private Quaternion targetRotation;
    private float rotationTime;
    private bool enemyFlipped;

    // Enemy Raycast Variables
    [SerializeField] private float wallRaycastDistance;
    private Transform raycastOrigin;
    private RaycastHit2D groundRaycast;
    private RaycastHit2D wallRaycast;

    // Enemy Knockback Variables
    [SerializeField] private Vector2 knockbackForce;
    private float knockbackDirection;
    protected bool enemyIsHit;

    // Enemy HP Variables
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;

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

    private void Awake()
    {
        OnAwake();
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyColliderTransform = transform.Find("EnemyCollider").transform;
        playerColliderTransform = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerCollider").transform;
        raycastOrigin = transform.Find("EnemyCollider/RaycastOrigin").transform;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerLayer = LayerMask.GetMask("Player");
        enemyState = EnemyState.ENEMY_PATROLLING;
        
        if(moveRightOnAwake)
        {
            transform.Rotate(0, 180, 0);
            directionFloat = 1;
        }
        else
        {
            
            directionFloat = -1;
        }
    }

    private void Update()
    {
        EnemySwitchState();

        AdjustCurrentHealth(0);
    }

    protected virtual void OnAwake()
    {
        print("OnAwaken");
    }

    protected Vector2 DirectionToTarget(Vector2 currentPosition, Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - currentPosition;

        return direction;
    }

    // Checks Enemy States
    protected virtual void EnemySwitchState()
    {
        switch(enemyState)
        {
            case EnemyState.ENEMY_PATROLLING:
                EnemyPatrolBehavior();
                break;
            case EnemyState.ENEMY_CHASING:
                EnemyChasingBehavior();
                break;
        };
    }

    // Default Enemy Patrol Function
    protected virtual void EnemyPatrolBehavior()
    {
        // Enemy checks if Player enters range of detection
        isDetectPlayerWhilePatrolling = Physics2D.OverlapBox(enemyColliderTransform.position, playerDetectionRange, 0f, playerLayer);
        if (isDetectPlayerWhilePatrolling)
        {
            enemyState = EnemyState.ENEMY_CHASING;
        }

        // Enemy Patrolling Movement
        enemyRigidbody2D.velocity = new Vector2(moveSpeed * directionFloat, enemyRigidbody2D.velocity.y);
        groundRaycast = Physics2D.Raycast(raycastOrigin.position, Vector2.down, 2);
        wallRaycast = Physics2D.Raycast(raycastOrigin.position, transform.right, wallRaycastDistance);

        // Enemy turns around when at an edge or hits a wall
        if (!groundRaycast.collider || !wallRaycast.collider.CompareTag("Player"))
        {
            if(!enemyFlipped)
            {
                enemyFlipped = true;
                EnemyFlip();
            }
        }
    }

    // Enemy Chase/StopChasing Function
    private void EnemyChasingBehavior()
    {
        // Enemy checks if Player remains inside Chase range
        isDetectPlayerWhileChasing = Physics2D.OverlapBox(enemyColliderTransform.position, new Vector2(playerDetectionRange.x, playerDetectionRange.y), 0f, playerLayer);
        if (isDetectPlayerWhileChasing)
        {
            stopChasingTimeTimer = 0; // If Player is within Chase range, resets StopChasingTime
        }
        else
        {   // Enemy goes back to Patrol State if Player is outside Chase range for long enough (stopChasingTime)
            if(stopChasingTimeTimer >= stopChasingTime)
            {
                enemyState = EnemyState.ENEMY_PATROLLING;
                stopChasingTimeTimer = 0;
            }
            else
            {
                stopChasingTimeTimer += Time.deltaTime;
            }
        }

        EnemyAttackBehavior();
    }

    // Virtual function for Overriding by ChildScripts
    protected virtual void EnemyAttackBehavior()
    {
        print("Attacking");
    }

    // Enemy Knockback when attacked by Player Function
    public void PlayerHitEnemy(Vector2 playerPosition)
    {
        StopAllCoroutines();
        enemyIsHit = true;
        if (playerPosition.x > this.transform.position.x)
        {
            knockbackDirection = -knockbackForce.x;
        }
        else if (playerPosition.x < this.transform.position.x)
        {
            knockbackDirection = knockbackForce.x;
        }
        
        print($"enemyIsHit: {enemyIsHit}");
        StartCoroutine(DamageEnemy());
    }
    
    IEnumerator DamageEnemy()
    {
        enemyRigidbody2D.velocity = Vector2.zero;
        enemyRigidbody2D.velocity = new Vector2(knockbackDirection, knockbackForce.y);
        yield return new WaitForSecondsRealtime(0.3f);
        enemyRigidbody2D.velocity = Vector2.zero;
        enemyIsHit = false;
    }
    
    // Flipping Enemy Object Function
    private void EnemyFlip()
    {
        directionFloat *= -1;
        if (directionFloat > 0)
        {
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0, targetRotation.eulerAngles.z);
        }
        else if (directionFloat < 0)
        {
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 180, targetRotation.eulerAngles.z);
        }

        rotationTime = 0f;
        StartCoroutine(EnemyFlipSprite());
    }
    
    // Flipping Enemy Sprite Function
    IEnumerator EnemyFlipSprite()
    {
        while (rotationTime < 1f)
        {
            rotationTime += Time.deltaTime;
            this.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationTime);

            yield return null;
        }
        enemyFlipped = false;
    }

    // Enemy Checks for Player when collides with Player Object function
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerController.DamagePlayer(this.gameObject);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + Vector3.down * 2);
        //Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + transform.right * wallRaycastDistance);

        //Gizmos.DrawWireCube(enemyColliderTransform.position, playerDetectionRange);
        //Gizmos.DrawWireCube(enemyColliderTransform.position, new Vector2(playerDetectionRange.x * 2, playerDetectionRange.y));
    }

    protected virtual void AdjustCurrentHealth(int adj)
    {
        currentHealth += adj;

        if (currentHealth < 0)
        {
            currentHealth = 0;
            Destroy(this);
        }
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (maxHealth < 1)
        {
            maxHealth = 1;
        }
    }
}
