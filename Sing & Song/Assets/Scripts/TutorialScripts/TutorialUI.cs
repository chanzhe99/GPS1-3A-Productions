using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUIGameObject;
    [SerializeField] private Animator tutorialUIAnimator;

    public void ShowTutorialUI()
    {
        tutorialUIGameObject.SetActive(true);
        tutorialUIAnimator.SetBool("IsShowing", true);
        StopAllCoroutines();
        StartCoroutine(HideWhenArrowKeyPressed());
    }

    private IEnumerator HideWhenArrowKeyPressed()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                break;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                break;
            }
            yield return null;
        }

        
        yield return new WaitForSeconds(1.5f);
        tutorialUIAnimator.SetBool("IsShowing", false);
    }

    public void FadeInTutorialUI() // didn't use yet
    {
        tutorialUIGameObject.SetActive(false);
        this.enabled = false;
    }

}
