using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isDeleteAllDataOnAwake = false;

    [SerializeField] private float endSceneTransitionTime;
    [SerializeField] private AudioSource audioSourceBGM;

    public SingScript playerScript;
    public GameObject singGameObject;
    public GameObject songGameObject;
    private List<CheckPoint> checkPoints;

    public SaveData.PlayerSpawnData playerSpawnData;
    public SaveData.TutorialData tutorialData;
    public SaveData.OpeningCutsceneData openingCutsceneData;
    public SaveData.RhinoBossData rhinoBossData;
    public SaveData.MenuStateData menuStateData;

    public int lastCheckPointLevelIndex = -1;

    /*
    public int currentPointIndex = -1;
    public int lastCheckPointLevelIndex = 1;
    public bool isTutorialMoviePlayed = false;
    public bool isOpeningCutsceneMoviePlayed = false;
    public bool isNotFirstTimeRunRhinoCutscene = false;
    */
    private void Awake()
    {
        if (isDeleteAllDataOnAwake)
        {
            Debug.LogWarning("<color=#FF0000># Important! You are tring to delete all saving datas on awake, make sure you remember turn off the boolean after testing!</color>");
            DeleteAllGameDatas();
        }

        Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TrasitionFade, true);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.TrasitionFade, false, Global.gameStartFadeInSpeed);

        if (Global.gameManager == null) { Global.gameManager = this; }
        else { Destroy(gameObject); return; }

        checkPoints = new List<CheckPoint>();
        checkPoints.AddRange(FindObjectsOfType<CheckPoint>());

        playerSpawnData = (SaveData.PlayerSpawnData)SaveDataManager.LoadDataGetObject(Global.pathOfData_PlayerSpawnData);
        if (playerSpawnData == null) { playerSpawnData = new SaveData.PlayerSpawnData(); }
        else { DefinePlayerLocation(); }

        tutorialData = (SaveData.TutorialData)SaveDataManager.LoadDataGetObject(Global.pathOfData_TutorialData);
        if (tutorialData == null) { tutorialData = new SaveData.TutorialData(); }

        openingCutsceneData = (SaveData.OpeningCutsceneData)SaveDataManager.LoadDataGetObject(Global.pathOfData_OpeningCutsceneData);
        if (openingCutsceneData == null) { openingCutsceneData = new SaveData.OpeningCutsceneData(); }

        rhinoBossData = (SaveData.RhinoBossData)SaveDataManager.LoadDataGetObject(Global.pathOfData_RhinoBossData);
        if (rhinoBossData == null) { rhinoBossData = new SaveData.RhinoBossData(); }

        menuStateData = (SaveData.MenuStateData)SaveDataManager.LoadDataGetObject(Global.pathOfData_MenuStateData);
        if (menuStateData == null) { menuStateData = new SaveData.MenuStateData(); }

        FindCheckPointLocalLevelIndex();

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SaveDataManager.DeleteData(Global.pathOfData_TutorialData);
            }
        }
        else if (Input.GetKey(KeyCode.P))
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SaveDataManager.DeleteData(Global.pathOfData_PlayerSpawnData);

            }
        }
        else if (Input.GetKey(KeyCode.O))
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SaveDataManager.DeleteData(Global.pathOfData_OpeningCutsceneData);

            }
        }
        else if (Input.GetKey(KeyCode.R))
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SaveDataManager.DeleteData(Global.pathOfData_RhinoBossData);
            }
        }
        else if (Input.GetKey(KeyCode.M))
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SaveDataManager.DeleteData(Global.pathOfData_MenuStateData);
            }
        }
    }

    private void FindCheckPointLocalLevelIndex()
    {
        if (playerSpawnData.CurrentPointIndex == -1)
        {
            lastCheckPointLevelIndex = 1;
            return;
        }

        List<CheckPoint> tempCheckPoint = new List<CheckPoint>();
        tempCheckPoint.AddRange(FindObjectsOfType<CheckPoint>());

        for (int i = 0; i < tempCheckPoint.Count; i++)
        {
            if (tempCheckPoint[i].PointIndex == playerSpawnData.CurrentPointIndex)
            {
                lastCheckPointLevelIndex = tempCheckPoint[i].CurrentLocalLevelIndex;
                return;
            }
        }

        lastCheckPointLevelIndex = 1;
    }

    private void DefinePlayerLocation()
    {
        if (playerSpawnData.CurrentPointIndex == -1) return;

        for (int i = 0; i < checkPoints.Count; i++)
        {
            if(checkPoints[i].PointIndex == playerSpawnData.CurrentPointIndex)
            {
                playerScript.GetComponent<Transform>().position = checkPoints[i].PlayerSpawnPosition;
                songGameObject.GetComponent<Transform>().position = checkPoints[i].PlayerSpawnPosition;

                break;
            }
            
        }
        
    }

    public void SaveAllGameDatas()
    {
        SaveDataManager.SaveData(playerSpawnData, Global.pathOfData_PlayerSpawnData);
        SaveDataManager.SaveData(tutorialData, Global.pathOfData_TutorialData);
        SaveDataManager.SaveData(openingCutsceneData, Global.pathOfData_OpeningCutsceneData);
        SaveDataManager.SaveData(rhinoBossData, Global.pathOfData_RhinoBossData);
        SaveDataManager.SaveData(menuStateData, Global.pathOfData_MenuStateData);
    }

    public void SaveRhinoBossData()
    {
        SaveDataManager.SaveData(rhinoBossData, Global.pathOfData_RhinoBossData);
    }

    public void SaveMenuStateData()
    {
        SaveDataManager.SaveData(menuStateData, Global.pathOfData_MenuStateData);
    }

    public void ResetAllGameDatas()
    {
        playerSpawnData = new SaveData.PlayerSpawnData();
        tutorialData = new SaveData.TutorialData();
        openingCutsceneData = new SaveData.OpeningCutsceneData();
        rhinoBossData = new SaveData.RhinoBossData();
        menuStateData = new SaveData.MenuStateData();
    }

    public void DeleteAllGameDatas()
    {
        SaveDataManager.DeleteData(Global.pathOfData_TutorialData);
        SaveDataManager.DeleteData(Global.pathOfData_PlayerSpawnData);
        SaveDataManager.DeleteData(Global.pathOfData_OpeningCutsceneData);
        SaveDataManager.DeleteData(Global.pathOfData_RhinoBossData);
        SaveDataManager.DeleteData(Global.pathOfData_MenuStateData);
    }

    public void TimeToEndAndSayThankYou()
    {
        playerScript.CanDoAction = false;
        playerScript.doAnimationFollowPlayerState = false;

        StopAllCoroutines();
        StartCoroutine(EndSceneTransitionTime());
    }

    private IEnumerator EndSceneTransitionTime()
    {
        //Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.TrasitionFade, true, Global.gameStartFadeInSpeed);

        yield return null;
        while (true)
        {
            audioSourceBGM.volume -= Time.deltaTime;
            if (audioSourceBGM.volume <= 0.0f)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(endSceneTransitionTime);
        SceneManager.LoadSceneAsync((int)Global.SceneIndex.EndScene, LoadSceneMode.Single);
    }

    private void OnApplicationQuit()
    {
        menuStateData.SetIsJustStartGame(true);
        SaveAllGameDatas();
    }

}

