﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PAUSEMENU : MonoBehaviour
{
    private bool paused = false;
    [SerializeField] private GameObject pauseMenuPanelGameObject;
    [SerializeField] private GameObject pauseOptionMenuPanelGameObject;

    void Update()
    {
        if (Global.userInterfaceActiveManager.MenusVisibilityState[(int)Global.MenusType.StartMenuUI]) return;

        if (Input.GetButtonDown("PauseButton"))
        {
            if (!paused)
            {
                Pause();
                //Debug.Log("pause");
            }
            else
            {
                Resume();
                //Debug.Log("onStarting");
            }
            //Debug.Log("can detecd the key down");
        }
    }
    public void Resume()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.PauseMenuUI, false, 0.5f);

        paused = false;
        Time.timeScale = 1.0f;
    }

    public void Pause()
    {
        pauseMenuPanelGameObject.SetActive(true);
        pauseOptionMenuPanelGameObject.SetActive(false);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.PauseMenuUI, true, 0.3f);

        paused = true;
        Time.timeScale = 0.0f;
    }

    public void QuitToMainMenu()
    {
        FindObjectOfType<MainLevelController>().isAbleSwitchLevel = false;

        Global.gameManager.menuStateData.SetIsReturnToMainMenu(true);
        Global.gameManager.SaveAllGameDatas();

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    

   /* public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }*/
}
