using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingScript : GameCharacter
{
    #region Input Variables
    private Vector2 input;
    private bool inputJumpPress;
    private bool inputJump;
    private bool inputHeal;
    private bool inputDash;
    private bool inputMeleeAttack;
    private bool inputSpiritAttack;
    private bool inputInteract;
    #endregion
    #region Spirit Variables
    [Header("Spirit Variables")]
    [SerializeField] private int maximumSpirit = 9;
    private float currentSpirit;
    #endregion
    #region Ground & Ceiling Check Variables
    private Transform groundCheck;
    private Transform ceilingCheck;
    private Vector2 checkSize;
    private bool isGrounded;
    private bool isHitCeiling;
    #endregion
    #region Jump Variables
    [Header("Jump Variables")]
    [SerializeField] private float jumpHeight = 4.5f;
    [SerializeField] private float timeToJumpApex = 0.35f;
    private float jumpVelocity;
    private float jumpTime;
    private float gravity;
    #endregion
    #region Heal Variables
    [Header("Heal Variables")]
    [SerializeField] private float inputBufferTime = 0.2f;
    [SerializeField] private int spiritDrainToHeal = 3;
    [SerializeField] private float healTime = 1.5f;
    private float inputBuffer;
    private float spiritDrain;
    private float healTimeTimer;
    private bool finishedHeal;
    #endregion
    #region Dash Variables
    [Header("Dash Variables")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashInterval;
    private float dashTimeTimer;
    private float dashIntervalTimer;
    #endregion

    private enum PlayerState
    {
        PLAYER_IDLE,
        PLAYER_RUNNING,
        PLAYER_JUMPING,
        PLAYER_FALLING,
        PLAYER_HEALING,
        PLAYER_DASHING,
        PLAYER_HIT
    };
    private PlayerState playerState;

    protected override void Initialise()
    {
        base.Initialise();
        #region Initialise Health Variables
        currentHealth = maximumHealth;
        #endregion
        #region Initialise Spirit Variables
        currentSpirit = maximumSpirit;
        #endregion
        #region Initialise Ground & Ceiling Check Variables
        groundCheck = transform.Find("SingCollider/GroundCheck").transform;
        ceilingCheck = transform.Find("SingCollider/CeilingCheck").transform;
        checkSize = new Vector2(capsuleCollider2D.size.x * 0.95f, 0.1f);
        #endregion
        #region Initialise Jump Variables
        gravity = rigidbody2D.gravityScale;
        jumpVelocity = Mathf.Sqrt(Mathf.Pow(jumpHeight, 2) * gravity);
        #endregion

    }

    private void Update()
    {
        #region Check Inputs
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // Input for directions
        inputJumpPress = Input.GetButtonDown("JumpButton"); // Input for jump press
        inputJump = Input.GetButton("JumpButton"); // Input for jump
        inputHeal = Input.GetButton("HealButton"); // Input for heal
        inputDash = Input.GetButtonDown("DashButton"); // Input for dash
        inputMeleeAttack = Input.GetButtonDown("MeleeAttackButton"); // Input for melee attack
        inputSpiritAttack = Input.GetButtonDown("SpiritAttackButton"); // Input for spirit attack
        inputInteract = Input.GetButtonDown("InteractButton"); // Input for interact
        #endregion
        #region Check Ground & Ceiling
        isGrounded = Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, terrainLayer);
        isHitCeiling = Physics2D.OverlapBox(ceilingCheck.position, checkSize, 0f, terrainLayer);
        #endregion

        

        print($"playerState: {playerState}");
        print($"inputBuffer: {inputBuffer}");
        PlayerSwitchState();
    }
    
    private void PlayerSwitchState()
    {
        switch(playerState)
        {
            case PlayerState.PLAYER_IDLE:
                animator.SetBool("isRunning", false);
                PlayerFlip();
                if(!isGrounded)
                    playerState = PlayerState.PLAYER_FALLING;
                if(input.x != 0)
                    playerState = PlayerState.PLAYER_RUNNING;
                else if(inputJumpPress && isGrounded)
                    playerState = PlayerState.PLAYER_JUMPING;
                else if(inputHeal && currentSpirit >= spiritDrainToHeal)
                    if(inputBuffer >= inputBufferTime)
                    {
                        inputBuffer = 0;
                        playerState = PlayerState.PLAYER_HEALING;
                    }
                    else
                    {
                        inputBuffer += Time.deltaTime;
                    }
                else if(!inputHeal && inputBuffer > 0)
                    inputBuffer = 0;
                else if(inputDash)
                    playerState = PlayerState.PLAYER_DASHING;
                break;
            case PlayerState.PLAYER_RUNNING:
                animator.SetBool("isRunning", true);
                PlayerFlip();
                PlayerMove();
                if(!isGrounded)
                    playerState = PlayerState.PLAYER_FALLING;
                if(input.x == 0)
                    playerState = PlayerState.PLAYER_IDLE;
                else if(inputJumpPress && isGrounded)
                    playerState = PlayerState.PLAYER_JUMPING;
                else if(inputHeal && currentSpirit >= spiritDrainToHeal)
                    if (inputBuffer >= inputBufferTime)
                    {
                        inputBuffer = 0;
                        playerState = PlayerState.PLAYER_HEALING;
                    }
                    else
                    {
                        inputBuffer += Time.deltaTime;
                    }
                else if(!inputHeal && inputBuffer > 0)
                    inputBuffer = 0;
                else if(inputDash)
                    playerState = PlayerState.PLAYER_DASHING;
                break;
            case PlayerState.PLAYER_JUMPING:
                animator.SetBool("isRunning", false);
                PlayerFlip();
                PlayerMove();
                PlayerJump();
                if(!inputJump)
                {
                    jumpTime = 0;
                    playerState = PlayerState.PLAYER_FALLING;
                }
                break;
            case PlayerState.PLAYER_FALLING:
                PlayerFlip();
                PlayerMove();
                PlayerFall();
                break;
            case PlayerState.PLAYER_HEALING:
                animator.SetBool("isRunning", false);
                rigidbody2D.velocity = Vector2.zero;
                PlayerHeal();
                if(!isGrounded)
                    playerState = PlayerState.PLAYER_FALLING;
                if (!inputHeal || currentSpirit <= 0 || finishedHeal && currentSpirit < spiritDrainToHeal || finishedHeal && currentHealth >= maximumHealth)
                {
                    spiritDrain = 0;
                    currentSpirit = Mathf.RoundToInt(currentSpirit);
                    playerState = PlayerState.PLAYER_IDLE;
                }
                else if (inputHeal && currentSpirit >= spiritDrainToHeal && finishedHeal)
                    playerState = PlayerState.PLAYER_HEALING;
                break;
            case PlayerState.PLAYER_DASHING:
                PlayerDash();
                playerState = PlayerState.PLAYER_IDLE;
                break;
        }
    }

    private void PlayerFlip() // Checks direction of player
    {
        if (input.x > 0 && !facingRight || input.x < 0 && facingRight)
            FlipCharacter();
    }

    private void PlayerMove() // Makes player move
    {
        rigidbody2D.velocity = new Vector2(input.x * moveSpeed, rigidbody2D.velocity.y);
    }

    private void PlayerJump() // Makes player jump
    {
        if(jumpTime >= timeToJumpApex)
        {
            jumpTime = 0;
            playerState = PlayerState.PLAYER_FALLING;
        }
        else
        {
            jumpTime += Time.deltaTime;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpVelocity);
        }
    }

    private void PlayerFall() // Resets jump values when player lands after jumping
    {
        if(isGrounded)
            playerState = PlayerState.PLAYER_IDLE;
    }

    private void PlayerHeal() // Makes player heal
    {
        if(spiritDrain >= spiritDrainToHeal)
        {
            spiritDrain = 0;
            currentSpirit = Mathf.RoundToInt(currentSpirit);
            if(currentHealth < maximumHealth)
            {
                currentHealth += 1;
                print($"currentHealth: {currentHealth}");
            }
            finishedHeal = true;
        }
        else
        {
            spiritDrain += Time.deltaTime * (spiritDrainToHeal / healTime);
            currentSpirit -= Time.deltaTime * (spiritDrainToHeal / healTime);
            finishedHeal = false;
        }
    }

    private void PlayerDash()
    {

    }

    private void PlayerMeleeAttack()
    {

    }

    private void PlayerSpiritAttack()
    {

    }

    private void PlayerHit()
    {

    }

    public void DamagePlayer(GameObject enemyObject)
    {
        if(playerState != PlayerState.PLAYER_HIT)
            playerState = PlayerState.PLAYER_HIT;
    }
}
