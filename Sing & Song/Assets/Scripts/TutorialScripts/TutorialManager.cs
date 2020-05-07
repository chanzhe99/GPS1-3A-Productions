using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    public enum Index_ButtonNameOfTutorial { JumpButton, Horizontal, MeleeAttackButton, HealButton, DashButton };

    //[SerializeField] private GameObject tutorialUIGameObject;
    [SerializeField] private Animator tutorialUIAnimator;
    [SerializeField] private Text tutorialText;
    [SerializeField] private GameObject singGameObject;
    private float closeDialogueTime = 0.1f;
    private Vector2 tutorialStartPosition;
    [SerializeField] private float tutorialMoveOutDisappearDistanceX;
    [SerializeField] private List<Image> keysImages;
    //[SerializeField] private Image keysImage;
    //[SerializeField] private List<Sprite> keysImageSprites;

    [SerializeField] private List<string> tutorialDialogue = new List<string>();
    /*
    {
        "Move",  // Left and Right arrow button dialogue
        "Hold to Jump",  // Z button dialogue
        "Attack"  // X button dialogue
    };
    */

    private List<string> buttonNameOfTutorial = new List<string> { "JumpButton", "Horizontal", "MeleeAttackButton", "HealButton", "DashButton" };
    private string currentRequestNameOfButton;

    private void Awake()
    {
        tutorialUIAnimator.SetBool("IsShowing", false);
    }

    private void Update()
    {
        /*
        currentRequestNameOfButton = buttonNameOfTutorial[(int)Index_ButtonNameOfTutorial.HealButton];
        if (Input.GetButtonDown(currentRequestNameOfButton))
        {
            Debug.Log("Press");
        }
        */
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
        Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TutorialUI, true);
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

        if (index_ButtonNameOfTutorial == Index_ButtonNameOfTutorial.Horizontal)
        {
            //tutorialUIAnimator.SetBool("IsShowing", false);
            //singGameObject.GetComponent<Rigidbody2D>().constraints  = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }

        bool triggedEndTutorial = true;
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        while (triggedEndTutorial)
        {
            switch (index_ButtonNameOfTutorial)
            {
                case Index_ButtonNameOfTutorial.JumpButton:
                    if (Input.GetButtonDown(currentRequestNameOfButton))
                        triggedEndTutorial = false;

                    if (Vector2.Distance(tutorialStartPosition, singGameObject.transform.position) > tutorialMoveOutDisappearDistanceX)
                        triggedEndTutorial = false;
                    break;


                case Index_ButtonNameOfTutorial.Horizontal:
                    if (Input.GetButtonDown(currentRequestNameOfButton))
                        triggedEndTutorial = false;

                    if (Vector2.Distance(tutorialStartPosition, singGameObject.transform.position) > tutorialMoveOutDisappearDistanceX)
                        triggedEndTutorial = false;
                    break;


                case Index_ButtonNameOfTutorial.MeleeAttackButton:
                    if (dialogueManager.IsEndOfDialogue)
                    {
                        if (Input.GetButtonDown(currentRequestNameOfButton))
                        {
                            triggedEndTutorial = false;
                        }
                    }
                    break;

                case Index_ButtonNameOfTutorial.HealButton:
                    //if (Input.GetButtonDown(currentRequestNameOfButton))
                    //    triggedEndTutorial = false;
                    if (Vector2.Distance(tutorialStartPosition, singGameObject.transform.position) > tutorialMoveOutDisappearDistanceX * 1.5f)
                        triggedEndTutorial = false;

                    if (singGameObject.GetComponent<SingScript>().GetHealedOnce())
                    {
                        print("healed = " + singGameObject.GetComponent<SingScript>().GetHealedOnce());
                        triggedEndTutorial = false;
                    }

                    break;

                case Index_ButtonNameOfTutorial.DashButton:
                    if (Input.GetButtonDown(currentRequestNameOfButton))
                        triggedEndTutorial = false;
                    if (Vector2.Distance(tutorialStartPosition, singGameObject.transform.position) > tutorialMoveOutDisappearDistanceX * 1.5f)
                        triggedEndTutorial = false;
                    break;

            }// put the event triger in the switch case

            yield return null;
        }

        
        yield return new WaitForSeconds(closeDialogueTime);
        tutorialUIAnimator.SetBool("IsShowing", false);

        if (index_ButtonNameOfTutorial == Index_ButtonNameOfTutorial.JumpButton)
        {
            yield return new WaitForSeconds(1.0f);
            Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TutorialUI, false);
            ShowTutorialUI(Index_ButtonNameOfTutorial.Horizontal);
        }

        if(index_ButtonNameOfTutorial == Index_ButtonNameOfTutorial.HealButton)
        {
            yield return new WaitForSeconds(1.0f);
            Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TutorialUI, false);
            ShowTutorialUI(Index_ButtonNameOfTutorial.DashButton);
        }
    }

    public void FadeInTutorialUI() // didn't use yet
    {
        Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TutorialUI, false);
        this.enabled = false;
    }

}
