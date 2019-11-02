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
    [SerializeField] private GameObject startMenuPanelGameObject;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueButtonText;
    [SerializeField] private Color continueButtonNotInteractableTextColor;
    private Color continueButtonTextDefualtColor;
    [SerializeField] private Animator levelTransitionScreenFadingAnimator;
    private string name_PlayerPrefs_OnPlayNewGameState = "OnPlayNewGameState";
    private string name_PlayerPrefs_OnPlayerRevive = "OnPlayerRevive";

    private string nameAnimatorTrigger_StartFading = "StartFading";

    private void Start()
    {
        continueButtonTextDefualtColor = continueButtonText.color;

        if (Global.gameManager.IsTutorialMoviePlayed ==  true)
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
        
        if (PlayerPrefs.HasKey(name_PlayerPrefs_OnPlayNewGameState))
        {
            //Debug.Log("OnPlayNewGameState = " + bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayNewGameState)));
            if (bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayNewGameState)))
            {
                startMenuPanelGameObject.SetActive(false);
                OpeningTimelineController openingTimelineController = FindObjectOfType<OpeningTimelineController>();
                if (openingTimelineController != null)
                {
                    openingTimelineController.ShowOpeningDialogue();
                }

                PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "false");
            }
        }
        else
        {
            PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "false");
        }
        
        if (PlayerPrefs.HasKey(name_PlayerPrefs_OnPlayerRevive))
        {
            if (bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayerRevive)))
            {
                levelTransitionScreenFadingAnimator.SetTrigger(nameAnimatorTrigger_StartFading);
                inGameUIGameObject.SetActive(true);
                FindObjectOfType<SingScript>().CanDoAction = true;
                Time.timeScale = 1.0f;
                startMenuPanelGameObject.SetActive(false);
                PlayerPrefs.SetString(name_PlayerPrefs_OnPlayerRevive, "false");
            }
        }
        
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
        OpeningTimelineController tempOpeningTimelineController = FindObjectOfType<OpeningTimelineController>();

        if (tempOpeningTimelineController == null)
        {
            player.GetComponent<SingScript>().CanDoAction = true;
            inGameUIGameObject.SetActive(true);
        }
    }

    public void StartNewGame()
    {
        OpeningTimelineController tempOpeningTimelineController = FindObjectOfType<OpeningTimelineController>();

        if (tempOpeningTimelineController == null)
        {
            Global.gameManager.DeleteAllGameDatas();
            Global.gameManager.SaveAllGameDatas();

            PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "true");
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            tempOpeningTimelineController.ShowOpeningDialogue();
        }
    }

    public void PlayerRevive()
    {
        PlayerPrefs.SetString(name_PlayerPrefs_OnPlayerRevive, "true");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    /*public void OnStartGame(string sceneName)
    {
        //Application.LoadLevel("menu");
        SceneManager.LoadScene(2);//1 is level 1
                                  // Application.LoadLevel(sceneName);
    }*/
}
