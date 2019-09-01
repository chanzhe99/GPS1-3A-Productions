using System.Collections;
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

    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Escape))
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
        pauseMenuPanelGameObject.SetActive(false);
        paused = false;
        Time.timeScale = 1.0f;
    }

    public void Pause()
    {
        pauseMenuPanelGameObject.SetActive(true);
        paused = true;
        Time.timeScale = 0.0f;
    }

    public void QuitToMainMenu()
    {
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
