using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLevelController : MonoBehaviour
{

    [HideInInspector] public LevelTransitionController currentLevel;
    private int lastCheckPointLevelIndex = 1;

    private void Start()
    {
        List<LevelTransitionController> tempLevelTransitionController = new List<LevelTransitionController>();
        tempLevelTransitionController.AddRange(FindObjectsOfType<LevelTransitionController>());

        for (int i=0; i < tempLevelTransitionController.Count; i++)
        {

            if (tempLevelTransitionController[i].LevelIndex == lastCheckPointLevelIndex)
            {
                currentLevel = tempLevelTransitionController[i];
                currentLevel.setChildrenActive(true);
            }
            else
            {
                tempLevelTransitionController[i].setChildrenActive(false);
            }
        }

    }


}
