using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLevelController : MonoBehaviour
{
    private LevelTransitionController currentLevel;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform singTransform;
    [SerializeField] private Transform songTransform;
    [SerializeField] private SingScript singScript;

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

            

            if (tempLevelTransitionController[i].LevelIndex == Global.gameManager.LastCheckPointLevelIndex)
            {
                tempLevelTransitionController[i].SetUpStart();
                tempLevelTransitionController[i].SetDisableToSwitch(true);
                currentLevel = tempLevelTransitionController[i];
                currentLevel.SetChildrenActive(true);
            }
            else
            {
                tempLevelTransitionController[i].SetUpStart();
                tempLevelTransitionController[i].SetDisableToSwitch(false);
                tempLevelTransitionController[i].SetChildrenActive(false);
                
                
            }
        }

    }

    public void SwitchToAnotherLevelAndStartScreenFading(LevelTransitionController tempLevelTransitionController, Vector3 tempPosition)
    {
        StopAllCoroutines();
        StartCoroutine(WaitFadingTrasition(tempLevelTransitionController, tempPosition));
    }

    private IEnumerator WaitFadingTrasition(LevelTransitionController tempLevelTransitionController, Vector3 tempPosition)
    {
        singScript.CanDoAction = false; // Don't let player control while level on transition

        // Fade In transition
        cameraController.enabled = false;
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.TrasitionFade, true, Global.gameLevelTransitionFadeInSpeed);

        // Set Sing and Song position to next level
        singTransform.position = tempPosition;
        songTransform.position = tempPosition;

        while (true)
        {
            // Don't let them fall down while his go thougt to next level
            singTransform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            songTransform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (!Global.userInterfaceActiveManager.MenusOnTrasition[(int)Global.MenusType.TrasitionFade]) // Wait until fade In transition finish, then just do fade out transition
            {
                currentLevel.SetChildrenActive(false); // Hide the current level all game objects
                currentLevel = tempLevelTransitionController; // Set the next level to current level
                currentLevel.SetChildrenActive(true); // Show the next level all game objects

                // Fade out transition
                cameraController.enabled = true;
                Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.TrasitionFade, false, Global.gameLevelTransitionFadeInSpeed);

                singScript.CanDoAction = true; // Let player control while level finish transition
                break;
            }

            yield return null;
        }
    }

    public void SwitchToAnotherLevel(LevelTransitionController tempLevelTransitionController)
    {
        currentLevel = tempLevelTransitionController;
    }

}
