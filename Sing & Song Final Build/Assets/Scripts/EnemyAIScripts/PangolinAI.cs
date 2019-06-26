using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class PangolinAI : EnemyAI
{
    /*
    #region ColdDownCount Class
    [System.Serializable]
    protected class ColdDownCount
    {
        private float timeCount;
        [SerializeField] private float coldDownTime;
        [SerializeField] private float defalutTime;

        private bool isTiming;

        public ColdDownCount(float startTime, float endTime)
        {
            defalutTime = startTime > endTime ? endTime : startTime;
            coldDownTime = startTime > endTime ? startTime : endTime;
        }

        public ColdDownCount(float coldDown)
        {
            Mathf.Abs(coldDown);
            defalutTime = 0.0f;
            coldDownTime = coldDown;
        }

        public bool counting()
        {
            timeCount += 1.0f * Time.deltaTime;
            isTiming = timeCount >= coldDownTime;
            return isTiming;
        }

        public bool isTimerValueIsDefault()
        {
            return timeCount == defalutTime;
        }

        public void resetTimer()
        {
            timeCount = defalutTime;
        }

        public bool getIsTiming()
        {
            return isTiming;
        }
    }
    #endregion

    
    //! Variable
    [Header("Timer Set Up"), SerializeField] private ColdDownCount timerStunning;
    [SerializeField] private ColdDownCount timerJumpRolling;
    [SerializeField] private ColdDownCount timerAttackColdDown;
    [Header("Check Objects Size"), SerializeField] private Vector2 checkGroundBoxSize;
    [SerializeField] private Vector2 checkWallBoxSize;
    [SerializeField] private float checkPrecipiceRange;
    [Header("Velocity And Force Value"), SerializeField] private float rollingSpeed;
    [SerializeField] private Vector2 jumpForceVector2;
    private float reverseSpeed;
    private float timeForReverseForce;

    [Header("Enemy State Boolen")] private bool onGround;
    private bool onCloseWalls;
    private bool onMovement;
    private bool isRolling;
    private bool isStunning;
    private bool facingRight;

    //! Component from parent Game Objects
    private Transform enemyTransfrom;

    //! Component from the child's Game Object
    [Header("Child GameObjects"), SerializeField, Space] private Transform enemyChildEyePointTransform;
    [SerializeField] private Transform enemyChildGroundPointTransform;
    [SerializeField] private Transform enemyChildPrecipicePointTransform;
    private Transform enemyChildSpriteAndColliderTransform;

    //! Component from others Game Object
    private Collider2D playerCollider2D;

    #region Name, Tag And Layer Mask
    private string namePangolinSpriteAndCollider = "SpriteAndCollider";
    private string namePangolinEyePoint = "EyePoint";
    private string namePangolinGroundPoint = "GroundPoint";

    private string layerMaskGround = "Terrain";
    private string layerMaskPlayer = "Player";
    private string layerMaskWall = "Wall";

    private string tagWall = "Wall";
    private string tagPlayer = "Player";
    #endregion

    enum EnemyStates { StatePatrolling, StateChasing, StateAttacking, StateStunning, StateIdle };

    private EnemyStates enemyStates;

    void Awake()
    {
        //Get the component pointer here
        //! GameObject Component
        enemyRigidbody2D = this.GetComponent<Rigidbody2D>();
        enemyTransfrom = this.transform;

        //! Child Component
        enemyChildSpriteAndColliderTransform = this.GetComponent<Transform>().Find(namePangolinSpriteAndCollider);
    }

    void Start()
    {
        //Set default value, text, or boolean here
        enemyStates = EnemyStates.StatePatrolling;
        onMovement = true;
        timeForReverseForce = 1.0f;

        //! For Start to moveRightOnAwake
        facingRight = moveRightOnAwake;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag(tagPlayer) && isRolling)
        {
            onMovement = false;
            isRolling = false;
            enemyRigidbody2D.velocity = Vector2.zero;
            jumpForceVector2.x = facingRight ? -Mathf.Abs(jumpForceVector2.x) : Mathf.Abs(jumpForceVector2.x);
            enemyRigidbody2D.AddForce(new Vector2(onGround ? jumpForceVector2.x: jumpForceVector2.x * 1.2f, jumpForceVector2.y), ForceMode2D.Impulse);

            enemyStates = EnemyStates.StateIdle;
            timerAttackColdDown.resetTimer();

            //player get Damage here
        }

    }

    protected override void EnemySwitchState()
    {
        //! Physics2D side here
        isDetectPlayerWhileChasing = playerCollider2D = Physics2D.OverlapBox(enemyChildGroundPointTransform.position, playerDetectionRange, 0.0f, LayerMask.GetMask(layerMaskPlayer));
        onCloseWalls = Physics2D.OverlapBox(enemyChildEyePointTransform.position, checkWallBoxSize, 0.0f, LayerMask.GetMask(layerMaskWall));
        onGround = Physics2D.OverlapBox(enemyChildGroundPointTransform.position, checkGroundBoxSize, 0.0f, LayerMask.GetMask(layerMaskGround));

        switch (enemyStates)
        {
            case EnemyStates.StatePatrolling:
                Movement();
                DetectPlayer();
                TrackPlayer();
                CloseWallToReverse();
                Flip();
                break;
            case EnemyStates.StateAttacking:
                StartJumpRolling();
                CloseWallToReverse();
                Flip();
                break;
            case EnemyStates.StateChasing:
                Movement();
                TrackPlayer();
                CloseWallToReverse();
                Flip();
                break;
            case EnemyStates.StateStunning:
                Stunning();
                break;
            case EnemyStates.StateIdle:
                TakeABreath();
                break;

        }
        base.EnemySwitchState();

    }

    private void DetectPlayer()
    {
        if (isDetectPlayer)
        {
            //Debug.Log("DetectedPlayer");
            onMovement = false;
            enemyStates = EnemyStates.StateAttacking;
        }

    }

    private void Movement()
    {
        if (onMovement)
        {
            if (!isRolling && onGround)
            {
                //Debug.Log("Patrolling");
                if (!Physics2D.Raycast(enemyChildPrecipicePointTransform.position, Vector2.down, checkPrecipiceRange, LayerMask.GetMask(layerMaskGround)))
                {
                    facingRight = !facingRight;
                    Flip();
                }
                enemyRigidbody2D.velocity = new Vector2(facingRight ? moveSpeed : -moveSpeed, enemyRigidbody2D.velocity.y);

            }
            else if (isRolling)
            {
                //Debug.Log("Rolling");
                if (timeForReverseForce < 1.0f) timeForReverseForce += 1.5f * Time.deltaTime;
                reverseSpeed = Mathf.Lerp(0.0f, facingRight ? rollingSpeed : -rollingSpeed, timeForReverseForce);
                enemyChildSpriteAndColliderTransform.Rotate(Vector3.forward, rollingSpeed * 5.0f);
                enemyRigidbody2D.velocity = new Vector2(reverseSpeed, enemyRigidbody2D.velocity.y);

            }

        }

    }

    private void TrackPlayer()
    {
        if (isDetectPlayer)
        {
            if (playerCollider2D.transform.position.x > enemyTransfrom.position.x)
            {
                //Debug.Log(!facingRight ? "Enemy turn right to chasing the player" : "Track player = Right");
                if (!facingRight) timeForReverseForce = 0.0f;
                facingRight = !facingRight ? !facingRight : facingRight;

            }
            else if (playerCollider2D.transform.position.x < enemyTransfrom.position.x)
            {
                //Debug.Log(facingRight ? "Enemy turn left to chasing the player" : "Track player = Left");
                if (facingRight) timeForReverseForce = 0.0f;
                facingRight = facingRight ? !facingRight : facingRight;

            }

        }

    }

    private void StartJumpRolling()
    {
        if (onGround)
        {
            if (!isRolling)
            {
                //Debug.Log("Jump to Rolling");
                isRolling = true;

                enemyRigidbody2D.velocity = Vector2.zero;
                jumpForceVector2.x = facingRight ? Mathf.Abs(jumpForceVector2.x) : -Mathf.Abs(jumpForceVector2.x);
                enemyRigidbody2D.AddForce(jumpForceVector2, ForceMode2D.Impulse);

            }

            if (timerJumpRolling.counting())
            {
                //Debug.Log("Set movement to true while enemy on ground after jumping");
                onMovement = true;
                enemyStates = EnemyStates.StateChasing;
                timerJumpRolling.resetTimer();

            }

        }

        enemyChildSpriteAndColliderTransform.Rotate(Vector3.forward, moveSpeed * 5.0f);

    }

    private void CloseWallToReverse()
    {
        if (onCloseWalls)
        {
            if (!isRolling)
            {
                facingRight = !facingRight;

            }
            else
            {
                Debug.Log("");

                onMovement = false;
                enemyRigidbody2D.velocity = Vector2.zero;

                jumpForceVector2.x = facingRight ? -Mathf.Abs(jumpForceVector2.x) : Mathf.Abs(jumpForceVector2.x);
                enemyRigidbody2D.AddForce(jumpForceVector2, ForceMode2D.Impulse);

                enemyStates = EnemyStates.StateStunning;
                timerStunning.resetTimer();

            }

        }

    }

    private void Flip()
    {
        enemyTransfrom.rotation = Quaternion.Euler(enemyTransfrom.rotation.x, facingRight ? 180.0f : 0.0f, enemyTransfrom.rotation.z);

    }

    private void Stunning()
    {
        if (onGround)
        {
            if (timerStunning.isTimerValueIsDefault())
            {
                enemyChildSpriteAndColliderTransform.localRotation = Quaternion.Euler(enemyChildSpriteAndColliderTransform.localRotation.x, enemyChildSpriteAndColliderTransform.localRotation.y, 0.0f);

            }
            if (timerStunning.counting())
            {
                onMovement = true;
                isRolling = false;
                enemyStates = EnemyStates.StatePatrolling;
                timerStunning.resetTimer();

            }

        }
        else
        {
            enemyChildSpriteAndColliderTransform.Rotate(Vector3.forward, -(rollingSpeed * 5.0f));
            timerStunning.resetTimer();

        }

    }

    private void TakeABreath()
    {
        if (onGround)
        {
            if (timerAttackColdDown.isTimerValueIsDefault())
            {
                enemyChildSpriteAndColliderTransform.localRotation = Quaternion.Euler(enemyChildSpriteAndColliderTransform.localRotation.x, enemyChildSpriteAndColliderTransform.localRotation.y, 0.0f);

            }
            if (timerAttackColdDown.counting())
            {
                if (isDetectPlayer)
                {
                    DetectPlayer();
                    TrackPlayer();
                    Flip();
                    enemyStates = EnemyStates.StateAttacking;

                }
                else
                {
                    onMovement = true;
                    isRolling = false;
                    enemyStates = EnemyStates.StatePatrolling;

                }

                timerAttackColdDown.resetTimer();

            }

        }
        else
        {
            enemyChildSpriteAndColliderTransform.Rotate(Vector3.forward, -(moveSpeed * 5.0f));
            timerAttackColdDown.resetTimer();

        }

    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(enemyChildEyePointTransform.position, checkWallBoxSize);
        Gizmos.DrawWireCube(enemyChildGroundPointTransform.position, checkGroundBoxSize);
        Gizmos.DrawRay(enemyChildPrecipicePointTransform.position, Vector2.down * checkPrecipiceRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(enemyChildEyePointTransform.position, playerDetectionRange);

    }
    */
}
