using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    public enum Index_ButtonNameOfTutorial { Horizontal, JumpButton, MeleeAttackButton };

    [SerializeField] private GameObject tutorialUIGameObject;
    [SerializeField] private Animator tutorialUIAnimator;
    [SerializeField] private Text tutorialText;
    [SerializeField] private GameObject singGameObject;
    private float closeDialogueTime = 0.1f;
    private Vector2 tutorialStartPosition;
    [SerializeField] private float tutorialMoveOutDisappearDistanceX = 5f;
    [SerializeField] private List<Image> keysImages;
    //[SerializeField] private Image keysImage;
    //[SerializeField] private List<Sprite> keysImageSprites;

    private List<string> tutorialDialogue = new List<string> {
        "Use the arrow keys to move",  // Left and Right arrow button dialogue
        "Use the Z key to jump",  // Z button dialogue
        "Use the X key to attack"  // X button dialogue
    };

    private List<string> buttonNameOfTutorial = new List<string> { "Horizontal", "JumpButton", "MeleeAttackButton" };
    private string currentRequestNameOfButton;

    private void Awake()
    {
        tutorialUIGameObject.SetActive(true);
        tutorialUIAnimator.SetBool("IsShowing", false);
    }

    public void ShowTutorialUI(Index_ButtonNameOfTutorial index_ButtonNameOfTutorial)
    {
        for(int i=0; i< keysImages.Count; i++)
        {
            if(i == (int)index_ButtonNameOfTutorial)
            {
                keysImages[i].gameObject.SetActive(true);
            }
            else
            {
                keysImages[i].gameObject.SetActive(false);
            }
        }
        //keysImage.sprite = keysImageSprites[(int)index_ButtonNameOfTutorial];
        tutorialUIGameObject.SetActive(true);
        tutorialStartPosition = singGameObject.transform.position;
        currentRequestNameOfButton = buttonNameOfTutorial[(int)index_ButtonNameOfTutorial];
        tutorialText.text = tutorialDialogue[(int)index_ButtonNameOfTutorial];

        tutorialUIAnimator.SetBool("IsShowing", true);
        StopAllCoroutines();
        StartCoroutine(HideWhenArrowKeyPressed(index_ButtonNameOfTutorial));
    }

    private IEnumerator HideWhenArrowKeyPressed(Index_ButtonNameOfTutorial index_ButtonNameOfTutorial)
    {
        if(currentRequestNameOfButton == null)
        {
            tutorialUIAnimator.SetBool("IsShowing", false);
            Debug.LogError("Error! Incorrect call to the function of tutorial - Variable \"" + nameof(currentRequestNameOfButton) + "\" are null reference!");
            StopAllCoroutines();
            yield return null;
        }
        if (singGameObject.GetComponent<SingScript>() == null)
        {
            tutorialUIAnimator.SetBool("IsShowing", false);
            Debug.LogError("Error! Incorrect call to the function of tutorial - Getting temp variable \"" + nameof(singGameObject) + "\" are null reference!");
            StopAllCoroutines();
            yield return null;
        }

        if (index_ButtonNameOfTutorial == Index_ButtonNameOfTutorial.JumpButton)
        {
            //singGameObject.GetComponent<Rigidbody2D>().constraints  = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }

        while (true)
        {
            if (Vector2.Distance(tutorialStartPosition, singGameObject.transform.position) > tutorialMoveOutDisappearDistanceX)
            {
                break;
            }
            if (FindObjectOfType<DialogueManager>().IsEndOfDialogue)
            {
                if (Input.GetButtonDown(currentRequestNameOfButton))
                {
                    //singGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    break;
                }
            }
            yield return null;
        }

        
        yield return new WaitForSeconds(closeDialogueTime);
        tutorialUIAnimator.SetBool("IsShowing", false);

        if (index_ButtonNameOfTutorial == Index_ButtonNameOfTutorial.Horizontal)
        {
            yield return new WaitForSeconds(1.0f);
            tutorialUIGameObject.SetActive(false);
            ShowTutorialUI(Index_ButtonNameOfTutorial.JumpButton);
        }
    }

    public void FadeInTutorialUI() // didn't use yet
    {
        tutorialUIGameObject.SetActive(false);
        this.enabled = false;
    }

}
