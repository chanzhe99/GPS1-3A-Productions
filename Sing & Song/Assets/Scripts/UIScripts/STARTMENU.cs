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

    private void Start()
    {
        player.GetComponent<SingScript>().enabled = false;
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

    public void StartGame()
    {
        OpenningTimelineController tempOpenningTimelineController = FindObjectOfType<OpenningTimelineController>();

        if (tempOpenningTimelineController == null)
        {
            player.GetComponent<SingScript>().enabled = true;
        }
    }


    /*public void OnStartGame(string sceneName)
    {
        //Application.LoadLevel("menu");
        SceneManager.LoadScene(2);//1 is level 1
                                  // Application.LoadLevel(sceneName);
    }*/
}
