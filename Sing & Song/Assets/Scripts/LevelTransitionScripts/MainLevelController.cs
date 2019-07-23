using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLevelController : MonoBehaviour
{

    private LevelTransitionController currentLevel;
    private int lastCheckPointLevelIndex = 1;
    [SerializeField] private Animator levelTransitionScreenFadingAnimator;

    public LevelTransitionController CurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }

    private void Start()
    {
        List<LevelTransitionController> tempLevelTransitionController = new List<LevelTransitionController>();
        tempLevelTransitionController.AddRange(FindObjectsOfType<LevelTransitionController>());

        for (int i=0; i < tempLevelTransitionController.Count; i++)
        {

            

            if (tempLevelTransitionController[i].LevelIndex == lastCheckPointLevelIndex)
            {
                tempLevelTransitionController[i].SetUpStart();
                tempLevelTransitionController[i].SetDisableToSwitch(true);
                currentLevel = tempLevelTransitionController[i];
                currentLevel.setChildrenActive(true);
            }
            else
            {
                tempLevelTransitionController[i].SetUpStart();
                tempLevelTransitionController[i].SetDisableToSwitch(false);
                tempLevelTransitionController[i].setChildrenActive(false);
                
                
            }
        }

    }

    public void SwitchToAnotherLevelAndStartScreenFading(LevelTransitionController tempLevelTransitionController)
    {
        currentLevel = tempLevelTransitionController;
        levelTransitionScreenFadingAnimator.SetTrigger("StartFading");
    }

    public void SwitchToAnotherLevel(LevelTransitionController tempLevelTransitionController)
    {
        currentLevel = tempLevelTransitionController;
    }

}
