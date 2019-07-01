using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float maximumSpirit = 9f;
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
    [SerializeField] private float spiritDrainToHeal = 3f;
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
    #region Melee Attack Variables
    [Header("Melee Attack Variables")]
    [SerializeField] private Transform meleeAttackTransform;
    [SerializeField] private float meleeAttackInterval;
    private float meleeAttackIntervalTimer;
    private Collider2D[] enemiesHit;
    #endregion
    #region Damaged Variables
    [Header("Damaged Variables")]
    [SerializeField] private float hitImpactTime = 0.2f;
    [SerializeField] private float hitKnockbackTime = 0.3f;
    [SerializeField] private float invulnerabilityPeriod = 0.5f;
    private bool vulnerable;
    private bool ignoreEnemyCollision;
    #endregion
    #region UI Variables
    [Header("UI Variables")]
    [SerializeField] private Image[] healthCrystals;
    [SerializeField] private Image spiritWell;
    [SerializeField] private float spiritWellFlowSpeed;
    private Vector2 maximumSpiritWellPosition;
    private Vector2 minimumSpiritWellPosition;
    private Vector2 currentSpiritWellPosition;
    private float previousSpiritWellPosition;
    private float spiritWellAlpha;
    //private float spiritWellEdge;
    //private bool spiritWellFlowingRight;
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
        #region Initialise Melee Attack Variables
        meleeAttackIntervalTimer = meleeAttackInterval;
        #endregion
        #region Initialise Damaged Variables
        vulnerable = true;
        #endregion
        #region Initialise UI Variables
        minimumSpiritWellPosition = new Vector2(spiritWell.rectTransform.anchoredPosition.x, -200);
        maximumSpiritWellPosition = new Vector2(spiritWell.rectTransform.anchoredPosition.x, -65);
        previousSpiritWellPosition = currentSpiritWellPosition.y;
        spiritWellAlpha = spiritWell.color.a;
        //spiritWellEdge = 500f;
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
        #region Check Spirit
        if(currentSpirit > maximumSpirit) { currentSpirit = maximumSpirit; } // maybe move this to attack function
        if(currentSpirit < 0) { currentSpirit = 0; }
        if(playerState != PlayerState.PLAYER_HEALING && currentSpirit >= spiritDrainToHeal - 0.1f && currentSpirit < spiritDrainToHeal) { currentSpirit = Mathf.Round(currentSpirit); }
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
        #region Update Melee Attack Interval Timer & Position
        if(meleeAttackIntervalTimer < meleeAttackInterval) { meleeAttackIntervalTimer += Time.deltaTime; }
        if(playerState == PlayerState.PLAYER_IDLE || playerState == PlayerState.PLAYER_RUNNING)
        {
            meleeAttackTransform.localPosition = (input.y > 0) ? Vector2.up * 2f : Vector2.left;
            if(inputMeleeAttack && meleeAttackIntervalTimer >= meleeAttackInterval) { PlayerMeleeAttack(); }
        }
        else if(playerState == PlayerState.PLAYER_JUMPING || playerState == PlayerState.PLAYER_FALLING)
        {
            meleeAttackTransform.localPosition = (input.y != 0) ? Vector2.up * input.y * 2f : Vector2.left;
            if(inputMeleeAttack && meleeAttackIntervalTimer >= meleeAttackInterval) { PlayerMeleeAttack(); }
        }
        #endregion
        #region Set Enemy Layer Collision
        ignoreEnemyCollision = !vulnerable;
        Physics2D.IgnoreLayerCollision(8, 9, ignoreEnemyCollision);
        #endregion

        #region Update UI
        for (int i = 0; i < healthCrystals.Length; i++)
        {
            healthCrystals[i].enabled = (i < currentHealth) ? true : false;
        }
        spiritWellAlpha = (playerState != PlayerState.PLAYER_HEALING && currentSpirit < spiritDrainToHeal) ? 0.6f : 1f;
        spiritWell.color = new Color(1f, 1f, 1f, Mathf.Lerp(spiritWell.color.a, spiritWellAlpha, Time.deltaTime * 2));
        currentSpiritWellPosition = Vector2.Lerp(minimumSpiritWellPosition, maximumSpiritWellPosition, currentSpirit/maximumSpirit);
        spiritWell.rectTransform.anchoredPosition = Vector2.Lerp(spiritWell.rectTransform.anchoredPosition, currentSpiritWellPosition, 1f);

        if(previousSpiritWellPosition != currentSpiritWellPosition.y)
        {
            //spiritWell.rectTransform.Translate(spiritWellFlowSpeed * Time.deltaTime, 0f, 0f);
            previousSpiritWellPosition = currentSpiritWellPosition.y;
        }
        #endregion

        PlayerSwitchState();
        //print($"playerState: {playerState}");
        //print($"inputBuffer: {inputBuffer}");
        //print($"isGrounded: {isGrounded}");
        //print($"isHitCeiling: {isHitCeiling}");
        //print($"currentHealth: {currentHealth}");
        //print($"currentSpirit: {currentSpirit}");
        //print($"dashedInAir: {dashedInAir}");
        print($"enemiesHit: {enemiesHit}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackTransform.position, attackRange.x);
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
            vulnerable = true;
            playerState = (dashedInAir) ? PlayerState.PLAYER_FALLING : PlayerState.PLAYER_IDLE;
        }
        else
        {
            dashTimeTimer += Time.deltaTime;
            vulnerable = false;
            this.rigidbody2D.velocity = (facingRight) ? Vector2.right * dashSpeed : Vector2.left * dashSpeed;
        }
    } // Makes player dash

    private void PlayerMeleeAttack()
    {
        enemiesHit = Physics2D.OverlapCircleAll(meleeAttackTransform.position, attackRange.x, enemyLayer);
        if (enemiesHit.Length != 0 && meleeAttackTransform.localPosition.y < 0)
        {
            this.rigidbody2D.velocity = Vector2.zero;
            this.rigidbody2D.AddForce(new Vector2(0f, knockbackDirection.y * 1.5f), ForceMode2D.Impulse);
        }
        for (int i = 0; i < enemiesHit.Length; i++)
        {
            if(enemiesHit[i].GetComponentInParent<FlyingLemurAI>() != null) { enemiesHit[i].GetComponentInParent<FlyingLemurAI>().DamageEnemy(); }
            if(enemiesHit[i].GetComponentInParent<WildDogAI>() != null) { enemiesHit[i].GetComponentInParent<WildDogAI>().DamageEnemy(); }
            if(enemiesHit[i].GetComponentInParent<PangolinAI>() != null) { enemiesHit[i].GetComponentInParent<PangolinAI>().DamageEnemy(); }
            if(enemiesHit[i].GetComponentInParent<CrocodileAI>() != null) { enemiesHit[i].GetComponentInParent<CrocodileAI>().DamageEnemy(); }
            if(enemiesHit[i].GetComponentInParent<RhinoAI>() != null) { enemiesHit[i].GetComponentInParent<RhinoAI>().DamageEnemy(); }
            currentSpirit = (currentSpirit < maximumSpirit) ? currentSpirit += 1 : maximumSpirit;
        }
        //if()
    } // Makes player use melee attack

    private void PlayerSpiritAttack()
    {

    } // Makes player use spirit attack 
    
    public void DamagePlayer(GameObject enemyObject)
    {
        if (enemyObject.transform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; }
        else { knockbackDirection.x = knockbackForce.x; }
        if(vulnerable)
        {
            vulnerable = false;
            playerState = PlayerState.PLAYER_HIT;
            StartCoroutine(PlayerKnockback());
            if (currentHealth > 0) { currentHealth -= 1; }
            else { /*restart from checkpoint*/ }
        }
            
    } // Gets called when player is hit by enemy

    IEnumerator PlayerKnockback()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitImpactTime);
        Time.timeScale = 1;
        if (knockbackDirection.x < 0 && !facingRight || knockbackDirection.x > 0 && facingRight) { FlipCharacter(); }
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(knockbackDirection, ForceMode2D.Impulse);
        yield return new WaitForSecondsRealtime(hitKnockbackTime);
        playerState = PlayerState.PLAYER_FALLING;
        yield return new WaitForSecondsRealtime(invulnerabilityPeriod);
        vulnerable = true;
    }
}
