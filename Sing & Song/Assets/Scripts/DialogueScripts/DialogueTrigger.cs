using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public ObjectDialogue dialogue;
    //trigger with button
    /*public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }*/

    void OnTriggerEnter2D(Collider2D hit)
    {
        if(hit.CompareTag("Player"))
        {
            OpenDialogue(false);
            //Destroy(gameObject);
        }
    }

    public void OpenDialogue(bool isClicked = true)
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, isClicked);
    }
}