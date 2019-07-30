using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SingScript playerScript;
    public GameObject songGameObject;
    private List<CheckPoint> checkPoints;
    private SaveData.PlayerSpawnData playerSpawnData;
    private SaveData.TutorialData tutorialData;
    private SaveData.OpeningCutsceneData openingCutsceneData;

    private int currentPointIndex = -1;
    public int CurrentPointIndex { set { /*currentPointIndex = (value < 0 || value > FindObjectsOfType<CheckPoint>().Length) ? -1 : value;*/ currentPointIndex = value; } get { return currentPointIndex; } }

    private int lastCheckPointLevelIndex = 1;
    public int LastCheckPointLevelIndex { set { lastCheckPointLevelIndex = value; } get { return lastCheckPointLevelIndex; } }

    private bool isTutorialMoviePlayed = false;
    public bool IsTutorialMoviePlayed { set { isTutorialMoviePlayed = value; } get { return isTutorialMoviePlayed; } }

    private bool isOpeningCutsceneMoviePlayed = false;
    public bool IsOpeningCutsceneMoviePlayed { set { isOpeningCutsceneMoviePlayed = value; } get { return isOpeningCutsceneMoviePlayed; } }

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SaveDataManager.DeleteData(Global.pathOfData_TutorialData);
            }
        }
        else if (Input.GetKey(KeyCode.P))
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SaveDataManager.DeleteData(Global.pathOfData_PlayerSpawnData);

            }
        }
        else if (Input.GetKey(KeyCode.O))
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SaveDataManager.DeleteData(Global.pathOfData_OpeningCutsceneData);

            }
        }
    }

    private void Awake()
    {
        if (Global.gameManager == null) { Global.gameManager = this; }
        else { Destroy(gameObject); return; }

        checkPoints = new List<CheckPoint>();
        checkPoints.AddRange(FindObjectsOfType<CheckPoint>());

        playerSpawnData = new SaveData.PlayerSpawnData();
        playerSpawnData = (SaveData.PlayerSpawnData)SaveDataManager.LoadDataGetObject(Global.pathOfData_PlayerSpawnData);
        if (playerSpawnData != null)
        {
            DefinePlayerLocation();
            lastCheckPointLevelIndex = playerSpawnData.lastCheckPointLevelIndex;
        }

        tutorialData = new SaveData.TutorialData();
        tutorialData = (SaveData.TutorialData)SaveDataManager.LoadDataGetObject(Global.pathOfData_TutorialData);
        if (tutorialData != null)
        {
            isTutorialMoviePlayed = tutorialData.isTutorialMoviePlayed;
        }

        openingCutsceneData = new SaveData.OpeningCutsceneData();
        openingCutsceneData = (SaveData.OpeningCutsceneData)SaveDataManager.LoadDataGetObject(Global.pathOfData_OpeningCutsceneData);
        if (openingCutsceneData != null)
        {
            isOpeningCutsceneMoviePlayed = openingCutsceneData.isOpeningCutsceneMoviePlayed;
        }
    }

    private void DefinePlayerLocation()
    {
        for(int i = 0; i < checkPoints.Count; i++)
        {
            if(checkPoints[i].PointIndex == playerSpawnData.currentPointIndex)
            {
                playerScript.GetComponent<Transform>().position = checkPoints[i].PlayerSpawnPosition;
                songGameObject.GetComponent<Transform>().position = checkPoints[i].PlayerSpawnPosition;
                break;
            }
            
        }
        
    }

    public void SaveAllGameDatas()
    {
        playerSpawnData = new SaveData.PlayerSpawnData(currentPointIndex, lastCheckPointLevelIndex);
        SaveDataManager.SaveData(playerSpawnData, Global.pathOfData_PlayerSpawnData);

        tutorialData = new SaveData.TutorialData(isTutorialMoviePlayed);
        SaveDataManager.SaveData(tutorialData, Global.pathOfData_TutorialData);

        openingCutsceneData = new SaveData.OpeningCutsceneData(isOpeningCutsceneMoviePlayed);
        SaveDataManager.SaveData(openingCutsceneData, Global.pathOfData_OpeningCutsceneData);
    }
}

