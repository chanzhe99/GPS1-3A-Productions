﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator checkPointUIBackgroundImageAnimator;
    [SerializeField] private ColdDownCount timerSavedPointRequest;
    [SerializeField] private Vector2 boxSizeVector2;
    [SerializeField] private int pointIndex;
    public int PointIndex
    {
        get
        {
            return pointIndex;
        }
    }
    [Header("Spawn when player die or load game")]
    [SerializeField] private Transform playerSpawnPosition;
    public Vector3 PlayerSpawnPosition
    {
        get
        {
            return playerSpawnPosition.position;
        }
    }

    private bool isPlayerInArea = false;
    private bool isPressingButton = false;
    private bool ableToSaveData = false;

    private string layerMaskPlayer = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    void Update()
    {
        isPlayerInArea = Physics2D.OverlapBox(transform.position, boxSizeVector2, 0.0f, LayerMask.GetMask(layerMaskPlayer));

        checkPointUIBackgroundImageAnimator.SetBool("PlayerInArea", isPlayerInArea ? true : false);
        checkPointUIBackgroundImageAnimator.SetBool("GameSaved", ableToSaveData ? false : true);

        if (Input.GetButtonDown("InteractButton"))// change the key to game save point button
        {
            isPressingButton = true;
        }
        if (Input.GetButtonUp("InteractButton"))// change the key to game save point button
        {
            isPressingButton = false;
        }

        if (isPlayerInArea && isPressingButton) // change the key to game save point button
        {

            CheckToSavingPoint();

        }
        else if(!isPlayerInArea)
        {
            ableToSaveData = true;
        }

    }

    private void CheckToSavingPoint()
    {

        if (ableToSaveData)
        {
            if (timerSavedPointRequest.CountingAndCheck()) //Save Point here
            {
                isPressingButton = false;
                ableToSaveData = false;

                Global.gameManager.CurrentPointIndex = pointIndex;
                Global.gameManager.SaveAllGameDatas();

                timerSavedPointRequest.ResetTimer();

            }

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSizeVector2);

    }

}
