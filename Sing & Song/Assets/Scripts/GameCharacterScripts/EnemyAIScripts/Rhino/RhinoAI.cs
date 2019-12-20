using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoAI : EnemyAI
{
    [Header("Boss health slider")]
    [SerializeField] private BossHPBarController bossHPBarController = null;
    [SerializeField] private Transform hitParticleSpawnLocation = null;

    private GameObject rhinoBossObjectPool = null;
    public bool ableStartAnimation = false;
    public bool allowRhinoAttackBehaviour = true;
    private bool onPhase2_FirstTimeAttack = true;
    private bool isRhinoFightEndMovie_FirstTimePlay = true;
    private bool isAttacking = false;

    private int phase1_MaxHealth = 0;
    private int phase2_MaxHealth = 0;

    #region General position and count distance variable
    [Space(6.0f)][Header("Rhino AI Details:")]
        [SerializeField] private Transform rockfallSourceOriginPoint;
        [SerializeField] private float halfLengthOfRockfallAppearField;
        private Transform enemyTransform;
    #endregion

    #region Boolean variable
        private bool onPhase2 = false;
        private bool isFinishPhaseCharge = false;
        private bool onPreAttackAnimationFinish = false;
    #endregion

    #region General timer Variable
        [SerializeField] private float rhinoPre_WarActionTime;
        private float rhinoPre_WarActionTimeTimer = 0.0f;
    #endregion

    #region Boss Phase Attack State
        private enum BossPhaseAttack
        {
            None,
            Phase1_Attack1,
            Phase1_Attack2,
            Phase1_Attack3,
            Phase2_Attack1,
            Phase2_Attack2,
        };
        private BossPhaseAttack bossPhaseAttack;
    #endregion

    #region Phase 1 Attack 1 variable required
        [Header("Phase 1 Attack 1 variables :")]

        // For Compute And Set The Rhino Next Movement
        // It Actually Just For Create The Variable In Memory One Time, So The Memory Not Need Create The Variable Every Time(each frame)
        private Vector2 enemyTowardsPositon; // (save a bit memory usage frequency)
    #endregion

    #region Phase 1 Attack 2 variable required 
        [Header("Phase 1 Attack 2 variables :")]

        // Stom Wave Prefab And Wave Game Objects
        [SerializeField] private GameObject wavePrefab;
        private GameObject waveGameObjectsContainer;
        private List<GameObject> waveGameObjects = new List<GameObject>();

        // Stom Wave Number Of Times Detail
        [SerializeField] private int maxNumberOfStomWave;
        private int currentNumberOfStomWave;

        // Stom Wave Game Objects Spawn Origin
        private Vector2 waveSpawnOriginPosition;
    #endregion

    #region Phase 1 Attack 3 variable required
        [Header("Phase 1 Attack 3 variables :")]

        // Different 3 Shpae Of Rockfall Prefab And Rockfall GameObjects
        [SerializeField] private List<GameObject> rockfallPrefabs = new List<GameObject>();
        private GameObject rocksOfFallGameobjectsContainer;
        private List<GameObject> rocksOfFallGameobjects = new List<GameObject>();

        // Setting Rockfall Fixed Appear Position
        private List<Vector2> rockfallFixedDifferentAppearPosition = new List<Vector2>(); // Set Original Positions To Get The Memory Address For Random
        private List<Vector2> tempRockfallFixedDifferentAppearPosition = new List<Vector2>(); // Random Original Positions

        // Rockfalls Number Of Times Detail
        [SerializeField] private int maxNumberOfRockfalls;
        private int currentNumberOfRockfalls;

        // Each Rockfall Have How Many Rock Fall Down
        [SerializeField] private int eachRoundHowManyRockfalls;

        // For Set Each Round‘s Which Rockfall Appear On Which Different Random Fixed Position
        private int indexOfWhichRockIsFalling;

        // Each Round of Each Rockfall interval
        [SerializeField] private float eachRockOfFallIntervalTime;
        private float eachRockOfFallIntervalTimeTimer;
    #endregion
    
    #region Phase 2 Attack 1 variable required
        [Header("Phase 2 Attack 1 variables :")]

        // Rhino Ethereal Armour Prefab And Ethereal Armour Game Object
        [SerializeField] private GameObject enemyEtherealArmourPrefab;
        private GameObject enemyEtherealArmourGameObject;

        // For Check The Rhino Ethereal Armour Game Object Is Disable Display or not
        private bool isEtherealArmourFinish = false;

        // For Set The Rhino Ethereal Armour Appear Position
        private Transform enemyEtherealArmourTransform;
    #endregion

    #region Phase 2 Attack 2 variable required
        [Header("Phase 2 Attack 2 variables :")]

        // Stom Wave With Rockfalls Number Of Times Detail
        [SerializeField] private int maxNumberOfStomWaveAndRockfalls;
        private int currentNumberOfStomWaveAndRockfalls;

        // Each Round of Phase2 Each Rockfall interval
        [SerializeField] private float eachRockOfFallIntervalPhase2Time;
        private float eachRockOfFallIntervalPhase2TimeTimer;

        // For Check Phase 2 Attack 2's Current Attack Type Is Stomp Wave Or Rockfall
        private bool onStompWave = true;
    #endregion

    #region | Getter |
    public bool IsAttacking => isAttacking;
    #endregion

    // For Reference Rhino's Attack Types
    // 
    //  Phase 1
    //      Charges towards player
    //      Stomp the ground to perform attack wave
    //      Stomp the ground and make rocks fall from the top
    //      
    //  Phase 2
    //      Charges towards player with projection of it’s ethereal armour before following up with the actual Rhino
    //      Stomps the ground - Combo of attack wave and rocks falling

    protected override void Initialise()
    {
        isAwayCheckPlayer = false;

        base.Initialise();
        terrainLayer = LayerMask.GetMask("Terrain");
        enemyTransform = transform;

        rhinoBossObjectPool = new GameObject("rhinoBossObjectPool");
        rocksOfFallGameobjectsContainer = new GameObject("rockfallsContainer");
        waveGameObjectsContainer = new GameObject("waveContainer");

        rocksOfFallGameobjectsContainer.transform.SetParent(rhinoBossObjectPool.transform);
        waveGameObjectsContainer.transform.SetParent(rhinoBossObjectPool.transform);

        for (int i=0; i < eachRoundHowManyRockfalls; i++)
        {
            rocksOfFallGameobjects.Add(Instantiate(rockfallPrefabs[Random.Range(0, 3)], Vector2.zero, Quaternion.identity));
            rocksOfFallGameobjects[i].transform.SetParent(rocksOfFallGameobjectsContainer.transform);
            rocksOfFallGameobjects[i].SetActive(false);
        }

        Vector2 tempValue = new Vector2(); // The whole game only use one time, so crete it here more save memory usage frequency 
        for (int i = 0; i < (eachRoundHowManyRockfalls); i++)
        {
            tempValue.x = rockfallSourceOriginPoint.position.x - halfLengthOfRockfallAppearField;
            tempValue.x = tempValue.x + (((rockfallSourceOriginPoint.position.x + halfLengthOfRockfallAppearField) - (rockfallSourceOriginPoint.position.x - halfLengthOfRockfallAppearField)) / (eachRoundHowManyRockfalls-1)) * i;
            tempValue.y = rockfallSourceOriginPoint.position.y;
            rockfallFixedDifferentAppearPosition.Add(tempValue);
        }
        
        
        for (int i=0; i < maxNumberOfStomWave; i++)
        {
            waveGameObjects.Add(Instantiate(wavePrefab, Vector2.zero, Quaternion.identity));
            waveGameObjects[i].transform.SetParent(waveGameObjectsContainer.transform);
            waveGameObjects[i].gameObject.SetActive(false);
        }

        GameObject tempGameObject = Instantiate(enemyEtherealArmourPrefab, Vector2.zero, Quaternion.identity);
        enemyEtherealArmourGameObject = tempGameObject;
        enemyEtherealArmourGameObject.GetComponent<RhinoEtherealArmourController>().rhinoAI = this.GetComponent<RhinoAI>();
        enemyEtherealArmourTransform = enemyEtherealArmourGameObject.transform;
        enemyEtherealArmourTransform.SetParent(rhinoBossObjectPool.transform);
        enemyEtherealArmourGameObject.SetActive(false);
        /*
        foreach (SpriteRenderer tempSpriteRenderer in headGameObjectOfAllSpriteRenderers.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderers.Add(tempSpriteRenderer);
            originialColor.Add(tempSpriteRenderer.color);
        }
        */

        phase2_MaxHealth = (int)(this.maximumHealth/2);
        phase1_MaxHealth = this.maximumHealth - phase2_MaxHealth;

        bossHPBarController.SetUpHPBarSlider(phase1_MaxHealth);
        bossHPBarController.UpdateHPBarProgress(this.currentHealth - phase2_MaxHealth);
    }

    protected override void EnemyPatrol()
    {
        if (ableStartAnimation)
        {
            if (rhinoPre_WarActionTimeTimer.Equals(0.0f))
            {
                //pre-war animation run here
                animator.SetTrigger(Global.nameAnimatorTrigger_RhinoAI_Roar);
            }

            if (rhinoPre_WarActionTimeTimer >= rhinoPre_WarActionTime)
            {
                onPhase2 = false;
                animator.SetBool(Global.nameAnimatorBool_RhinoAI_Phase2, onPhase2);
                enemyState = EnemyState.ENEMY_CHASING;
            }
            else { rhinoPre_WarActionTimeTimer += Time.deltaTime; }
        }
    }

    protected override void EnemyChase()
    {
        isAttacking = false;

        if (currentHealth <= phase2_MaxHealth && onPhase2 == false)
        {
            onPhase2 = true;
            //allowRhinoAttackBehaviour = false;
            FindObjectOfType<RhinoBossFightManager>().PlayBossFightPhase2Movie();
        }

        if (allowRhinoAttackBehaviour)
        {
            if (onPhase2 && onPhase2_FirstTimeAttack)
            {
                this.currentHealth = phase2_MaxHealth;
                bossHPBarController.UpdateHPBarProgress(this.currentHealth, 1f);
                bossPhaseAttack = BossPhaseAttack.Phase2_Attack1;

                isAttacking = true;
                animator.SetBool(Global.nameAnimatorBool_RhinoAI_Phase2, onPhase2);
                enemyState = EnemyState.ENEMY_ATTACKING;

                onPhase2_FirstTimeAttack = false;
                return;
            }

            if (onPhase2.Equals(false))
            {
                bossPhaseAttack = (BossPhaseAttack)Random.Range((int)BossPhaseAttack.Phase1_Attack1, (int)BossPhaseAttack.Phase1_Attack3 + 1);
            }
            else
            {
                bossPhaseAttack = (BossPhaseAttack)Random.Range((int)BossPhaseAttack.Phase2_Attack1, (int)BossPhaseAttack.Phase2_Attack2 + 1);
                //Debug.Log(bossPhaseAttack);
            }

            isAttacking = true;
            animator.SetBool(Global.nameAnimatorBool_RhinoAI_Phase2, onPhase2);
            enemyState = EnemyState.ENEMY_ATTACKING;
        }
    }

    protected override void EnemyAttack()
    {
        if (currentHealth <= phase2_MaxHealth && onPhase2 == false)
        {
            ResetAllAttackModeVariable();

            enemyState = EnemyState.ENEMY_CHASING;
            return;
        }
        switch (bossPhaseAttack)
        {
            case BossPhaseAttack.Phase1_Attack1:
                Phase1_FirstAttackMode();
            break;

            case BossPhaseAttack.Phase1_Attack2:
                Phase1_SecondAttackMode();
            break;

            case BossPhaseAttack.Phase1_Attack3:
                Phase1_ThirdAttackMode();
            break;
                
            case BossPhaseAttack.Phase2_Attack1:
                Phase2_FirstAttackMode();
            break;

            case BossPhaseAttack.Phase2_Attack2:
                Phase2_SecondAttackMode();
            break;
        }
    }

    private void ResetAllAttackModeVariable()
    {
        SetOnPreAttackAnimationFinishTrue();
        SetIsFinishPhaseChargeTrue();
        SetIsEtherealArmourFinishTrue();

        bossPhaseAttack = BossPhaseAttack.None;
        animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);

        tempRockfallFixedDifferentAppearPosition.Clear();
        tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);// reset back the temp position

        eachRockOfFallIntervalTimeTimer = 0;
        indexOfWhichRockIsFalling = 0;
        currentNumberOfStomWave = 0;
        currentNumberOfRockfalls = 0;
        isFinishPhaseCharge = false;
        onPreAttackAnimationFinish = false;

        // Phase 2 variables, actually not need but for safe reset it better
        isEtherealArmourFinish = false;
        onStompWave = false;
        animator.SetBool(Global.nameAnimatorBool_RhinoAI_OnStompWave, onStompWave);
        eachRockOfFallIntervalPhase2TimeTimer = 0;
        indexOfWhichRockIsFalling = 0;
        currentNumberOfStomWaveAndRockfalls = 0;

        animator.SetBool(Global.nameAnimatorBool_RhinoAI_ContinueAttack, false);
    }

    private void Phase1_FirstAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
        }
        else{
            if (onPreAttackAnimationFinish)
            {
                if (hitWall.Equals(false))
                {
                    enemyTowardsPositon.x = -moveSpeed;
                    enemyTowardsPositon.y = 0.0f;
                    enemyTransform.Translate(enemyTowardsPositon * Time.deltaTime);
                }
                else
                {
                    bossPhaseAttack = BossPhaseAttack.None;
                    animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);

                    // set values back to defalut
                    isFinishPhaseCharge = false;
                    onPreAttackAnimationFinish = false;

                    enemyState = EnemyState.ENEMY_RESTING;
                }
            }
        }
    }

    private void Phase1_SecondAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            animator.SetBool(Global.nameAnimatorBool_RhinoAI_ContinueAttack, true);
        }
        else
        {
            if (onPreAttackAnimationFinish)
            {
                waveSpawnOriginPosition.x = facingRight ? capsuleCollider2D.bounds.max.x : capsuleCollider2D.bounds.min.x;
                waveSpawnOriginPosition.y = capsuleCollider2D.bounds.min.y;

                waveGameObjects[currentNumberOfStomWave].transform.SetPositionAndRotation(waveSpawnOriginPosition, this.transform.rotation);
                waveGameObjects[currentNumberOfStomWave].SetActive(true);

                currentNumberOfStomWave++;
                onPreAttackAnimationFinish = false;
            }

            if (currentNumberOfStomWave >= maxNumberOfStomWave)
            {
                animator.SetBool(Global.nameAnimatorBool_RhinoAI_ContinueAttack, false);

                bossPhaseAttack = BossPhaseAttack.None;
                animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);

                // set values back to defalut
                currentNumberOfStomWave = 0;
                isFinishPhaseCharge = false;
                onPreAttackAnimationFinish = false;

                enemyState = EnemyState.ENEMY_RESTING;
            }
        }
    }

    private void Phase1_ThirdAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            indexOfWhichRockIsFalling = 0;
            animator.SetBool(Global.nameAnimatorBool_RhinoAI_ContinueAttack, true);

            if (tempRockfallFixedDifferentAppearPosition.Count <= 0)
            {
                tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);
            }
        }
        else
        {
            if (onPreAttackAnimationFinish)
            {
                if (eachRockOfFallIntervalTimeTimer < eachRockOfFallIntervalTime)
                {
                    eachRockOfFallIntervalTimeTimer += Time.deltaTime;
                }
                else
                {
                    eachRockOfFallIntervalTimeTimer = 0;

                    int whichPosition = Random.Range(0, tempRockfallFixedDifferentAppearPosition.Count); // random the current rockfall position without overlap the same position in a round

                    // set the rock appaer to the position and default rotate
                    rocksOfFallGameobjects[indexOfWhichRockIsFalling].transform.SetPositionAndRotation(tempRockfallFixedDifferentAppearPosition[whichPosition], Quaternion.identity);

                    rocksOfFallGameobjects[indexOfWhichRockIsFalling].SetActive(true);

                    tempRockfallFixedDifferentAppearPosition.RemoveAt(whichPosition);// remove the temp position at which Position, so next rockfall won't be same postion after a round

                    if (indexOfWhichRockIsFalling.Equals(eachRoundHowManyRockfalls - 1))
                    {
                        // set values back to defalut
                        indexOfWhichRockIsFalling = 0;
                        tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);// reset back the temp position

                        currentNumberOfRockfalls++;
                        onPreAttackAnimationFinish = false;
                    }
                    else
                    {
                        indexOfWhichRockIsFalling++;
                    }
                }

            }

            if (currentNumberOfRockfalls >= maxNumberOfRockfalls)
            {
                animator.SetBool(Global.nameAnimatorBool_RhinoAI_ContinueAttack, false);

                bossPhaseAttack = BossPhaseAttack.None;
                animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);

                // set values back to defalut
                currentNumberOfRockfalls = 0;
                isFinishPhaseCharge = false;
                onPreAttackAnimationFinish = false;

                enemyState = EnemyState.ENEMY_RESTING;
            }
        }
    }

    private void Phase2_FirstAttackMode()
    {
        if (!isFinishPhaseCharge)
        {

            //call animation here
            animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            isEtherealArmourFinish = false;
        }
        else
        {
            if (!isEtherealArmourFinish)
            {
                if (onPreAttackAnimationFinish && enemyEtherealArmourGameObject.activeSelf == false)
                {
                    enemyEtherealArmourTransform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
                    enemyEtherealArmourGameObject.GetComponent<RhinoEtherealArmourController>().facingRight = this.facingRight;
                    enemyEtherealArmourGameObject.SetActive(true);
                    onPreAttackAnimationFinish = false;
                }
            }
            else{
                if (!hitWall)
                {
                    enemyTowardsPositon.x = -moveSpeed * 1.3f;
                    enemyTowardsPositon.y = 0.0f;
                    enemyTransform.Translate(enemyTowardsPositon * Time.deltaTime);
                }
                else
                {
                    bossPhaseAttack = BossPhaseAttack.None;
                    animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);

                    // set values back to defalut
                    isFinishPhaseCharge = false;
                    onPreAttackAnimationFinish = false;
                    isEtherealArmourFinish = false;

                    enemyState = EnemyState.ENEMY_RESTING;
                }
            }
        }
    }

    private void Phase2_SecondAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            onStompWave = false;
            animator.SetBool(Global.nameAnimatorBool_RhinoAI_OnStompWave, onStompWave);
            animator.SetBool(Global.nameAnimatorBool_RhinoAI_ContinueAttack, true);

            if (tempRockfallFixedDifferentAppearPosition.Count <= 0)
            {
                tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);
            }
        }
        else
        {
            if (onStompWave.Equals(false))
            {
                if (onPreAttackAnimationFinish)
                {
                    if (eachRockOfFallIntervalPhase2TimeTimer < eachRockOfFallIntervalPhase2Time)
                    {
                        eachRockOfFallIntervalPhase2TimeTimer += Time.deltaTime;
                    }
                    else
                    {
                        eachRockOfFallIntervalPhase2TimeTimer = 0;

                        int whichPosition = Random.Range(0, tempRockfallFixedDifferentAppearPosition.Count); // random the current rockfall position without overlap the same position in a round

                        // set the rock appaer to the position and default rotate
                        rocksOfFallGameobjects[indexOfWhichRockIsFalling].transform.SetPositionAndRotation(tempRockfallFixedDifferentAppearPosition[whichPosition], Quaternion.identity);

                        rocksOfFallGameobjects[indexOfWhichRockIsFalling].SetActive(true);

                        tempRockfallFixedDifferentAppearPosition.RemoveAt(whichPosition);// remove the temp position at which Position, so next rockfall won't be same postion after a round

                        if (indexOfWhichRockIsFalling.Equals(eachRoundHowManyRockfalls - 1))
                        {
                            // set values back to defalut
                            indexOfWhichRockIsFalling = 0;
                            tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);// reset back the temp position
                            
                            onPreAttackAnimationFinish = false;
                            onStompWave = true;
                            animator.SetBool(Global.nameAnimatorBool_RhinoAI_OnStompWave, onStompWave);
                        }
                        else
                        {
                            indexOfWhichRockIsFalling++;
                        }
                    }
                }
            }
            else
            {
                if (onPreAttackAnimationFinish)
                {
                    waveSpawnOriginPosition.x = facingRight ? capsuleCollider2D.bounds.max.x : capsuleCollider2D.bounds.min.x;
                    waveSpawnOriginPosition.y = capsuleCollider2D.bounds.min.y;

                    waveGameObjects[currentNumberOfStomWave].transform.SetPositionAndRotation(waveSpawnOriginPosition, this.transform.rotation);
                    waveGameObjects[currentNumberOfStomWave].SetActive(true);

                    onPreAttackAnimationFinish = false;
                    onStompWave = false;
                    animator.SetBool(Global.nameAnimatorBool_RhinoAI_OnStompWave, onStompWave);

                    currentNumberOfStomWaveAndRockfalls++;
                }
            }

            if (currentNumberOfStomWaveAndRockfalls >= maxNumberOfStomWaveAndRockfalls)
            {
                animator.SetBool(Global.nameAnimatorBool_RhinoAI_ContinueAttack, false);

                bossPhaseAttack = BossPhaseAttack.None;
                animator.SetInteger(Global.nameAnimatorInteger_RhinoAI_AttackType, (int)bossPhaseAttack);

                // set values back to defalut
                currentNumberOfStomWaveAndRockfalls = 0;
                isFinishPhaseCharge = false;
                onPreAttackAnimationFinish = false;

                enemyState = EnemyState.ENEMY_RESTING;
            }

        }
    }

    public void RhinoFliping()
    {
        FlipCharacter();
    }

    public void SetOnPreAttackAnimationFinishTrue()
    {
        onPreAttackAnimationFinish = true;
    }

    public void SetIsFinishPhaseChargeTrue()
    {
        isFinishPhaseCharge = true;
    }

    public void SetIsEtherealArmourFinishTrue()
    {
        isEtherealArmourFinish = true;
    }

    public override void DamageEnemyMelee()
    {
        if (!spiritArmour.activeSelf)
        {
            Instantiate(hitParticleEffect, hitParticleSpawnLocation.position, Quaternion.identity);
            if (this.currentHealth > 0)
            {
                
                if (!onPhase2 && this.currentHealth > phase2_MaxHealth)
                {
                    this.currentHealth -= 1;
                    bossHPBarController.UpdateHPBarProgress(this.currentHealth - phase2_MaxHealth);
                }
                else if(onPhase2)
                {
                    this.currentHealth -= 1;
                    bossHPBarController.UpdateHPBarProgress(this.currentHealth);
                }

                StopCoroutine("DamageColorChange");
                StopCoroutine("NotDamageColorChange");

                StartCoroutine("DamageColorChange");
            }

        }
    }

    protected override void EnemyDieSound()
    {
        SoundManagerScripts.PlaySound("rhino_die_sound");
    }

    /*
    private IEnumerator DamageColorChange()
    {
        float colorChangeTime01 = 0.15f;
        float colorChangeTime02 = 0.05f;
        float colorChangeTime03 = 0.05f;

        WaitForSeconds waitForSeconds01 = new WaitForSeconds(colorChangeTime01);
        WaitForSeconds waitForSeconds02 = new WaitForSeconds(colorChangeTime02);
        WaitForSeconds waitForSeconds03 = new WaitForSeconds(colorChangeTime03);

        foreach (SpriteRenderer tempSpriteRenderer in spriteRenderers)
        {
            tempSpriteRenderer.color = getDamageColor;
        }
        yield return waitForSeconds01;
        for (int i=0; i < spriteRenderers.Count; i++)
        {
                spriteRenderers[i].color = originialColor[i];
        }
        yield return waitForSeconds02;

        foreach (SpriteRenderer tempSpriteRenderer in spriteRenderers)
        {
            tempSpriteRenderer.color = getDamageColor;
        }
        yield return waitForSeconds03;
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].color = originialColor[i];
        }
        yield return null;
    }

    /*
    private IEnumerator NotDamageColorChange()
    {
        foreach (SpriteRenderer tempSpriteRenderer in spriteRenderers)
        {
            tempSpriteRenderer.color = getNotDamageColor;
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].color = originialColor[i];
        }
        yield return null;
    }
    */

    protected override void EnemyDieColorChange()
    {
        if (isRhinoFightEndMovie_FirstTimePlay)
        {
            FindObjectOfType<RhinoBossFightManager>().PlayBossFightEndMovie();
            isRhinoFightEndMovie_FirstTimePlay = false;
        }

        /*
        foreach (SpriteRenderer tempSpriteRenderer in spriteRenderers)
        {
            tempDieTransparentColor.r = tempSpriteRenderer.color.r;
            tempDieTransparentColor.g = tempSpriteRenderer.color.g;
            tempDieTransparentColor.b = tempSpriteRenderer.color.b;
            tempDieTransparentColor.a = tempSpriteRenderer.color.a - (dieTransparentColorSpeed * Time.deltaTime);
            tempSpriteRenderer.color = tempDieTransparentColor;
        }
        
        if(spriteRenderers[spriteRenderers.Count-1].color.a <= 0.0f)
        {
            Debug.Log("Ending");
            FindObjectOfType<RhinoBossFightManager>().PlayBossFightEndMovie();
            //Global.gameManager.TimeToEndAndSayThankYou();
            this.gameObject.SetActive(false);
        }
        */
    }
    
    private void OnDestroy()
    {
        /*
        //For clear residuary rockfalls when the boss is die, because the player only can fight this boss one time, if the player still can fight again this boss then delete this whole OnDestroy() function.
        for (int i = 0; i < rocksOfFallGameobjects.Count; i++)
        {
            Destroy(rocksOfFallGameobjects[i]);
            //Debug.Log("delete rocks fall");
        }
        for (int i = 0; i < maxNumberOfStomWave; i++)
        {
            Destroy(waveGameObjects[i]);
            //Debug.Log("delete rhino wave");
        }
        Destroy(enemyEtherealArmourGameObject);
        //Debug.Log("delete rhino enemy ethereal armour");
        */
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        //if (this.colliderTransform != null)
        //{
        //    Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange);
        //    Gizmos.DrawWireCube(this.colliderTransform.position, attackRange);
        //}
        // For see the line of fallrock length of Cave top 
        Gizmos.color = Color.white;
        Gizmos.DrawRay(rockfallSourceOriginPoint.position, Vector2.left * halfLengthOfRockfallAppearField);
        Gizmos.DrawRay(rockfallSourceOriginPoint.position, Vector2.right * halfLengthOfRockfallAppearField);
        
    }
    
}
