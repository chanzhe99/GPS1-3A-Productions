using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region PlayerVariables
    // TEMP CAMERA FOR KNOCKBACK EFFECT
    private GameObject cameraObject;
    private Animator animator;
    private bool inCombat;
    [SerializeField] private Vector2 combatRange;

    #region PlayerComponenet+OtherObjectVariables
    // Player Component + Other Objects Variables
    private CapsuleCollider2D playerCapsuleCollider2D;
    private Rigidbody2D playerRigidBody2D;
    #endregion

    #region Ground&CeilingCheckVariables
    // Ground & Ceiling Check Variables
    private Transform groundCheck;
    private Transform ceilingCheck;
    private bool isGrounded;
    private bool isHitCeiling;
    private Vector2 checkSize;
    private LayerMask terrainLayer;
    #endregion

    #region PlayerHealthVariables
    [Header("Player Health Variables")]// Health Variables
    [SerializeField] private Image[] HP;
    private int maxHP;
    private int currentHP;
    #endregion

    #region PlayerSpiritVariables
    [Header("Player Spirit Variables")]// Spirit Variables
    [SerializeField] private Image SPBar;
    [SerializeField] private float SPBarSpeed;
    private Vector2 maxSPPosition;
    private Vector2 minSPPosition;
    private Vector2 newSPPosition;
    private bool SPBarMovingRight;
    private int maxSP;
    private float currentSP;
    #endregion

    #region PlayerMovementVariables
    [Header("Player Movement Variables")]// Movement Variables
    [SerializeField] private float moveSpeed;
    private float xInput;
    private float yInput;
    private bool facingRight;
    private bool canMove;
    #endregion

    #region PlayerJumpVariables
    [Header("Player Jump Variables")]// Jump Variables
    [SerializeField] private float jumpForceDefault;
    [SerializeField] private float jumpTime;
    private float jumpForce;
    private float jumpTimeTimer;
    private float gravityScaleDefault;
    private bool isJumping;
    private bool isInAir;
    private bool airJumped;
    private bool canJump;
    #endregion

    #region PlayerDashVariables
    [Header("Player Dash Variables")]// Dash Variables
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashInterval;
    private float dashTimeTimer;
    private float dashIntervalTimer;
    private bool dashingRight;
    private bool isDashing;
    private bool airDashed;
    private bool canDash;
    #endregion

    #region PlayerAttackVariables
    [Header("Player Attack Variables")]// Attack Variables
    [SerializeField] private Transform playerAttackTransform;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackInterval;
    private float attackIntervalTimer;
    private LayerMask enemyLayer;
    private bool canAttack;
    
    #endregion

    #region PlayerHealVariables
    [Header("Player Heal Variables")]// Heal Variables
    [SerializeField] private float healChargeSP;
    private float healChargeSPDrain;
    private float healBuffer;
    private bool canHeal;
    private bool healCharging;
    private bool healed;
    #endregion

    #region PlayerDamagedVariables
    [Header("Player Damaged Variables")]// Damaged Variables
    [SerializeField] private float invulnerabilityPeriod;
    [SerializeField] private Vector2 knockbackForce;
    private float knockbackDirection;
    private bool isHit;
    #endregion

    #region SpriteFlipVariables
    // Sprite Flip Variables
    private Quaternion targetRotation;
    float rotationTime;
    #endregion

    #region SongControlVariables
    [Header("Song Control Variables")]// Node Update Variables (For Song Follow)
    [SerializeField] private GameObject node;
    [SerializeField] private GameObject song;
    [SerializeField] private Vector3 nodeOffsetDefault;
    [SerializeField] private float nodeUpdateInterval;
    private Vector3 nodeOffset;
    private float nodeUpdateIntervalTimer;
    #endregion

    #endregion

    // Set variables when awake
    private void Start()
    {
        animator = GetComponent<Animator>();
        cameraObject = GameObject.Find("MainCamera"); //TEMP CODE(GET RID LATER)
        playerCapsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        playerRigidBody2D = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("SingCollider/GroundCheck").transform;
        ceilingCheck = transform.Find("SingCollider/CeilingCheck").transform;
        checkSize = new Vector2(playerCapsuleCollider2D.size.x * 0.95f, 0.1f);
        gravityScaleDefault = playerRigidBody2D.gravityScale;
        terrainLayer = LayerMask.GetMask("Terrain");
        enemyLayer = LayerMask.GetMask("Enemy");
        maxHP = 5;
        currentHP = maxHP;
        maxSP = 100;
        currentSP = maxSP;
        maxSPPosition = new Vector2(SPBar.rectTransform.anchoredPosition.x, -65);
        minSPPosition = new Vector2(SPBar.rectTransform.anchoredPosition.x, -200);
        SPBarMovingRight = true;
        facingRight = true;
        canMove = true;
        canHeal = true;
    }

    // Updates every frame
    private void Update()
    {
        // Call Actions Function

        PlayerActions();
        //PlayerMove();
        //PlayerJump();
        //PlayerDash();
        //PlayerAttack();
        //PlayerHeal();
        NodeUpdate();
        PlayerCollisionManager();
        //print($"healBuffer: {healBuffer}");
        
        //PlayerFlip(); //(FIX BEFORE IMPLEMENT)
        //print($"healChargeSPr: {healChargeSPDrain}");
    }

    private void PlayerActions()
    {
        PlayerUnsheathe();
        PlayerHealth();
        PlayerSpirit();
        PlayerMove();
        PlayerJump();
        PlayerDash();
        PlayerAttack();
        PlayerHeal();
    }

    private void PlayerUnsheathe()
    {
        inCombat = Physics2D.OverlapBox(transform.position, combatRange, 0f, enemyLayer);

        if(inCombat)
        {
            animator.SetBool("inCombat", true);
        }
        else
        {
            animator.SetBool("inCombat", false);
        }
    }

    // HP Function
    private void PlayerHealth()
    {
        for (int i = 0; i < HP.Length; i++)
        {
            if(i < currentHP)
            {
                HP[i].enabled = true;
            }
            else
            {
                HP[i].enabled = false;
            }
        }

        if(currentHP >= maxHP)
        {
            currentHP = maxHP;
        }

        if(currentHP <= 0)
        {
            print("U DEAD NIGGA");
        }
    }

    // SP Function
    private void PlayerSpirit()
    {
        newSPPosition = Vector2.Lerp(minSPPosition, maxSPPosition, currentSP * 0.01f);
        SPBar.rectTransform.anchoredPosition = Vector2.Lerp(SPBar.rectTransform.anchoredPosition, newSPPosition, Time.deltaTime);

        if (SPBar.rectTransform.anchoredPosition.x >= 400)
        {
            SPBarMovingRight = false;
        }
        else if(SPBar.rectTransform.anchoredPosition.x <= -400)
        {
            SPBarMovingRight = true;
        }
        
        if(currentSP >= maxSP)
        {
            currentSP = maxSP;
        }
        if(currentSP <= 0)
        {
            currentSP = 0;
        }
    }

    // Movement Function (// (FIXED) canMove makes movement velocity code not run, thus allowing knockback to use velocity //TEMPORARILY USING "transform.Translate" BECAUSE "rigidbody2d.velocity" LOCKS X MOVEMENT) (transform.Translate causes sprite to vibrate when pushing against terrain)
    private void PlayerMove()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if(xInput != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if(canMove)
        {
            playerRigidBody2D.velocity = new Vector2(xInput * moveSpeed, playerRigidBody2D.velocity.y);
            //transform.Translate(xMoveInput * Time.deltaTime, 0f, 0f);

            // Check Direction
            if(xInput > 0 && !facingRight || xInput < 0 && facingRight)
            {
                PlayerFlip();
            }
        }
    }

    // Jump Function (Somewhat Okay)
    private void PlayerJump()
    {
        // Check isGrounded & isHitCeiling
        isGrounded = true;//Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, terrainLayer);
        isHitCeiling = Physics2D.OverlapBox(ceilingCheck.position, checkSize, 0f, terrainLayer);

        // Set isGrounded Variable Defaults
        if(isGrounded)
        {
            isInAir = false;
            airJumped = false;
            airDashed = false;
            playerRigidBody2D.gravityScale = gravityScaleDefault;
            jumpForce = jumpForceDefault;
        }

        // Set isJumping off when hit ceiling
        if(isHitCeiling)
        {
            if(isJumping)
            {
                isJumping = false;
            }
        }

        // Set isJumping true when button is pressed
        // Original Syntax: if(isGrounded && Input.GetButtonDown("Jump"))
        if(isGrounded && Input.GetButtonDown("JumpButton")) // Ground Jump
        {
            isJumping = true;
            isInAir = true;
            jumpTimeTimer = 0;
        }
        else if(isInAir && !airJumped && Input.GetButtonDown("JumpButton")) // Air Jump
        {
            isJumping = true;
            airJumped = true;
            jumpForce = jumpForceDefault;
            jumpTimeTimer = 0;
        }

        // Set isJumping false when button is released
        if (Input.GetButtonUp("JumpButton"))
        {
            isJumping = false;
        }

        // Set jump height based on button hold
        if (isJumping && Input.GetButton("JumpButton"))
        {
            if(jumpTimeTimer >= jumpTime)
            {
                isJumping = false;
            }
            else
            {
                jumpForce += Time.deltaTime * 5;
                playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, jumpForce);
                jumpTimeTimer += Time.deltaTime;
            }
        }

        // Increase gravity when aerial
        if(!isGrounded)
        {
            isInAir = true;
            if(playerRigidBody2D.gravityScale < gravityScaleDefault * 1.5)
            {
                playerRigidBody2D.gravityScale += Time.deltaTime * 5;
            }
        }
    }

    // Dash Function (//FIXED //FIX PROBLEM WHERE PLAYER CAN CHANGE DIRECTIONS DURING DASH)
    private void PlayerDash()
    {
        // Check Dash Interval
        if(dashIntervalTimer >= dashInterval)
        {
            if(canDash && !airDashed && Input.GetButtonDown("DashButton"))
            {
                // Check if Dash in Air
                if(isGrounded)
                {
                    airDashed = false;
                }
                else
                {
                    airDashed = true;
                }

                // Set isDashing variables
                dashIntervalTimer = 0;
                isDashing = true;
                dashTimeTimer = 0;
            }
        }
        else
        {
            dashIntervalTimer += Time.deltaTime;
        }

        // Dashing code
        if(!isDashing)
        {
            // Only change Dash direction while not Dashing
            dashingRight = facingRight;
        }
        else
        {

            // Check Dash Time
            if (dashTimeTimer >= dashTime)
            {
                isDashing = false;
            }
            else
            {
                dashTimeTimer += Time.deltaTime;

                // Dash Direction
                if(dashingRight)
                {
                    playerRigidBody2D.velocity = Vector2.right * dashSpeed;
                    //transform.Translate(dashSpeed * Time.deltaTime, 0f, 0f);
                }
                else
                {
                    playerRigidBody2D.velocity = Vector2.left * dashSpeed;
                    //transform.Translate(-dashSpeed * Time.deltaTime, 0f, 0f);
                }
            }
        }
    }

    // Attack Function (Works but Unfinished)
    private void PlayerAttack()
    {
        // Check Attack Input + Attack
        if(attackIntervalTimer >= attackInterval)
        {
            // Check Attack Direction
            if(canAttack)
            {
                if (!isGrounded)
                {
                    if (yInput == 0)
                    {
                        playerAttackTransform.localPosition = Vector2.right * 1.5f;
                    }
                    else
                    {
                        playerAttackTransform.localPosition = Vector2.up * yInput * 1.5f;
                    }
                }
                else
                {
                    if (yInput > 0)
                    {
                        playerAttackTransform.localPosition = Vector2.up * 1.5f;
                    }
                    else
                    {
                        playerAttackTransform.localPosition = Vector2.right * 1.5f;
                    }
                }

                if (Input.GetButtonDown("AttackButton"))
                {
                    animator.SetTrigger("isAttack");
                    Collider2D[] enemyToDamage = Physics2D.OverlapCircleAll(playerAttackTransform.position, attackRange, enemyLayer);

                    for (int i = 0; i < enemyToDamage.Length; i++)
                    {
                        if(enemyToDamage[i].GetComponentInParent<FlyingLemurAI>() != null)
                        {
                            enemyToDamage[i].GetComponentInParent<FlyingLemurAI>().DamageEnemy();
                        }
                        if (enemyToDamage[i].GetComponentInParent<WildDogAI>() != null)
                        {
                            enemyToDamage[i].GetComponentInParent<WildDogAI>().DamageEnemy();
                        }
                            
                        //enemyToDamage[i].GetComponentInParent<PangolinAI>().DamageEnemy();
                        //enemyToDamage[i].GetComponentInParent<CrocodileAI>().DamageEnemy();

                        if (currentSP < maxSP)
                        {
                            currentSP += 10;
                            if (SPBarMovingRight)
                            {
                                SPBar.rectTransform.Translate(SPBarSpeed, 0f, 0f);
                            }
                            else
                            {
                                SPBar.rectTransform.Translate(-SPBarSpeed, 0f, 0f);
                            }
                        }
                    }
                    attackIntervalTimer = 0;
                }
            }
        }
        else
        {
            attackIntervalTimer += Time.deltaTime;
        }
    }

    // Heal Function (Works but Unfinished) // MAKE ALL ACTIONS DISABLED DURING HEAL // STILL CAN HEAL WHEN NOT ENOUGH SP // if below minimum amount, cant enter heal state at all
    private void PlayerHeal() // HEALING "WORKS"
    {
        // Can heal only when Grounded & have at least 1/3 SP
        canHeal = isGrounded;

        // Check if HealButton is held down
        if(canHeal && !isHit)
        {
            if(Input.GetButton("HealButton") && currentSP >= 50) //TEMP FIX FOR HEALING WITHOUT ENOUGH
            {
                healCharging = true;
            }
            else if(Input.GetButtonUp("HealButton"))
            {
                healCharging = false;
            }

            // Check if Charging Heal
            if (healCharging)
            {
                playerRigidBody2D.velocity = Vector2.zero;
                canMove = false;
                canDash = false;
                canAttack = false;
                
                //if (healBuffer >= 0.5f)
                //{
                    if (healChargeSPDrain >= healChargeSP) // IMPLEMENT A HEAL SP DRAIN FLOAT
                    {
                        healChargeSPDrain = 0;
                        if (currentHP < maxHP)
                        {
                            currentHP += 1;
                            //healBuffer = 0;
                        }
                    }
                    else
                    {
                        healChargeSPDrain += Time.deltaTime * 25;
                        currentSP -= Time.deltaTime * 25;
                        if (SPBarMovingRight)
                        {
                            SPBar.rectTransform.Translate(SPBarSpeed * 0.01f, 0f, 0f);
                        }
                        else
                        {
                            SPBar.rectTransform.Translate(-SPBarSpeed * 0.01f, 0f, 0f);
                        }
                    } 
                //}
                //else
                //{
                //    healChargeSPDrain = 0;
                //    healBuffer += Time.deltaTime;
                //}
            }
            else
            {
                canMove = true;
                canDash = true;
                canAttack = true;
                healChargeSPDrain = 0;
            }
        }
    }

    // Hit Enemy Function
    public void DamagePlayer(GameObject enemyObject)
    {
        if(enemyObject.transform.position.x > this.transform.position.x)
        {
            knockbackDirection = -knockbackForce.x;
        }
        else
        {
            knockbackDirection = knockbackForce.x;
        }

        if(!isHit)
        {
            isHit = true;
            if(currentHP > 0)
            {
                currentHP -= 1;
            }
            else
            {
                //restart from checkpoint
            }
            StartCoroutine(PlayerKnockback());
        }
    }

    IEnumerator PlayerKnockback() // VERY JANK FIX FOR KNOCKBACK (MAKE THIS PROPER KNOCKBACK)
    {
        canMove = false;
        Time.timeScale = 0;
        cameraObject.GetComponent<Camera>().orthographicSize = 5;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        cameraObject.GetComponent<Camera>().orthographicSize = 6;
        if (knockbackDirection < 0 && !facingRight || knockbackDirection > 0 && facingRight)
        {
            PlayerFlip();
        }
        playerRigidBody2D.velocity = new Vector2(knockbackDirection, knockbackForce.y);
        yield return new WaitForSecondsRealtime(0.3f);
        playerRigidBody2D.velocity = Vector2.zero;
        canMove = true;
        yield return new WaitForSecondsRealtime(invulnerabilityPeriod);
        isHit = false;
    }

    // Collision Manager
    private void PlayerCollisionManager()
    {
        if (isHit)
        {
            // Invulnerable for a short period after getting hit
            Physics2D.IgnoreLayerCollision(8, 9, true);
        }
        else
        {
            if (!isDashing)
            {
                // Vulnerable to EnemyLayer while not Dashing
                Physics2D.IgnoreLayerCollision(8, 9, false);
            }
            else
            {
                // Invulnerable to EnemyLayer while Dashing
                Physics2D.IgnoreLayerCollision(8, 9, true);
            }
        }
    }

    // Player Sprite Flip Function
    private void PlayerFlip()
    {
        facingRight = !facingRight;

        if(facingRight)
        {
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0, targetRotation.eulerAngles.z);
        }
        else
        {
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 180, targetRotation.eulerAngles.z);
        }

        rotationTime = 0f;

        StartCoroutine(PlayerFlipSprite());
    }

    // Player Sprite Flip Animation
    IEnumerator PlayerFlipSprite()
    {
        while(rotationTime < 1f)
        {
            rotationTime += Time.deltaTime;
            this.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationTime);

            yield return null;
        }
    }

    // Node Update Function
    private void NodeUpdate()
    {
        if(facingRight)
        {
            nodeOffset = nodeOffsetDefault;
        }
        else
        {
            nodeOffset = -nodeOffsetDefault;
        }

        if(nodeUpdateIntervalTimer >= nodeUpdateInterval)
        {
            node.transform.position = transform.position + nodeOffset;
            nodeUpdateIntervalTimer = 0;
        }
        else
        {
            nodeUpdateIntervalTimer += Time.deltaTime;
        }

        song.transform.position = Vector3.MoveTowards(song.transform.position, node.transform.position, moveSpeed * Time.deltaTime);
    }
    
    // Draw groundCheck & ceilingCheck
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (playerAttackTransform != null)
        {
            Gizmos.DrawWireSphere(playerAttackTransform.position, attackRange);
        }
        if(groundCheck != null)
        {
            Gizmos.DrawWireCube(groundCheck.position, checkSize);
        }
        if(ceilingCheck != null)
        {
            Gizmos.DrawWireCube(ceilingCheck.position, checkSize);
        }

        Gizmos.DrawWireCube(transform.position, combatRange);
    }
}
