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
    private bool dashedInAir;
    #endregion

    #region Damaged Variables
    [Header("Damaged Variables")]
    [SerializeField] private float invulnerabilityPeriod;
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
        #region Initialise Jump Variables
        gravity = this.rigidbody2D.gravityScale;
        jumpVelocity = Mathf.Sqrt(Mathf.Pow(jumpHeight, 2) * gravity);
        #endregion
        #region Initialise Dash Variables
        dashIntervalTimer = dashInterval;
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
        UpdateRaycastOrigins();
        VerticalCollisionDetection();
        #endregion
        #region Reset Jump Time
        if(playerState != PlayerState.PLAYER_JUMPING && jumpTime > 0) { jumpTime = 0; }
        #endregion
        #region Reset Heal Input Buffer
        if(!inputHeal && inputBuffer > 0) { inputBuffer = 0; }
        #endregion
        #region Reset Dash Interval
        if(dashIntervalTimer < dashInterval) { dashIntervalTimer += Time.deltaTime; }
        #endregion

        PlayerSwitchState();
        print($"playerState: {playerState}");
        //print($"inputBuffer: {inputBuffer}");
        //print($"isGrounded: {isGrounded}");
        //print($"isHitCeiling: {isHitCeiling}");
        //print($"currentHealth: {currentHealth}");
        //print($"currentSpirit: {currentSpirit}");
        //print($"dashedInAir: {dashedInAir}");
    }
    
    private void PlayerSwitchState()
    {
        switch (playerState)
        {
            case PlayerState.PLAYER_IDLE:
                animator.SetBool("isRunning", false);
                PlayerFlip();
                if(!isGrounded) { playerState = PlayerState.PLAYER_FALLING; }
                else if(input.x != 0) { playerState = PlayerState.PLAYER_RUNNING; }
                else if(inputJumpPress && isGrounded) { playerState = PlayerState.PLAYER_JUMPING; }
                else if(inputHeal && currentSpirit >= spiritDrainToHeal)
                {
                    if(inputBuffer >= inputBufferTime)
                    {
                        inputBuffer = 0;
                        playerState = PlayerState.PLAYER_HEALING;
                    }
                    else { inputBuffer += Time.deltaTime; }
                }
                else if(inputDash && dashIntervalTimer >= dashInterval)
                {
                    dashIntervalTimer = 0f;
                    playerState = PlayerState.PLAYER_DASHING;
                }
                break;
            case PlayerState.PLAYER_RUNNING:
                animator.SetBool("isRunning", true);
                PlayerFlip();
                PlayerMove();
                if(!isGrounded) { playerState = PlayerState.PLAYER_FALLING; }
                else if(input.x == 0) { playerState = PlayerState.PLAYER_IDLE; }
                else if(inputJumpPress && isGrounded) { playerState = PlayerState.PLAYER_JUMPING; }
                else if(inputHeal && currentSpirit >= spiritDrainToHeal)
                {
                    if(inputBuffer >= inputBufferTime)
                    {
                        inputBuffer = 0;
                        playerState = PlayerState.PLAYER_HEALING;
                    }
                    else { inputBuffer += Time.deltaTime; }
                }
                else if(inputDash && dashIntervalTimer >= dashInterval)
                {
                    dashIntervalTimer = 0f;
                    playerState = PlayerState.PLAYER_DASHING;
                }
                break;
            case PlayerState.PLAYER_JUMPING:
                animator.SetBool("isRunning", false);
                PlayerFlip();
                PlayerMove();
                PlayerJump();
                if(!inputJump || isHitCeiling) { playerState = PlayerState.PLAYER_FALLING; }
                else if(inputDash && dashIntervalTimer >= dashInterval && !dashedInAir)
                {
                    dashIntervalTimer = 0f;
                    dashedInAir = true;
                    playerState = PlayerState.PLAYER_DASHING;
                }
                break;
            case PlayerState.PLAYER_FALLING:
                animator.SetBool("isRunning", false);
                PlayerFlip();
                PlayerMove();
                if(isGrounded)
                {
                    dashedInAir = false;
                    playerState = PlayerState.PLAYER_IDLE;
                } // Resets player state to idle after landing from a fall
                if(inputDash && dashIntervalTimer >= dashInterval && !dashedInAir)
                {
                    dashIntervalTimer = 0f;
                    dashedInAir = true;
                    playerState = PlayerState.PLAYER_DASHING;
                }
                break;
            case PlayerState.PLAYER_HEALING:
                animator.SetBool("isRunning", false);
                this.rigidbody2D.velocity = Vector2.zero;
                PlayerHeal();
                if(!isGrounded) { playerState = PlayerState.PLAYER_FALLING; }
                else if(!inputHeal || currentSpirit <= 0 || finishedHeal && currentSpirit < spiritDrainToHeal || finishedHeal && currentHealth >= maximumHealth)
                {
                    spiritDrain = 0;
                    currentSpirit = Mathf.RoundToInt(currentSpirit);
                    playerState = PlayerState.PLAYER_IDLE;
                }
                else if(inputHeal && currentSpirit >= spiritDrainToHeal && finishedHeal) { playerState = PlayerState.PLAYER_HEALING; }
                break;
            case PlayerState.PLAYER_DASHING:
                animator.SetBool("isRunning", false);
                PlayerDash();
                break;
            case PlayerState.PLAYER_HIT:
                animator.SetBool("isRunning", false);
                StartCoroutine(PlayerKnockback());
                break;
        }
    }

    private void PlayerFlip()
    {
        if(input.x > 0 && !facingRight || input.x < 0 && facingRight) { FlipCharacter(); }
    } // Checks direction of player

    private void PlayerMove()
    {
        this.rigidbody2D.velocity = new Vector2(input.x * moveSpeed, this.rigidbody2D.velocity.y);
    } // Makes player move

    private void PlayerJump()
    {
        if(jumpTime >= timeToJumpApex) { playerState = PlayerState.PLAYER_FALLING; }
        else
        {
            jumpTime += Time.deltaTime;
            this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, jumpVelocity);
        }
    } // Makes player jump

    private void PlayerHeal()
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
    } // Makes player heal

    private void PlayerDash()
    {
        if(dashTimeTimer >= dashTime)
        {
            dashTimeTimer = 0;
            this.rigidbody2D.velocity = Vector2.zero;
            playerState = (dashedInAir) ? PlayerState.PLAYER_FALLING : PlayerState.PLAYER_IDLE;
        }
        else
        {
            dashTimeTimer += Time.deltaTime;
            this.rigidbody2D.velocity = (facingRight) ? Vector2.right * dashSpeed : Vector2.left * dashSpeed;
        }
    } // Makes player dash

    private void PlayerMeleeAttack()
    {

    } // Makes player use melee attack

    private void PlayerSpiritAttack()
    {

    } // Makes player use spirit attack 
    
    public void DamagePlayer(GameObject enemyObject)
    {
        if (enemyObject.transform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; }
        else { knockbackDirection.x = knockbackForce.x; }
        if(playerState != PlayerState.PLAYER_HIT)
        {
            playerState = PlayerState.PLAYER_HIT;
            if (currentHealth > 0) { currentHealth -= 1; }
            else { /*restart from checkpoint*/ }
        }
            
    } // Gets called when player is hit by enemy

    IEnumerator PlayerKnockback()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        if (knockbackDirection.x < 0 && !facingRight || knockbackDirection.x > 0 && facingRight) { FlipCharacter(); }
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(knockbackDirection, ForceMode2D.Impulse);
        
        yield return new WaitForSecondsRealtime(0.2f);
        this.rigidbody2D.velocity = Vector2.zero;
        playerState = PlayerState.PLAYER_FALLING;
        //yield return new WaitForSecondsRealtime(invulnerabilityPeriod);
    }
}
