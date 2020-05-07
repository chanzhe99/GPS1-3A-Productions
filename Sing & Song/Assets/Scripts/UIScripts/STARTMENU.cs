using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class STARTMENU : MonoBehaviour
{
    private bool calledmenu = false;
    //[SerializeField] private GameObject startMenuPanelGameObject;
    private bool menuActiveOnStart;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueButtonText;
    [SerializeField] private Color continueButtonNotInteractableTextColor;
    private Color continueButtonTextDefualtColor;

    private string name_PlayerPrefs_OnPlayNewGameState = "OnPlayNewGameState";
    private string name_PlayerPrefs_OnPlayerRevive = "OnPlayerRevive";

    private string nameAnimatorTrigger_StartFading = "StartFading";

    private OpeningTimelineController openingTimelineController;

    private void Start()
    {
        openingTimelineController = FindObjectOfType<OpeningTimelineController>();


        // Set continue button interatable depend on the 
        continueButtonTextDefualtColor = continueButtonText.color;
        
        if (!Global.gameManager.menuStateData.IsOnNewGame || Global.gameManager.openingCutsceneData.IsOpeningCutsceneMoviePlayed)
        {
            continueButton.interactable = true;
            continueButtonText.color = continueButtonTextDefualtColor;
        }
        else
        {
            continueButton.interactable = false;
            continueButtonText.color = continueButtonNotInteractableTextColor;
        }
        
        player.GetComponent<SingScript>().CanDoAction = false;


        // Check the menu state
        if(Global.gameManager.menuStateData.IsJustStartGame)
        {
            Time.timeScale = 1.0f;
            menuActiveOnStart = true;

            Global.gameManager.menuStateData.SetIsJustStartGame(false);
        }
        else if (Global.gameManager.playerSpawnData.OnPlayerRevive)
        {
            Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TrasitionFade, false);
            Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.TrasitionFade, true, Global.gameStartFadeInSpeed);

            inGameUIGameObject.SetActive(true);
            FindObjectOfType<SingScript>().CanDoAction = true;
            Time.timeScale = 1.0f;
            menuActiveOnStart = false;
            Global.gameManager.playerSpawnData.SetOnPlayerRevive(false);
            Global.gameManager.SaveAllGameDatas();
        }
        else if (Global.gameManager.menuStateData.IsOnNewGame && !Global.gameManager.menuStateData.IsReturnToMainMenu)
        {
            Time.timeScale = 1.0f;
            menuActiveOnStart = false;
            if (!Global.gameManager.openingCutsceneData.IsOpeningCutsceneMoviePlayed)
            {
                openingTimelineController.ShowOpeningDialogue();
            }
        }
        else
        {
            Global.gameManager.menuStateData.SetIsReturnToMainMenu(false);
            Global.gameManager.SaveAllGameDatas();

            menuActiveOnStart = true;
        }

        Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.StartMenuUI, menuActiveOnStart);

        if(!PlayerPrefs.HasKey(name_PlayerPrefs_OnPlayerRevive) && !PlayerPrefs.HasKey(name_PlayerPrefs_OnPlayNewGameState))
        {
            Debug.Log(name_PlayerPrefs_OnPlayerRevive + " and " + name_PlayerPrefs_OnPlayNewGameState + " discard PlayerPrefs have deleted!");
        }
        else
        {
            Debug.Log(name_PlayerPrefs_OnPlayerRevive + " PlayerPrefs have delete!");
            PlayerPrefs.DeleteKey(name_PlayerPrefs_OnPlayerRevive);

            Debug.Log(name_PlayerPrefs_OnPlayNewGameState + " PlayerPrefs have delete!");
            PlayerPrefs.DeleteKey(name_PlayerPrefs_OnPlayNewGameState);
        }
        

        #region NotWant
        /*
        if (PlayerPrefs.HasKey(name_PlayerPrefs_OnPlayNewGameState))
        {
            //Debug.Log("OnPlayNewGameState = " + bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayNewGameState)));
            if (bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayNewGameState)))
            {
                //startMenuPanelGameObject.SetActive(false);
                menuActiveOnStart = false;
                OpeningTimelineController openingTimelineController = FindObjectOfType<OpeningTimelineController>();
                if (openingTimelineController != null)
                {
                    openingTimelineController.ShowOpeningDialogue();
                }

                PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "false");
            }
            else
            {
                menuActiveOnStart = true;
            }
        }
        else
        {
            menuActiveOnStart = true;

            PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "false");
        }

        if (PlayerPrefs.HasKey(name_PlayerPrefs_OnPlayerRevive))
        {
            if (bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayerRevive)))
            {
                Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TrasitionFade, false);
                Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.TrasitionFade, true, Global.gameStartFadeInSpeed);

                inGameUIGameObject.SetActive(true);
                FindObjectOfType<SingScript>().CanDoAction = true;
                Time.timeScale = 1.0f;
                menuActiveOnStart = false;
                //startMenuPanelGameObject.SetActive(false);
                PlayerPrefs.SetString(name_PlayerPrefs_OnPlayerRevive, "false");
            }
        }

        Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.StartMenuUI, menuActiveOnStart);

        if(!Global.gameManager.isTutorialMoviePlayed && !Global.gameManager.isOpeningCutsceneMoviePlayed)
        {
            Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.StartMenuUI, false);
            StartNewGame();
        }
        */
        #endregion

    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && calledmenu)
        {
            SceneManager.LoadScene(0);
        }
    }*/

    /*public void StartLoading()
    {
        Debug.Log("clicked");
        Globe.nextSceneName = "SampleScene";//level name
        SceneManager.LoadScene("Loading");//loading scene
    }*/

    public void BackMainMenu()
    {
        Time.timeScale = 0.0f;
        calledmenu = true;
    }

    public void ExitGame()
    {
        Application.Quit();
        //Debug.Log("Quit Game");
    }

    //this funtion in the option menu
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void Continue()
    {
        if (!Global.gameManager.tutorialData.IsTutorialMoviePlayed)
        {
            FindObjectOfType<TutorialManager>().ShowTutorialUI(TutorialManager.Index_ButtonNameOfTutorial.JumpButton);
        }
        player.GetComponent<SingScript>().CanDoAction = true;
        inGameUIGameObject.SetActive(true);
    }

    public void StartNewGame()
    {
        if (Global.gameManager.menuStateData.IsOnNewGame && !Global.gameManager.openingCutsceneData.IsOpeningCutsceneMoviePlayed)
        {
            Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.StartMenuUI, menuActiveOnStart);

            openingTimelineController.ShowOpeningDialogue();
            
        }
        else
        {
            //int lastCheckPointLevelIndex = Global.gameManager.playerSpawnData.LastCheckPointLevelIndex;
            Global.gameManager.ResetAllGameDatas();
            Global.gameManager.menuStateData.SetIsJustStartGame(false);
            //Global.gameManager.playerSpawnData.SetLastCheckPointLevelIndex(lastCheckPointLevelIndex);
            //Global.gameManager.menuStateData.SetIsJustStartGame(false);
            Global.gameManager.SaveAllGameDatas();

            //PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "true");
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void PlayerRevive()
    {
        Global.gameManager.playerSpawnData.SetOnPlayerRevive(true);
        Global.gameManager.SaveAllGameDatas();

        //PlayerPrefs.SetString(name_PlayerPrefs_OnPlayerRevive, "true");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    /*public void OnStartGame(string sceneName)
    {
        //Application.LoadLevel("menu");
        SceneManager.LoadScene(2);//1 is level 1
                                  // Application.LoadLevel(sceneName);
    }*/
}
