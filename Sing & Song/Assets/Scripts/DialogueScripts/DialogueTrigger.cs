using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private bool isPlayOneTime = false;
    //public GameObject test;
    public ObjectDialogue dialogue;
    //trigger with button
    /*public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }*/

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag("Player"))
        {
            OpenDialogue(false);
            if (isPlayOneTime)
            {
                if (GetComponent<Collider2D>() != null)
                {
                    GetComponent<Collider2D>().enabled = false;
                }
                Destroy(this.gameObject);
            }
            //Destroy(gameObject);
        }
    }

    public void OpenDialogue(bool isClicked = true, bool endWillAssignedToControl = true)
    {
        Debug.Log(gameObject.name);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, isClicked, endWillAssignedToControl);
    }
}