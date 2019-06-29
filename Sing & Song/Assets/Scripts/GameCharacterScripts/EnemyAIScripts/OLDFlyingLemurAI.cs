﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLDFlyingLemurAI : EnemyAI
{
    

    /*
    // TEMP NAME Other Variables
    private Vector2 lemurPerchPosition;
    private Vector2 lemurDefaultPosition;
    private bool lemurCanDive;
    [SerializeField] private float lemurAttackRange;
    private Vector2 playerCurrentPosition;
    [SerializeField] private float lemurAttackDiveSpeed;
    // Use this for initialization

    [SerializeField] private float lemurBackOffSpeed;

    private enum LemurState
    {
        LEMUR_CHASING,
        LEMUR_ATTACKING,
        LEMUR_RETREATING
    }
    private LemurState lemurState;

    protected override void OnAwake()
    {
        lemurPerchPosition = transform.position;
        lemurDefaultPosition.y = transform.position.y;
    }

    protected override void EnemyPatrol()
    {
        isDetectPlayerWhilePatrolling = Physics2D.OverlapBox(enemyColliderTransform.position, playerDetectionRange, 0f, playerLayer);
        if (isDetectPlayerWhilePatrolling)
        {
            enemyState = EnemyState.ENEMY_CHASING;
            return;
        }

        // Make Lemur go back to Spawn Position
        if(transform.position.x != lemurPerchPosition.x && transform.position.y != lemurPerchPosition.y && !enemyIsHit)
        {
            //enemyRigidbody2D.velocity = DirectionToTarget(enemyColliderTransform.position, lemurPerchPosition);
            transform.position = Vector2.MoveTowards(transform.position, lemurPerchPosition, moveSpeed * Time.deltaTime);
        }

        // Lemur does not move so no need Patrolling Movement
    }

    protected override void EnemyAttackBehavior()
    {
        switch (lemurState)
        {
            case LemurState.LEMUR_CHASING:
                LemurChasingBehavior();
                break;
            case LemurState.LEMUR_ATTACKING:
                LemurAttackingBehavior();
                break;
            case LemurState.LEMUR_RETREATING:
                LemurRetreatingBehavior();
                break;
        };
    }

    private void LemurChasingBehavior()
    {
        lemurCanDive = Physics2D.OverlapCircle(enemyColliderTransform.position, lemurAttackRange, playerLayer);

        if(lemurCanDive)
        {
            lemurDefaultPosition.x = transform.position.x; // Stores Lemur's X Position as for Retreating
            playerCurrentPosition = playerColliderTransform.position; // Stores PLayer's Position at the moment of Attacking
            lemurState = LemurState.LEMUR_ATTACKING; // Changes Lemur's State to Attacking
            return;
        }
        else
        {
            //enemyRigidbody2D.velocity = DirectionToTarget(enemyColliderTransform.position, playerColliderTransform.position);
            transform.position = Vector2.MoveTowards(transform.position, playerColliderTransform.position, moveSpeed * Time.deltaTime);
        }

        //yield return null;
    }

    private void LemurAttackingBehavior()
    {
        if(transform.position.x != playerCurrentPosition.x && transform.position.y != playerCurrentPosition.y && !enemyIsHit)
        {
            //enemyRigidbody2D.velocity = DirectionToTarget(enemyColliderTransform.position, playerCurrentPosition);
            transform.position = Vector2.MoveTowards(transform.position, playerCurrentPosition, lemurAttackDiveSpeed * Time.deltaTime);
        }
        else
        {
            //yield return new WaitForSecondsRealtime(1f);
            lemurState = LemurState.LEMUR_RETREATING;
            return;
        }

        //yield return null;
    }

    private void LemurRetreatingBehavior()
    {
        if (transform.position.x != lemurDefaultPosition.x && transform.position.y != lemurDefaultPosition.y && !enemyIsHit)
        {
            //enemyRigidbody2D.velocity = DirectionToTarget(enemyColliderTransform.position, lemurDefaultPosition);
            transform.position = Vector2.MoveTowards(transform.position, lemurDefaultPosition, lemurBackOffSpeed * Time.deltaTime);
        }
        else
        {
            lemurState = LemurState.LEMUR_CHASING;
            return;
        }

        //yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (enemyColliderTransform != null)
        {
            Gizmos.DrawWireCube(enemyColliderTransform.position, playerDetectionRange); // do if check != NULL
            Gizmos.DrawWireSphere(enemyColliderTransform.position, lemurAttackRange);
        }
    }

    */
}