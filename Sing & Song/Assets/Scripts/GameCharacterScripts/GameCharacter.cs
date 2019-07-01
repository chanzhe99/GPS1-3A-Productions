using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class GameCharacter : MonoBehaviour
{
    #region Component Variables
    protected Transform colliderTransform;
    protected CapsuleCollider2D capsuleCollider2D;
    protected Rigidbody2D rigidbody2D;
    protected Animator animator;
    #endregion

    #region Layer Variables
    protected LayerMask playerLayer;
    protected LayerMask enemyLayer;
    protected LayerMask terrainLayer;
    #endregion
    #region Ground & Ceiling Raycast Variables
    [Header("Ground & Ceiling Raycast Variables")]
    [SerializeField] private int rayCount = 5;
    private const float colliderSkinWidth = 0.015f;
    private float raySpacing;
    private struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }
    private RaycastOrigins raycastOrigins;
    private Bounds colliderBounds;
    private RaycastHit2D raycastHit2D;
    private float rayLength;
    protected bool isGrounded;
    protected bool isHitCeiling;
    #endregion
    #region Sprite Flip Variables
    [SerializeField] protected bool spawnFacingRight;
    protected bool facingRight;
    private Quaternion targetRotation;
    private float rotationTime;
    #endregion


    [SerializeField] protected int maximumHealth;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected Vector2 attackRange;
    [SerializeField] protected float moveSpeed;
    protected int currentHealth;

    [SerializeField] protected Vector2 knockbackForce;
    protected Vector2 knockbackDirection;
    protected bool isHit;

    
    private void Start()
    {
        Initialise();
    }

    protected virtual void Initialise()
    {

        #region Initialise Component Variables
        colliderTransform = GetComponentInChildren<Transform>();
        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        #endregion
        #region Initialise Raycast Variables
        CalculateRaySpacing();
        #endregion
        #region Initialise Layer Variables
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");
        terrainLayer = LayerMask.GetMask("Terrain");
        #endregion
        #region Initialise Health Variables
        currentHealth = maximumHealth;
        #endregion

        DetermineStartingDirection(); 

        knockbackDirection = knockbackForce;
        currentHealth = maximumHealth;
    }

    protected void DetermineStartingDirection()
    {
        if(spawnFacingRight)
        {
            this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            facingRight = true;
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            facingRight = false;
        }
        targetRotation = this.transform.rotation;
    } // Determines whether the character spawns facing left or right

    protected void UpdateRaycastOrigins()
    {
        colliderBounds = capsuleCollider2D.bounds;
        colliderBounds.Expand(colliderSkinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(colliderBounds.min.x, colliderBounds.min.y);
        raycastOrigins.bottomRight = new Vector2(colliderBounds.max.x, colliderBounds.min.y);
        raycastOrigins.topLeft = new Vector2(colliderBounds.min.x, colliderBounds.max.y);
        raycastOrigins.topRight = new Vector2(colliderBounds.max.x, colliderBounds.max.y);
    } // Updates position of raycastOrigins

    protected void CalculateRaySpacing()
    {
        colliderBounds = capsuleCollider2D.bounds;
        colliderBounds.Expand(colliderSkinWidth * -2);

        rayCount = Mathf.Clamp(rayCount, 2, int.MaxValue);

        raySpacing = colliderBounds.size.x / (rayCount - 1);
    }  // Calculates the spacing between each raycast depending on amount

    protected void VerticalCollisionDetection()
    {
        rayLength = capsuleCollider2D.size.y * 0.1f + colliderSkinWidth;
        for(int i = 0; i < rayCount; i++)
        {
            raycastHit2D = Physics2D.Raycast(raycastOrigins.bottomLeft + Vector2.right * raySpacing * i, Vector2.down, rayLength, terrainLayer);
            if(raycastHit2D)
            {
                isGrounded = true;
                break;
            }
            else
            {
                isGrounded = false;
            }
            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * raySpacing * i, Vector2.down * rayLength, Color.red);
        }

        for (int i = 0; i < rayCount; i++)
        {
            raycastHit2D = Physics2D.Raycast(raycastOrigins.topLeft + Vector2.right * raySpacing * i, Vector2.up, rayLength, terrainLayer);
            if (raycastHit2D)
            {
                isHitCeiling = true;
                break;
            }
            else
            {
                isHitCeiling = false;
            }
            Debug.DrawRay(raycastOrigins.topLeft + Vector2.right * raySpacing * i, Vector2.up * rayLength, Color.red);
        }
    } // Toggles isGrounded & isHitceiling

    protected void FlipCharacter()
    {
        facingRight = !facingRight;
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + 180f, targetRotation.eulerAngles.z);
        rotationTime = 0f;
        StartCoroutine(FlipCharacterSprite());
    } // Sets character's rotation

    private IEnumerator FlipCharacterSprite()
    {
        while(rotationTime < 1f)
        {
            rotationTime += Time.deltaTime;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationTime);

            yield return null;
        }
    } // Makes character's sprite flip with flip effect
}
