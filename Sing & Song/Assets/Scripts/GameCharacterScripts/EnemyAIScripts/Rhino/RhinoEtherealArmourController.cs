using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoEtherealArmourController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [HideInInspector] public bool facingRight = false;
    [SerializeField] private Transform enemyEtherealArmourRaycastOrigin;
    private float detectWallRange = 0.3f;
    [SerializeField] private Vector2 detectPlayerVector2 = new Vector2(5.0f, 5.0f);
    [SerializeField] private Vector3 adjustDetectPlayerVector3Origin = new Vector3(3.72f, 0f, 0f);
    [HideInInspector] public RhinoAI rhinoAI;
    private Transform rhinoEtherealArmourTransform;
    private GameObject rhinoEtherealArmourGameOjebt;
    private Vector2 enemyTowardsPositon;
    [SerializeField] private float rhinoEtherealArmourAttackColdDown;
    private float rhinoEtherealArmourAttackTimeTimer;

    private bool isSkillHitWall;
    private bool isSkillHitPlayer;

    private LayerMask playerLayer;
    private LayerMask terrainLayer;

    private void Start()
    {
        rhinoEtherealArmourTransform = transform;
        rhinoEtherealArmourGameOjebt = gameObject;

        playerLayer = LayerMask.GetMask("Player");
        terrainLayer = LayerMask.GetMask("Terrain");
    }

    private void Update()
    {
        adjustDetectPlayerVector3Origin.x = facingRight ? -Mathf.Abs(adjustDetectPlayerVector3Origin.x) : Mathf.Abs(adjustDetectPlayerVector3Origin.x);
        isSkillHitPlayer = Physics2D.OverlapBox(enemyEtherealArmourRaycastOrigin.position + adjustDetectPlayerVector3Origin, detectPlayerVector2, 0.0f, playerLayer);
        isSkillHitWall = Physics2D.Raycast(enemyEtherealArmourRaycastOrigin.position, -this.transform.right, detectWallRange, terrainLayer);
        Debug.DrawRay(enemyEtherealArmourRaycastOrigin.position, -this.transform.right * detectWallRange);

        if (isSkillHitPlayer && rhinoEtherealArmourAttackTimeTimer == 0.0f)
        {
            FindObjectOfType<SingScript>().DamagePlayer(transform);
            rhinoEtherealArmourAttackTimeTimer += Time.deltaTime;
        }
        if (!isSkillHitWall)
        {
            enemyTowardsPositon.x = -moveSpeed;
            enemyTowardsPositon.y = 0.0f;
            rhinoEtherealArmourTransform.Translate(enemyTowardsPositon * Time.deltaTime);
        }
        else
        {
            enemyTowardsPositon.x = 0.0f;
            enemyTowardsPositon.y = 0.5f;
            rhinoEtherealArmourTransform.position = enemyTowardsPositon;

            rhinoAI.SetIsEtherealArmourFinishTrue();

            rhinoEtherealArmourGameOjebt.SetActive(false);
        }

        if(rhinoEtherealArmourAttackTimeTimer > rhinoEtherealArmourAttackColdDown)
        {
            rhinoEtherealArmourAttackTimeTimer = 0.0f;
        }
        else if(rhinoEtherealArmourAttackTimeTimer != 0.0f)
        {
            rhinoEtherealArmourAttackTimeTimer += Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(enemyEtherealArmourRaycastOrigin.position + adjustDetectPlayerVector3Origin, detectPlayerVector2);
    }

}
