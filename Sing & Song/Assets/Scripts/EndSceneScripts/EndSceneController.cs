using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    private Animator endSceneAnimator;
    [SerializeField] private float ableToRestartGameTime;
    private float ableToRestartGameTimeTimer;
    private bool isGetInputOneTime = false;

    // Start is called before the first frame update
    void Start()
    {
        endSceneAnimator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ableToRestartGameTimeTimer > ableToRestartGameTime)
        {
            if (Input.anyKey && !isGetInputOneTime)
            {
                endSceneAnimator.SetTrigger(Global.nameAnimatorTrigger_EndScene_FadeOut);
                isGetInputOneTime = true;
                //SceneManager.LoadSceneAsync((int)Global.SceneIndex.Splash, LoadSceneMode.Single);
            }
        }
        else
        {
            ableToRestartGameTimeTimer += Time.deltaTime;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync((int)Global.SceneIndex.Splash, LoadSceneMode.Single);
    }
}
