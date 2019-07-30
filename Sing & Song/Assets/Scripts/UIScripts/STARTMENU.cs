using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    [SerializeField] private Button continueButton;
    [SerializeField] private Text continueButtonText;
    [SerializeField] private Color continueButtonNotInteractableTextColor;
    private Color continueButtonTextDefualtColor;
    private string name_PlayerPrefs_OnPlayNewGameState = "OnPlayNewGameState";

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

        player.GetComponent<SingScript>().canDoAction = false;
        
        if (PlayerPrefs.HasKey(name_PlayerPrefs_OnPlayNewGameState))
        {
            Debug.Log("OnPlayNewGameState = " + bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayNewGameState)));
            if (bool.Parse(PlayerPrefs.GetString(name_PlayerPrefs_OnPlayNewGameState)))
            {
                startMenuPanelGameObject.SetActive(false);
                OpenningTimelineController openningTimelineController = FindObjectOfType<OpenningTimelineController>();
                if (openningTimelineController != null)
                {
                    openningTimelineController.PlayOpeningMovie();
                }

                PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "false");
            }
        }
        else
        {
            PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "false");
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
        Debug.Log("Quit Game");
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
        OpenningTimelineController tempOpenningTimelineController = FindObjectOfType<OpenningTimelineController>();

        if (tempOpenningTimelineController == null)
        {
            player.GetComponent<SingScript>().canDoAction = true;
        }
    }

    public void StartNewGame()
    {
        OpenningTimelineController tempOpenningTimelineController = FindObjectOfType<OpenningTimelineController>();

        if (tempOpenningTimelineController == null)
        {
            Global.gameManager.DeleteAllGameDatas();
            Global.gameManager.SaveAllGameDatas();

            PlayerPrefs.SetString(name_PlayerPrefs_OnPlayNewGameState, "true");
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            tempOpenningTimelineController.PlayOpeningMovie();
        }

        
    }


    /*public void OnStartGame(string sceneName)
    {
        //Application.LoadLevel("menu");
        SceneManager.LoadScene(2);//1 is level 1
                                  // Application.LoadLevel(sceneName);
    }*/
}
