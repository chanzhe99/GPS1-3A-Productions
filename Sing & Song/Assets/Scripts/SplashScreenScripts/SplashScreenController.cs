using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    private Animator splashScreenAnimator;
    
    private enum SplashState { KDU_Logo, ThreeA_Production_Logo };
    private SplashState splashState = SplashState.KDU_Logo;
    private void Start()
    {
        splashScreenAnimator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.anyKeyDown && splashState <= SplashState.ThreeA_Production_Logo)
        {
            switch (splashState)
            {
                case SplashState.KDU_Logo:
                    splashScreenAnimator.SetTrigger(Global.nameAnimatorTrigger_SplashScreen_PassKDU_Logo);
                    break;
                case SplashState.ThreeA_Production_Logo:
                    splashScreenAnimator.SetTrigger(Global.nameAnimatorTrigger_SplashScreen_Pass3A_Production_Logo);
                    break;
            }
            splashState++;
        }
    }

    public void EndSplashScreen()
    {
        SceneManager.LoadSceneAsync((int)Global.SceneIndex.Area1 , LoadSceneMode.Single);
    }
}
