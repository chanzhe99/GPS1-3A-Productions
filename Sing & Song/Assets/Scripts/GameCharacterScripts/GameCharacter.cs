using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class GameCharacter : MonoBehaviour
{
    #region Component Variables
    protected Transform colliderTransform;
    protected Rigidbody2D rigidbody2D;
    protected CapsuleCollider2D capsuleCollider2D;
    protected Animator animator;
    #endregion

    #region Layer Variables
    protected LayerMask playerLayer;
    protected LayerMask enemyLayer;
    protected LayerMask terrainLayer;
    #endregion

    #region Sprite Flip Variables
    [SerializeField] protected bool spawnFacingRight;
    protected bool facingRight;
    private Quaternion targetRotation;
    private float rotationTime;
    #endregion


    [SerializeField] protected int maximumHealth;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float moveSpeed;
    protected int currentHealth;

    [SerializeField] protected Vector2 knockbackForce;
    protected Vector2 knockbackDirection;
    protected bool isHit;

    
    

    private void Start()
    {
        #region Initialise Component Variables
        colliderTransform = GetComponentInChildren<Transform>();
        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        #endregion

        #region Initialise Layer Variables
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");
        terrainLayer = LayerMask.GetMask("Terrain");
        #endregion

        DetermineStartingDirection(); // Determines whether the character spawns facing left or right

        knockbackDirection = knockbackForce;
        currentHealth = maximumHealth;
        Initialise();
    }

    protected virtual void Initialise()
    {
        
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
    }

    protected void FlipCharacter()
    {
        facingRight = !facingRight;
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + 180f, targetRotation.eulerAngles.z);
        rotationTime = 0f;
        StartCoroutine(FlipCharacterSprite());
    }

    private IEnumerator FlipCharacterSprite()
    {
        while(rotationTime < 1f)
        {
            rotationTime += Time.deltaTime;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationTime);

            yield return null;
        }
    }
}
