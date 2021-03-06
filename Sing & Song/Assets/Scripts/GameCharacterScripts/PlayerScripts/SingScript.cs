﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingScript : GameCharacter
{
    [SerializeField] Transform singBelt = null;
    [SerializeField] private GameObject singMusicNoteParticleEffect;
    [SerializeField] private GameObject songMusicNoteParticleEffect;
    private ParticleSystem singMusicNotes;
    private ParticleSystem songMusicNotes;
    bool healedOnce = false;
    #region Permission Variables
    [Header("Ability Variables")]
    [SerializeField] private Image[] abilityHearts;
    private bool canDoAction = true;
    public bool canHeal = true;
    public bool canDash = true;
    private bool canSpiritAttack = false;
    private float dealthZone = 0.2f;

    public bool CanDoAction { get { return canDoAction; } set { canDoAction = value; playerState = PlayerState.PLAYER_IDLE; } }
    #endregion
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
    [Header("Spirit Variable")]
    [SerializeField] private float maximumSpirit = 10f;
    private float currentSpirit;
    #endregion
    #region Jump Variables
    [Header("Jump Variables")]
    [SerializeField] private float jumpHeight = 4.5f;
    [SerializeField] private float timeToJumpApex = 0.35f;
    [SerializeField] private GameObject landParticleEffect = null;
    private float jumpVelocity;
    private float jumpTime;
    private float gravity;
    #endregion
    #region Heal Variables
    [Header("Heal Variables")]
    [SerializeField] private float inputBufferTime = 0.2f;
    [SerializeField] private float spiritDrainToUse = 3f;
    [SerializeField] private float healTime = 1.5f;
    [SerializeField] private GameObject healParticleEffect = null;
    [SerializeField] private GameObject songHealParticleEffect = null;
    [SerializeField] private GameObject finishHealParticleEffect = null;
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
    [SerializeField] private float meleeAttackBuffer;
    private float meleeAttackIntervalTimer;
    private float meleeAttackBufferTimer;
    private Collider2D[] enemiesHit;
    #endregion
    #region Spirit Attack Variables
    [Header("Spirit Attack Variables")]
    [SerializeField] private GameObject spiritAttack;
    [SerializeField] private float spiritAttackDuration;
    private float spiritAttackDurationTimer;
    #endregion  
    #region Damaged Variables
    [Header("Damaged Variables")]
    [SerializeField] private float hitImpactTime = 0.2f;
    [SerializeField] private float invulnerabilityPeriod = 0.5f;
    private bool vulnerable;
    private bool ignoreEnemyCollision;
    #endregion
    #region Song Node Variables
    [Header("Song Variables")]
    [HideInInspector] public bool doAnimationFollowPlayerState = true;
    [SerializeField] private GameObject song;
    [SerializeField] private Animator songAnimator;
    [SerializeField] private Vector2 nodeOffsetDefault = new Vector2(1.5f, 0f);
    [SerializeField] private float nodeInterval = 0.15f;
    protected Vector2 nodePosition;
    private Vector2 nodeOffset;
    private Quaternion songTargetRotation;
    private float songRotationTime;
    private float nodeIntervalTimer;
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
    #region PlayerState Enums
    private enum PlayerState
    {
        PLAYER_IDLE,
        PLAYER_RUNNING,
        PLAYER_JUMPING,
        PLAYER_FALLING,
        PLAYER_HEALING,
        PLAYER_DASHING,
        PLAYER_SPIRIT,
        PLAYER_HIT
    };
    private PlayerState playerState;
    #endregion
    #region Sound Variables
    [SerializeField] private float JumpSoundTime = 1f;
    private float JumpSoundTimer = 0f;
    #endregion
    //float timeBtwTrail = 0f;

    protected override void Initialise()
    {
        base.Initialise();
        #region Initialise Permission Variables
        for (int i = 0; i < abilityHearts.Length; i++) { abilityHearts[i].enabled = false; }
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
        #region Initialise Melee Attack Variables
        meleeAttackIntervalTimer = meleeAttackInterval;
        #endregion
        #region Initialise Spirit Attack Variables
        spiritAttack.SetActive(false);
        #endregion
        #region Initialise Damaged Variables
        vulnerable = true;
        #endregion
        #region Initialise Song Node Variables
        nodePosition = (Vector2)transform.position + nodeOffset;
        #endregion
        #region Initialise UI Variables
        minimumSpiritWellPosition = new Vector2(spiritWell.rectTransform.anchoredPosition.x, -200);
        maximumSpiritWellPosition = new Vector2(spiritWell.rectTransform.anchoredPosition.x, -65);
        previousSpiritWellPosition = currentSpiritWellPosition.y;
        spiritWellAlpha = spiritWell.color.a;
        //spiritWellEdge = 500f;
        #endregion
        # region Initialise Player Health To Be Full When Each Game Load Again

            currentHealth = maximumHealth;

        #endregion
        #region Initialise Particle Systems
        singMusicNotes = singMusicNoteParticleEffect.GetComponent<ParticleSystem>();
        songMusicNotes = songMusicNoteParticleEffect.GetComponent<ParticleSystem>();
        #endregion
    } // Initialises player variables
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!animator.GetBool("isResting"))
            {
                animator.SetBool("isResting", true);
                songAnimator.SetBool("isResting", true);
            }
            else
            {
                animator.SetBool("isResting", false);
                songAnimator.SetBool("isResting", false);
            }
        }

        if (Input.GetKey(KeyCode.O))
        {
            songAnimator.SetBool("isHealing", true);
        }
        else
        {
            songAnimator.SetBool("isHealing", false);
        }

        if (canDoAction) // If canDoAction is false, that mean have some event triggered and it don't want player can be controling
        {

            #region Check Permissions
            //this.playerState = PlayerState.PLAYER_IDLE;
            if (canHeal)
        {
            abilityHearts[0].enabled = true;
            abilityHearts[0].color = new Color32(148, 250, 242, 255);
        }
        if(canHeal && canDash)
        {
            abilityHearts[1].enabled = true;
            abilityHearts[0].color = new Color32(255, 255, 255, 255);
            abilityHearts[1].color = new Color32(148, 250, 242, 255);
        }
            #endregion
            #region Check Inputs
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // Input for directions
                if (input.sqrMagnitude < (dealthZone * dealthZone)) input = Vector2.zero;
                inputJumpPress = Input.GetButtonDown("JumpButton"); // Input for jump press
                inputJump = Input.GetButton("JumpButton"); // Input for jump
                if (canHeal) inputHeal = Input.GetButton("HealButton"); // Input for heal
                if (canDash) inputDash = Input.GetButtonDown("DashButton"); // Input for dash
                inputMeleeAttack = Input.GetButtonDown("MeleeAttackButton"); // Input for melee attack
                if (canSpiritAttack) inputSpiritAttack = Input.GetButtonDown("SpiritAttackButton"); // Input for spirit attack

                inputInteract = Input.GetButtonDown("InteractButton"); // Input for interact
            #endregion
            #region Check Ground & Ceiling
            UpdateRaycastOrigins();
            VerticalCollisionDetection();
            #endregion
            #region Check Health
            if(currentHealth <= 0)
            {
                FindObjectOfType<STARTMENU>().PlayerRevive();
            }
            #endregion
            #region Check Spirit
            if (currentSpirit > maximumSpirit) { currentSpirit = maximumSpirit; }
            if(currentSpirit < 0) { currentSpirit = 0; }
            if(playerState != PlayerState.PLAYER_HEALING && currentSpirit >= spiritDrainToUse - 0.1f && currentSpirit < spiritDrainToUse) { currentSpirit = Mathf.Round(currentSpirit); }
            #endregion
            #region Reset jumpTime
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
            if(playerState == PlayerState.PLAYER_IDLE || playerState == PlayerState.PLAYER_RUNNING || playerState == PlayerState.PLAYER_JUMPING || playerState == PlayerState.PLAYER_FALLING)
            {
                if(inputMeleeAttack && meleeAttackIntervalTimer >= meleeAttackInterval)
                {
                    meleeAttackIntervalTimer = 0f;
                    PlayerMeleeAttack();
                }
            }
            #endregion
            #region Update Spirit Attack Position
            spiritAttack.transform.position = meleeAttackTransform.position;
            if(inputSpiritAttack && currentSpirit >= spiritDrainToUse)
            {
                currentSpirit -= spiritDrainToUse;
                playerState = PlayerState.PLAYER_SPIRIT;
            }
            #endregion
            #region Set Enemy Layer Collision
            ignoreEnemyCollision = !vulnerable;
            Physics2D.IgnoreLayerCollision(8, 9, ignoreEnemyCollision);
            #endregion
            #region Update UI
            for (int i = 0; i < healthCrystals.Length; i++) { healthCrystals[i].enabled = (i < currentHealth) ? true : false; }
            spiritWellAlpha = (playerState != PlayerState.PLAYER_HEALING && currentSpirit < spiritDrainToUse) ? 0.6f : 1f;
            spiritWell.color = new Color(1f, 1f, 1f, Mathf.Lerp(spiritWell.color.a, spiritWellAlpha, Time.deltaTime * 2));
            currentSpiritWellPosition = Vector2.Lerp(minimumSpiritWellPosition, maximumSpiritWellPosition, currentSpirit/maximumSpirit);
            spiritWell.rectTransform.anchoredPosition = Vector2.Lerp(spiritWell.rectTransform.anchoredPosition, currentSpiritWellPosition, Time.deltaTime * 10f);

            if(previousSpiritWellPosition != currentSpiritWellPosition.y)
            {
                //spiritWell.rectTransform.Translate(spiritWellFlowSpeed * Time.deltaTime, 0f, 0f);
                previousSpiritWellPosition = currentSpiritWellPosition.y;
            }
            #endregion
            #region Update Animation
            //if(inputJumpPress && isGrounded) { animator.SetTrigger("jump"); }
            //if(inputMeleeAttack && meleeAttackIntervalTimer >= meleeAttackInterval) { animator.SetTrigger("attack"); }
            #endregion

            PlayerSwitchState();
        }

        #region Update Song Node Position
        nodeOffset = (facingRight) ? -nodeOffsetDefault : nodeOffsetDefault;
        if (nodeIntervalTimer > nodeInterval)
        {
            nodeIntervalTimer = 0f;
            //nodePosition = (Vector2)transform.position + nodeOffset;
            if (playerState == PlayerState.PLAYER_IDLE || playerState == PlayerState.PLAYER_RUNNING) { nodePosition = (Vector2)transform.position + nodeOffset; }
            if (playerState == PlayerState.PLAYER_JUMPING || playerState == PlayerState.PLAYER_FALLING || playerState == PlayerState.PLAYER_DASHING || playerState == PlayerState.PLAYER_SPIRIT) { nodePosition = (Vector2)transform.position; }
        }
        else { nodeIntervalTimer += Time.deltaTime; }
        #endregion
        #region Update Song Position
        if (doAnimationFollowPlayerState)
        {
            if (this.transform.position.x < song.transform.position.x && song.transform.rotation.eulerAngles.y < 90f)
            {
                songTargetRotation = Quaternion.Euler(songTargetRotation.eulerAngles.x, 180f, songTargetRotation.eulerAngles.z);
                songRotationTime = 0f;
                StartCoroutine(FlipSongSprite());
            }
            if (this.transform.position.x > song.transform.position.x && song.transform.rotation.eulerAngles.y > 90f)
            {
                songTargetRotation = Quaternion.Euler(songTargetRotation.eulerAngles.x, 0f, songTargetRotation.eulerAngles.z);
                songRotationTime = 0f;
                StartCoroutine(FlipSongSprite());
            }

            if (playerState == PlayerState.PLAYER_IDLE || playerState == PlayerState.PLAYER_RUNNING)
            {
                song.GetComponent<Rigidbody2D>().gravityScale = 2f;
                if (song.transform.position.x == nodePosition.x) { songAnimator.SetBool("isRunning", false); }
                else
                {
                    songAnimator.SetBool("isRunning", true);
                    song.transform.position = Vector2.MoveTowards(song.transform.position, new Vector2(nodePosition.x, song.transform.position.y), moveSpeed * Time.deltaTime);
                }
            }
            if (playerState == PlayerState.PLAYER_JUMPING || playerState == PlayerState.PLAYER_FALLING)
            {
                songAnimator.SetBool("isRunning", false);
                song.transform.position = Vector2.MoveTowards(song.transform.position, new Vector2(this.transform.position.x, song.transform.position.y), moveSpeed * Time.deltaTime);
                //song.GetComponent<Rigidbody2D>().gravityScale = 0f;
                //song.transform.position = Vector2.MoveTowards(song.transform.position, nodePosition, moveSpeed * Time.deltaTime);
            }
            if (playerState == PlayerState.PLAYER_HEALING)
            {
                songAnimator.SetBool("isHealing", true);
            }
            if (playerState == PlayerState.PLAYER_DASHING)
            {
                song.transform.position = this.transform.position;
            }
        }
        #endregion

        //print($"canDoAction: {canDoAction}");
        //print($"playerHeal: {inputHeal}");
        //print($"playerState: {playerState}");
        //print($"inputBuffer: {inputBuffer}");
        //print($"isGrounded: {isGrounded}");
        //print($"isHitCeiling: {isHitCeiling}");
        //print($"currentHealth: {currentHealth}");
        //print($"currentSpirit: {currentSpirit}");
        //print($"dashedInAir: {dashedInAir}");
        //print($"enemiesHit: {enemiesHit}");
        //print($"meleeAttackBufferTimer: {meleeAttackBufferTimer}");
    } // Gets called every frame
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackTransform.position, attackRange.x);
    } // Draws melee attack range
    private void PlayerSwitchState()
    {
        switch (playerState)
        {
            case PlayerState.PLAYER_IDLE:
                animator.SetBool("isRunning", false);
                healParticleEffect.SetActive(false);
                songHealParticleEffect.SetActive(false);
                PlayerFlip();
                meleeAttackTransform.localPosition = (input.y > 0) ? Vector2.up * 2f : Vector2.left * 1.5f;
                spiritAttack.transform.localRotation = (input.y > 0) ? Quaternion.Euler(0f, 0f, -90f) : Quaternion.Euler(0f, 0f, 0f);
                if (!isGrounded) { playerState = PlayerState.PLAYER_FALLING; }
                else if(input.x != 0) { playerState = PlayerState.PLAYER_RUNNING; }
                else if(inputJumpPress && isGrounded)
                {
                    //Instantiate(landParticleEffect, transform.position, Quaternion.identity);
                    animator.SetTrigger("jump");
                    StartCoroutine(SongJump());
                    playerState = PlayerState.PLAYER_JUMPING;
                }
                else if(inputHeal && currentSpirit >= spiritDrainToUse)
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
                healParticleEffect.SetActive(false);
                songHealParticleEffect.SetActive(false);

                //if (timeBtwTrail >= 0.25f)
                //{
                //    Instantiate(landParticleEffect, transform.position, Quaternion.identity);
                //    timeBtwTrail = 0f;
                //}
                //else
                //    timeBtwTrail += Time.deltaTime;

                PlayerFlip();
                PlayerMove();
                meleeAttackTransform.localPosition = (input.y > 0) ? Vector2.up * 2f : Vector2.left * 1.5f;
                spiritAttack.transform.localRotation = (input.y > 0) ? Quaternion.Euler(0f, 0f, -90f) : Quaternion.Euler(0f, 0f, 0f);
                if (!isGrounded) { playerState = PlayerState.PLAYER_FALLING; }
                else if(input.x == 0) { playerState = PlayerState.PLAYER_IDLE; }
                else if(inputJumpPress && isGrounded)
                {
                    //Instantiate(landParticleEffect, transform.position, Quaternion.identity);
                    animator.SetTrigger("jump");
                    StartCoroutine(SongJump());
                    playerState = PlayerState.PLAYER_JUMPING;
                }
                else if(inputHeal && currentSpirit >= spiritDrainToUse)
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
                healParticleEffect.SetActive(false);
                songHealParticleEffect.SetActive(false);
                PlayerFlip();
                PlayerMove();
                PlayerJump();
                meleeAttackTransform.localPosition = (input.y != 0) ? Vector2.up * input.y * 2f : Vector2.left * 1.5f;
                spiritAttack.transform.localRotation = (input.y != 0) ? Quaternion.Euler(0f, 0f, input.y * -90f) : Quaternion.Euler(0f, 0f, 0f);
                if (!inputJump || isHitCeiling) { playerState = PlayerState.PLAYER_FALLING; }
                else if(inputDash && dashIntervalTimer >= dashInterval && !dashedInAir)
                {
                    dashIntervalTimer = 0f;
                    dashedInAir = true;
                    playerState = PlayerState.PLAYER_DASHING;
                }
                break;
            case PlayerState.PLAYER_FALLING:
                animator.SetBool("isRunning", false);
                healParticleEffect.SetActive(false);
                songHealParticleEffect.SetActive(false);
                PlayerFlip();
                PlayerMove();
                meleeAttackTransform.localPosition = (input.y != 0) ? Vector2.up * input.y * 2f : Vector2.left * 1.5f;
                spiritAttack.transform.localRotation = (input.y != 0) ? Quaternion.Euler(0f, 0f, input.y * -90f) : Quaternion.Euler(0f, 0f, 0f);
                if (isGrounded)
                {
                    dashedInAir = false;
                    animator.SetTrigger("land");
                    Instantiate(landParticleEffect, transform.position, Quaternion.identity);
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
                healParticleEffect.SetActive(true);
                songHealParticleEffect.SetActive(true);
                this.rigidbody2D.velocity = Vector2.zero;
                PlayerHeal();
                if(!isGrounded) { playerState = PlayerState.PLAYER_FALLING; }
                else if(!inputHeal || currentSpirit <= 0 || finishedHeal && currentSpirit < spiritDrainToUse || finishedHeal && currentHealth >= maximumHealth)
                {
                    spiritDrain = 0;
                    songAnimator.SetBool("isHealing", false);
                    playerState = PlayerState.PLAYER_IDLE;
                }
                else if(inputHeal && currentSpirit >= spiritDrainToUse && finishedHeal) { playerState = PlayerState.PLAYER_HEALING; }
                break;
            case PlayerState.PLAYER_DASHING:
                animator.SetBool("isRunning", false);
                healParticleEffect.SetActive(false);
                songHealParticleEffect.SetActive(false);
                PlayerDash();
                break;
            case PlayerState.PLAYER_SPIRIT:
                animator.SetBool("isRunning", false);
                healParticleEffect.SetActive(false);
                songHealParticleEffect.SetActive(false);
                this.rigidbody2D.gravityScale = 0f;
                this.rigidbody2D.velocity = Vector2.zero;
                PlayerSpiritAttack();
                break;
            case PlayerState.PLAYER_HIT:
                animator.SetBool("isRunning", false);
                healParticleEffect.SetActive(false);
                songHealParticleEffect.SetActive(false);
                break;
        }
    } // Determines what happens in each playerState
    private void PlayerFlip() { if(input.x > 0 && !facingRight || input.x < 0 && facingRight) { FlipCharacter(); } } // Checks direction of player
    private void PlayerMove() { this.rigidbody2D.velocity = new Vector2(input.x * moveSpeed, this.rigidbody2D.velocity.y); } // Makes player move
    private void PlayerJump()
    {
        if (jumpTime >= timeToJumpApex) { playerState = PlayerState.PLAYER_FALLING; }
        else
        {
            jumpTime += Time.deltaTime;
            this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, jumpVelocity);
        }
    } // Makes player jump
    private void PlayerHeal()
    {
        if(spiritDrain >= spiritDrainToUse)
        {
            spiritDrain = 0;
            if(currentHealth < maximumHealth)
            {
                currentHealth += 1;
                print($"currentHealth: {currentHealth}");
            }
            Instantiate(finishHealParticleEffect, singBelt.position, Quaternion.identity);
            finishedHeal = true;
            healedOnce = true;
            print(healedOnce);
        }
        else
        {
            spiritDrain += Time.deltaTime * (spiritDrainToUse / healTime);
            currentSpirit -= Time.deltaTime * (spiritDrainToUse / healTime);
            finishedHeal = false;
        }
    } // Makes player heal
    private void PlayerDash()
    {
        if(dashTimeTimer >= dashTime)
        {
            dashTimeTimer = 0;
            this.rigidbody2D.velocity = Vector2.zero;
            SoundManagerScripts.PlaySound("playerDash");//player dash sound
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
        if(meleeAttackTransform.localPosition.y == 0f) { animator.SetTrigger("attack"); }
        if(meleeAttackTransform.localPosition.y > 0f) { animator.SetTrigger("attackUp"); }
        if(meleeAttackTransform.localPosition.y < 0f) { animator.SetTrigger("attackDown"); }

        enemiesHit = Physics2D.OverlapCircleAll(meleeAttackTransform.position, attackRange.x, enemyLayer);
        if(enemiesHit.Length != 0)
        {
            this.rigidbody2D.velocity = Vector2.zero;
            if (meleeAttackTransform.localPosition.y < 0) { this.rigidbody2D.AddForce(new Vector2(0f, knockbackForce.y * 1.5f), ForceMode2D.Impulse); }
            else
            {
                if(facingRight) { this.rigidbody2D.AddForce(new Vector2(-knockbackForce.x * 0.5f, 0f), ForceMode2D.Impulse); }
                else { this.rigidbody2D.AddForce(new Vector2(knockbackForce.x * 0.5f, 0f), ForceMode2D.Impulse); }
            }
            playerState = (isGrounded) ? PlayerState.PLAYER_IDLE : PlayerState.PLAYER_FALLING;
        }
        for(int i = 0; i < enemiesHit.Length; i++)
        {
            if(enemiesHit[i].GetComponentInParent<EnemyAI>() != null) { enemiesHit[i].GetComponentInParent<EnemyAI>().DamageEnemyMelee(); }
            currentSpirit = (currentSpirit < maximumSpirit) ? currentSpirit += 1 : maximumSpirit;
        }
    } // Makes player use melee attack
    private void PlayerSpiritAttack()
    {
        if(spiritAttackDurationTimer >= spiritAttackDuration)
        {
            spiritAttackDurationTimer = 0f;
            spiritAttack.SetActive(false);
            this.rigidbody2D.gravityScale = gravity;
            playerState = (isGrounded) ? PlayerState.PLAYER_IDLE : PlayerState.PLAYER_FALLING;
        }
        else
        {
            spiritAttackDurationTimer += Time.deltaTime;
            spiritAttack.SetActive(true);
        }
    } // Makes player use spirit attack
    public void DamagePlayer(Transform enemyTransform)
    {
        if(enemyTransform.position.x >= this.transform.position.x) { knockbackDirection.x = -knockbackForce.x; SoundManagerScripts.PlaySound("playerGetHit"); }
        else{ knockbackDirection.x = knockbackForce.x; SoundManagerScripts.PlaySound("playerGetHit"); }//player get hit sound
        if (vulnerable)
        {
            vulnerable = false;
            playerState = PlayerState.PLAYER_HIT;
            GetDamageEffect();
            StartCoroutine(PlayerKnockback());
            if(currentHealth > 0) { currentHealth -= 1; }
        }
    } // Gets called when player is hit by enemy
    IEnumerator PlayerKnockback()
    {
        animator.SetTrigger("hurt");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitImpactTime);
        Time.timeScale = 1;
        if (knockbackDirection.x < 0 && !facingRight || knockbackDirection.x > 0 && facingRight) { FlipCharacter(); }
        this.rigidbody2D.velocity = Vector2.zero;
        this.rigidbody2D.AddForce(knockbackDirection, ForceMode2D.Impulse);
        yield return new WaitForSecondsRealtime(knockbackTime);
        playerState = PlayerState.PLAYER_FALLING;
        yield return new WaitForSecondsRealtime(invulnerabilityPeriod);
        vulnerable = true;
    } // Coroutine that runs when player is hit

    public void FlipSong(object invoker, bool facingRight)
    {
        if (typeof(RockScript).IsAssignableFrom(invoker.GetType()))
        {
            songTargetRotation = Quaternion.Euler(songTargetRotation.eulerAngles.x, facingRight ? 0.0f : 180.0f, songTargetRotation.eulerAngles.z);
            songRotationTime = 0f;
            StartCoroutine(FlipSongSprite());
        }
    } // Makes song's sprite flip with flip effect and only let the RockScript class can invoking this functions

    IEnumerator FlipSongSprite()
    {
        while(songRotationTime < 1f)
        {
            songRotationTime += Time.deltaTime;
            song.transform.rotation = Quaternion.Lerp(song.transform.rotation, songTargetRotation, songRotationTime);
            yield return null;
        }
    }

    IEnumerator SongJump()
    {
        yield return new WaitForSeconds(nodeInterval);
        song.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpVelocity * 1.15f), ForceMode2D.Impulse);
    }

    public void SetStateIdle()
    {
        this.playerState = PlayerState.PLAYER_IDLE;
        animator.SetBool("isRunning", false);
    }

    public void PlayMusicNoteParticles(bool play)
    {
        if (play)
        {
            singMusicNotes.Play();
            songMusicNotes.Play();
        }
        else
        {
            singMusicNotes.Stop();
            songMusicNotes.Stop();
        }
    }

    public bool GetHealedOnce()
    {
        print("healedOnce");
        return healedOnce;
    }
}
