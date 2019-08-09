using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoAI : EnemyAI
{
    public bool ableDoEnemyState = false;

    #region General position and count distance variable
    [Space(6.0f)][Header("Rhino AI Details:")]
    [SerializeField] private Transform rockfallSourceOriginPoint;
    [SerializeField] private float halfLengthOfRockfallAppearField;
    private Transform enemyTransform;
    #endregion

    #region Raycast check ground and wall variable
    //[SerializeField] private Transform raycastOriginPoint;
    //private float raycastCheckGroundDistance = 0.7f;
    //private float raycastCheckWallDistance = 0.1f;
    //private LayerMask terrainLayer;
    #endregion

    #region Boolean variable
    //private bool onGround;
    //private bool facingWall;
    private bool isSkillHitPlayer;
    private bool isSkillHitWall;
    private bool onPhase2 = false;
    private bool isFinishPhaseCharge = false;
    private bool onPreAttackAnimationFinish = false;
    #endregion

    #region General timer Variable
    [SerializeField] protected float rhinoPre_WarActionTime;
    protected float rhinoPre_WarActionTimeTimer = 0.0f;
    protected float phaseChargeTimeTimer = 0.0f;
    #endregion

    //For check pre-war action got working or not, can delete it when you put in the animation.
    [SerializeField] private GameObject headGameObjectOfAllSpriteRenderers;
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    [SerializeField] private Color getDamageColor;
    [SerializeField] private Color getNotDamageColor;
    private List<Color> originialColor = new List<Color>();

    private enum BossPhaseAttack
    {
        None,
        Phase1_Attack1,
        Phase1_Attack2,
        Phase1_Attack3,
        Phase2_Attack1,
        Phase2_Attack2,
    };
    [SerializeField] private BossPhaseAttack bossPhaseAttack;

    private string nameAnimatorInteger_AttackType = "AttackType";
    private string nameAnimatorTrigger_DoAgain = "DoAgain";
    private string nameAnimatorTrigger_Roar = "Roar";

    //private string nameAnimatorTrigger_RockType = "RockType";
    private string nameAnimatorBool_Phase2 = "Phase2";
    private string nameAnimatorBool_OnStompWave = "OnStompWave";
    private string nameAnimatorBool_ContinueAttack = "ContinueAttack";

    
    #region Phase 1 Attack 1 variable required
    [Header("Phase 1 Attack 1 variables :")]
    //[SerializeField] protected float Phase1_Attack1_ChargeTime;
    private Vector2 enemyTowardsPositon;
    #endregion

    #region Phase 1 Attack 2 variable required 
    [Header("Phase 1 Attack 2 variables :")]
    //[SerializeField] protected float Phase1_Attack2_ChargeTime;
    //[SerializeField] protected float Phase1_Attack2_TemporarilyStomWaveColdDown;
    //protected float temporarilyStomWaveColdDownTimer = 0.0f;
    [SerializeField] private GameObject wavePrefab;
    private List<GameObject> waveGameObjects = new List<GameObject>();
    private Vector2 waveSpawnOriginPosition;
    //[SerializeField] private Vector2 stompWaveMaxSize;
    //[SerializeField] private Vector2 stompWaveMinSize;
    //private Vector2 stompWaveCurrentSize;
    //private Vector2 timeForStomWaveSize;
    //[SerializeField] private Vector2 StomWaveSizeXExpansionSpeed;
    [SerializeField] private int maxNumberOfStomWave;
    private int currentNumberOfStomWave;
    //private Vector2 stomWavePosition;
    #endregion
    
    #region Phase 1 Attack 3 variable required
    [Header("Phase 1 Attack 3 variables :")]
    //[SerializeField] protected float Phase1_Attack3_ChargeTime;
    private List<Vector2> rockfallFixedDifferentAppearPosition = new List<Vector2>();
    private List<Vector2> tempRockfallFixedDifferentAppearPosition = new List<Vector2>();
    //[SerializeField] protected float Phase1_Attack3_TemporarilyRockfallsColdDown;
    //protected float temporarilyRockfallsColdDownTimer = 0.0f;
    [SerializeField] private List<GameObject> rockfallPrefabs;
    [SerializeField] private int eachRoundHowManyRockfalls;
    [SerializeField] private int maxNumberOfRockfalls;
    private int currentNumberOfRockfalls;
    private List<GameObject> rocksOfFalls = new List<GameObject>();
    private int indexOfWhichRockIsFalling;
    [SerializeField] private float eachRockOfFallIntervalTime;
    private float eachRockOfFallIntervalTimeTimer;
    #endregion
    
    #region Phase 2 Attack 1 variable required
    [Header("Phase 2 Attack 1 variables :")]
    //[SerializeField] protected float Phase2_Attack1_ChargeTime;
    [SerializeField] private GameObject enemyEtherealArmourPrefab;
    private GameObject enemyEtherealArmourGameObject;
    private bool isEtherealArmourFinish = false;
    //[SerializeField] private Transform enemyEtherealArmourRaycastOriginPoint;
    //private float enemyEtherealArmourRaycastCheckWallDistance = 0.1f;
    private Transform enemyEtherealArmourTransform;
    //[SerializeField] private Vector2 checkEnemyEtherealArmourHitPlayerBoxSize;
    #endregion

    #region Phase 2 Attack 2 variable required
    [Header("Phase 2 Attack 2 variables :")]
    //[SerializeField] protected float phase2_Attack2_ChargeTime;
    //[SerializeField] protected float phase2_Attack2_TemporarilyStomWaveColdDown;
    //[SerializeField] private Vector2 stompWaveMaxSizePhase2;
    //[SerializeField] private Vector2 stompWaveMinSizePhase2;
    //private Vector2 stompWaveCurrentSizePhase2;
    //private Vector2 timeForStomWaveSizePhase2;
    //[SerializeField] private Vector2 StomWaveSizeXExpansionSpeedPhase2;
    [SerializeField] private int maxNumberOfStomWaveAndRockfalls;
    private int currentNumberOfStomWaveAndRockfalls;
    [SerializeField] private float eachRockOfFallIntervalPhase2Time;
    private float eachRockOfFallIntervalPhase2TimeTimer;
    private bool onStompWave = true;
    //[Space(15f)]
    //[SerializeField] protected float phase2_Attack2_TemporarilyRockfallsColdDown;
    //[SerializeField] private float eachRockOfFallIntervalTimePhase2;
    #endregion

    /* For refer
        Phase 1
            Charges towards player
            Stomp the ground to perform attack wave
            Stomp the ground and make rocks fall from the top
        Phase 2
            Charges towards player with projection of it’s ethereal armour before following up with the actual Rhino
            Stomps the ground - Combo of attack wave and rocks falling
    */

    protected override void Initialise()
    {
        isAwayChangePlayer = false;
        base.Initialise();
        terrainLayer = LayerMask.GetMask("Terrain");
        enemyTransform = transform;
        //originialColor = spriteRenderer.color;
        //enemyEtherealArmourPrefab = Instantiate(enemyEtherealArmourPrefab, colliderTransform.position, enemyTransform.rotation);
        //enemyEtherealArmourRaycastOriginPoint = enemyEtherealArmourPrefab.GetComponentInChildren<Transform>();
        //enemyEtherealArmourTransform = enemyEtherealArmourPrefab.GetComponent<Transform>();

        //enemyTransform.rotation = Quaternion.Euler(enemyTransform.rotation.x, facingRight ? 180.0f : 0.0f, enemyTransform.rotation.z);

        for (int i=0; i < eachRoundHowManyRockfalls; i++)
        {
            //rocksOfFall[i] = Instantiate(rockfallPrefab, Vector2.zero, Quaternion.identity); //Same as this, for others to refer
            rocksOfFalls.Add(Instantiate(rockfallPrefabs[Random.Range(0, 3)], Vector2.zero, Quaternion.identity));
            rocksOfFalls[i].SetActive(false);
        }

        Vector2 tempValue;
        for (int i = 0; i < eachRoundHowManyRockfalls; i++)
        {
            tempValue.x = rockfallSourceOriginPoint.position.x - halfLengthOfRockfallAppearField;
            tempValue.x = tempValue.x + (((rockfallSourceOriginPoint.position.x + halfLengthOfRockfallAppearField) - (rockfallSourceOriginPoint.position.x - halfLengthOfRockfallAppearField)) / (eachRoundHowManyRockfalls-1)) * i;
            tempValue.y = rockfallSourceOriginPoint.position.y;
            rockfallFixedDifferentAppearPosition.Add(tempValue);
        }

        for (int i=0; i < maxNumberOfStomWave; i++)
        {
            waveGameObjects.Add(Instantiate(wavePrefab, Vector2.zero, Quaternion.identity));
            waveGameObjects[i].gameObject.SetActive(false);
        }
        GameObject tempGameObject = Instantiate(enemyEtherealArmourPrefab, Vector2.zero, Quaternion.identity);
        enemyEtherealArmourGameObject = tempGameObject;
        enemyEtherealArmourTransform = enemyEtherealArmourGameObject.transform;
        enemyEtherealArmourGameObject.GetComponent<RhinoEtherealArmourController>().rhinoAI = this.GetComponent<RhinoAI>();
        enemyEtherealArmourGameObject.SetActive(false);

        foreach (SpriteRenderer tempSpriteRenderer in headGameObjectOfAllSpriteRenderers.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderers.Add(tempSpriteRenderer);
            originialColor.Add(tempSpriteRenderer.color);
        }

        tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);
    }

    protected override void EnemyPatrol()
    {
        if (ableDoEnemyState)
        {
            if (rhinoPre_WarActionTimeTimer.Equals(0.0f))
            {
                //pre-war animation run here
                //spriteRenderer.color = Color.blue;
                animator.SetTrigger(nameAnimatorTrigger_Roar);
            }

            if (rhinoPre_WarActionTimeTimer >= rhinoPre_WarActionTime)
            {
                //spriteRenderer.color = originialColor;

                enemyState = EnemyState.ENEMY_CHASING;

            }
            else
            {
                rhinoPre_WarActionTimeTimer += 1.0f * Time.deltaTime;
            }
        }
    }

    protected override void EnemyChase()
    {
        //onGround = Physics2D.Raycast(raycastOriginPoint.position, Vector2.down, raycastCheckGroundDistance, terrainLayer);
        if(currentHealth < (int)(maximumHealth / 2) && onPhase2 == false)
        {
            onPhase2 = true;
        }
        

        if (onPhase2.Equals(false))
        {
            bossPhaseAttack = (BossPhaseAttack)Random.Range((int)BossPhaseAttack.Phase1_Attack1, (int)BossPhaseAttack.Phase1_Attack3 + 1);
        }
        else
        {
            bossPhaseAttack = (BossPhaseAttack)Random.Range((int)BossPhaseAttack.Phase2_Attack1, (int)BossPhaseAttack.Phase2_Attack2 + 1);
            Debug.Log(bossPhaseAttack);
        }

        animator.SetBool(nameAnimatorBool_Phase2, onPhase2);

        enemyState = EnemyState.ENEMY_ATTACKING;

    }

    protected override void EnemyAttack()
    {
        //if (onPhase2 == false)//this if else statement for double comfirm and for understand how it work only, you can keep using it or delete it and put the down and inside else"case BossPhaseAttack.Phase2_Attack1:" until "break;" and "case BossPhaseAttack.Phase2_Attack2:" until "break;" to upside, it won't happen any issue.
        {
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
    }

    private void Phase1_FirstAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;

            //for testing
            //spriteRenderer.color = Color.yellow;
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
                    //for testing
                    //spriteRenderer.color = originialColor;

                    bossPhaseAttack = BossPhaseAttack.None;
                    animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);

                    enemyState = EnemyState.ENEMY_RESTING;

                    // set values back to defalut
                    isFinishPhaseCharge = false;
                    onPreAttackAnimationFinish = false;
                    return;
                }
            }
        }
    }

    private void Phase1_SecondAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            animator.SetBool(nameAnimatorBool_ContinueAttack, true);

            //for testing
            //spriteRenderer.color = Color.green;
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
                animator.SetBool(nameAnimatorBool_ContinueAttack, false);
                //for testing
                //spriteRenderer.color = originialColor;
                //rigidbody2D.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);

                bossPhaseAttack = BossPhaseAttack.None;
                animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);

                // set values back to defalut
                //phaseChargeTimeTimer = temporarilyStomWaveColdDownTimer = 0.0f;

                enemyState = EnemyState.ENEMY_RESTING;

                currentNumberOfStomWave = 0;
                isFinishPhaseCharge = false;
                onPreAttackAnimationFinish = false;
                return;
            }
        }
    }

    private void Phase1_ThirdAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            animator.SetBool(nameAnimatorBool_ContinueAttack, true);

            //for testing
            //spriteRenderer.color = Color.red;
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
                    //rocksOfFalls[indexOfWhichRockIsFalling].transform.TransformPoint();
                    rocksOfFalls[indexOfWhichRockIsFalling].transform.SetPositionAndRotation(tempRockfallFixedDifferentAppearPosition[whichPosition], Quaternion.identity);

                    rocksOfFalls[indexOfWhichRockIsFalling].SetActive(true);

                    tempRockfallFixedDifferentAppearPosition.RemoveAt(whichPosition);// remove the temp position at which Position, so next rockfall won't be same postion after a round

                    if (indexOfWhichRockIsFalling.Equals(eachRoundHowManyRockfalls - 1))
                    {
                        // set values back to defalut
                        indexOfWhichRockIsFalling = 0;
                        tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);// reset back the temp position

                        currentNumberOfRockfalls++;
                        onPreAttackAnimationFinish = false;

                        if (currentNumberOfRockfalls >= (maxNumberOfRockfalls - 1))
                        {
                            animator.SetBool(nameAnimatorBool_ContinueAttack, false);
                        }
                    }
                    else
                    {
                        indexOfWhichRockIsFalling++;
                    }
                }

            }

            if (currentNumberOfRockfalls >= maxNumberOfRockfalls)
            {
                
                //for testing
                //spriteRenderer.color = originialColor;
                //rigidbody2D.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);

                bossPhaseAttack = BossPhaseAttack.None;
                animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);

                enemyState = EnemyState.ENEMY_RESTING;

                // set values back to defalut
                currentNumberOfRockfalls = 0;
                isFinishPhaseCharge = false;
                onPreAttackAnimationFinish = false;
                return;
            }
        }
    }

    private void Phase2_FirstAttackMode()
    {
        if (!isFinishPhaseCharge)
        {

            //call animation here
            animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            isEtherealArmourFinish = false;

            //for testing
            //spriteRenderer.color = Color.black;
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
                    //for testing
                    //spriteRenderer.color = originialColor;

                    bossPhaseAttack = BossPhaseAttack.None;
                    animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);

                    enemyState = EnemyState.ENEMY_RESTING;

                    // set values back to defalut
                    isFinishPhaseCharge = false;
                    onPreAttackAnimationFinish = false;
                    isEtherealArmourFinish = false;
                    return;
                }
            }
        }
    }

    private void Phase2_SecondAttackMode()
    {
        if (!isFinishPhaseCharge)
        {
            //call animation here
            animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);
            isFinishPhaseCharge = true;
            onPreAttackAnimationFinish = false;
            onStompWave = false;
            animator.SetBool(nameAnimatorBool_OnStompWave, onStompWave);
            animator.SetBool(nameAnimatorBool_ContinueAttack, true);

            //for testing
            //spriteRenderer.color = Color.cyan;
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
                        //rocksOfFalls[indexOfWhichRockIsFalling].transform.TransformPoint();
                        rocksOfFalls[indexOfWhichRockIsFalling].transform.SetPositionAndRotation(tempRockfallFixedDifferentAppearPosition[whichPosition], Quaternion.identity);

                        rocksOfFalls[indexOfWhichRockIsFalling].SetActive(true);

                        tempRockfallFixedDifferentAppearPosition.RemoveAt(whichPosition);// remove the temp position at which Position, so next rockfall won't be same postion after a round

                        if (indexOfWhichRockIsFalling.Equals(eachRoundHowManyRockfalls - 1))
                        {
                            // set values back to defalut
                            indexOfWhichRockIsFalling = 0;
                            tempRockfallFixedDifferentAppearPosition.AddRange(rockfallFixedDifferentAppearPosition);// reset back the temp position
                            
                            onPreAttackAnimationFinish = false;
                            onStompWave = true;
                            animator.SetBool(nameAnimatorBool_OnStompWave, onStompWave);
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
                    animator.SetBool(nameAnimatorBool_OnStompWave, onStompWave);

                    currentNumberOfStomWaveAndRockfalls++;
                }
            }

            if (currentNumberOfStomWaveAndRockfalls >= maxNumberOfStomWaveAndRockfalls)
            {
                animator.SetBool(nameAnimatorBool_ContinueAttack, false);
                //for testing
                //spriteRenderer.color = originialColor;
                //rigidbody2D.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);

                bossPhaseAttack = BossPhaseAttack.None;
                animator.SetInteger(nameAnimatorInteger_AttackType, (int)bossPhaseAttack);

                enemyState = EnemyState.ENEMY_RESTING;

                // set values back to defalut
                currentNumberOfStomWaveAndRockfalls = 0;
                isFinishPhaseCharge = false;
                onPreAttackAnimationFinish = false;
                return;
            }

        }
    }
    /*
    protected override void EnemyRest()
    {
        StartCoroutine(Resting());
    }

    private IEnumerator Resting()
    {
        yield return new WaitForSeconds(5.0f);
        //enemyState = EnemyState.ENEMY_CHASING;
    }
    */
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

    public void RedoAnimationAgain() {

        if(bossPhaseAttack != BossPhaseAttack.None && bossPhaseAttack != BossPhaseAttack.Phase2_Attack2)
        {
            animator.SetTrigger(nameAnimatorTrigger_DoAgain);
        }
        /*
        if(bossPhaseAttack == BossPhaseAttack.Phase2_Attack2 && onStompWave == false)
        {
            Debug.Log(nameof(onStompWave) + onStompWave);
            animator.SetTrigger(nameAnimatorTrigger_DoAgain);
        }
        */
    }

    public override void DamageEnemyMelee()
    {
        if (enemyState == EnemyState.ENEMY_RESTING)
        {
            if (!spiritArmour.activeSelf)
            {
                if (this.currentHealth > 0)
                {
                    this.currentHealth -= 1;
                    StopAllCoroutines();
                    StartCoroutine(DamageColorChange());
                }

            }
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(NotDamageColorChange());
        }
    }

    protected override void DamageEnemySpirit()
    {
        if(enemyState == EnemyState.ENEMY_RESTING)
        {
            if (this.currentHealth > 0)
            {
                this.currentHealth -= 2;
                StopAllCoroutines();
                StartCoroutine(DamageColorChange());
            }
            
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(NotDamageColorChange());
        }
    }

    protected override void EnemyDieSound()
    {
        SoundManagerScripts.PlaySound("rhino_die_sound");
    }

    private IEnumerator DamageColorChange()
    {
        foreach (SpriteRenderer tempSpriteRenderer in spriteRenderers)
        {
            tempSpriteRenderer.color = getDamageColor;
        }
        yield return new WaitForSeconds(0.1f);
        for (int i=0; i < spriteRenderers.Count; i++)
        {
                spriteRenderers[i].color = originialColor[i];
        }
        yield return null;
    }
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

    private void OnDestroy()
    {
        //For clear residuary rockfalls when the boss is die, because the player only can fight this boss one time, if the player still can fight again this boss then delete this whole OnDestroy() function.
        for (int i = 0; i < eachRoundHowManyRockfalls; i++)
        {
            Destroy(rocksOfFalls[i]);
            Debug.Log("delete rocks fall");
        }
        for (int i = 0; i < maxNumberOfStomWave; i++)
        {
            Destroy(waveGameObjects[i]);
            Debug.Log("delete rhino wave");
        }
        Destroy(enemyEtherealArmourGameObject);
        Debug.Log("delete rhino enemy ethereal armour");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (this.colliderTransform != null)
        {
            //Gizmos.DrawWireCube(this.colliderTransform.position, playerDetectionRange); // do if check != NULL
            //Gizmos.DrawWireSphere(this.colliderTransform.position, attackRange);
        }
        /*
        // For see normal detector
        Gizmos.DrawRay(raycastOriginPoint.position, Vector2.down * raycastCheckGroundDistance);
        Gizmos.DrawRay(raycastOriginPoint.position, transform.right * raycastCheckWallDistance);

        // For see the line of fallrock length of Cave top 
        if(onPhase2 == false)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(rockfallSourceOriginPoint.position, Vector2.left * halfLengthOfRockfallAppearField);
            Gizmos.DrawRay(rockfallSourceOriginPoint.position, Vector2.right * halfLengthOfRockfallAppearField);

            // For see the Box size of stom wave
            Gizmos.color = Color.green;
            stomWavePosition.x = enemyTransform.position.x;
            stomWavePosition.y = enemyTransform.position.y + (stompWaveCurrentSize.y / 2);
            Gizmos.DrawWireCube(stomWavePosition, stompWaveCurrentSize);
        }
        else
        {
        
            // For see the Box size of Enemy Ethereal Armour
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(enemyEtherealArmourTransform.position, checkEnemyEtherealArmourHitPlayerBoxSize);
        
            // For see the Box size of phase 2 stom wave
            Gizmos.color = Color.cyan;
            stomWavePosition.x = enemyTransform.position.x;
            stomWavePosition.y = enemyTransform.position.y + (stompWaveCurrentSizePhase2.y / 2);
            Gizmos.DrawWireCube(stomWavePosition, stompWaveCurrentSizePhase2);
            Gizmos.DrawRay(rockfallSourceOriginPoint.position, Vector2.left * halfLengthOfRockfallAppearField);
            Gizmos.DrawRay(rockfallSourceOriginPoint.position, Vector2.right * halfLengthOfRockfallAppearField);
            */
        //}
        
    }

}
