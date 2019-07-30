using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    

    [SerializeField] private GameObject singGameObject;
    [SerializeField] private ObjectDialogue dialogue;

    private string tag_Player = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tag_Player))
        {
            singGameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(this.GetComponent<BoxCollider2D>());

            FindObjectOfType<TutorialTimelineController>().StartTutorialMovie();
        }
    }

    public void PlayTutorialDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, false, false);
    }
}
