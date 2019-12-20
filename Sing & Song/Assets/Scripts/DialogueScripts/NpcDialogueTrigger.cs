using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcDialogueTrigger : MonoBehaviour
{
    //[SerializeField] private bool isInteractable = false;
    private bool isCollidingPlayer = false;
    private bool onReadDialogue = true;
    private int currentDialogueIndex = 0;
    [SerializeField] private Animator button;
    //public GameObject test;
    [SerializeField] private List<ObjectDialogue> dialogues = new List<ObjectDialogue>();
    [SerializeField] private DialogueManager dialogueManager;
    //trigger with button
    /*public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }*/
    /*
    if(Input.GetButtonDown("InteractButton"))
            {
                Debug.Log("Pressing Button");
                OpenDialogue(true);
}
*/

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag("Player"))
        {
            isCollidingPlayer = true;
            if(button != null) { button.SetBool("IsNear", true); }
            //please press button notification active to true
            StopAllCoroutines();
            StartCoroutine(DetectButtonDown());
        }        
    }
    
    void OnTriggerExit2D(Collider2D hit)
    {
        if (hit.CompareTag("Player"))
        {
            isCollidingPlayer = false;
            if(button != null) { button.SetBool("IsNear", false); }
            //please press button notification active to false
            StopAllCoroutines();
        }
    }

    private IEnumerator DetectButtonDown()
    {
        currentDialogueIndex = 0;
        onReadDialogue = true;

        while (onReadDialogue)
        {
            if(dialogueManager != null)
            {
                if (Input.GetButtonDown("InteractButton") && dialogueManager.IsEndOfDialogue)
                {
                    FindObjectOfType<SingScript>().GetComponent<SingScript>().SetStateIdle();
                    FindObjectOfType<SingScript>().GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    button.SetBool("IsNear", false);

                    while (true)
                    {
                        if (currentDialogueIndex > (dialogues.Count - 1) && dialogueManager.IsEndOfDialogue)
                        {
                            onReadDialogue = false;
                            break;
                        }
                        else if (dialogueManager.IsEndOfDialogue)
                        {
                            if ((currentDialogueIndex < (dialogues.Count - 1)))
                            {
                                OpenDialogue(dialogues[currentDialogueIndex], true, true, true);
                            }
                            else
                            {
                                OpenDialogue(dialogues[currentDialogueIndex], true, true, false);
                            }
                            currentDialogueIndex++;
                        }
                        yield return null;
                    }
                }
            }
            yield return null;
        }
        //yield return null;
        while (true)
        {
            if (isCollidingPlayer == true)
            {
                yield return new WaitForSeconds(1.0f);
                button.SetBool("IsNear", true);
                StopAllCoroutines();
                StartCoroutine(DetectButtonDown());
                break;
            }
            yield return null;
        }
    }

    public void OpenDialogue(ObjectDialogue dialogue, bool isClicked = true, bool endWillAssignedToControl = true, bool stillHaveDialogue = false)
    {
        Debug.Log(gameObject.name);
        dialogueManager.StartDialogue(dialogue, isClicked, endWillAssignedToControl, stillHaveDialogue);
    }
}
