using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    private Animator splashScreenAnimator;
    private string nameAnimatorBool_PassSplashScreen = "PassSplashScreen";

    private void Start()
    {
        splashScreenAnimator = this.GetComponent<Animator>();
        splashScreenAnimator.SetBool(nameAnimatorBool_PassSplashScreen, false);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            splashScreenAnimator.SetBool(nameAnimatorBool_PassSplashScreen, true);
        }
    }

    public void EndSplashScreen()
    {
        SceneManager.LoadSceneAsync((int)Global.SceneIndex.Area1 , LoadSceneMode.Single);
    }
}
