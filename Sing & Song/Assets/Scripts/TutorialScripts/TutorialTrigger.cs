using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private Rigidbody2D singRigidbody2D;
    [SerializeField] private TutorialTimelineController tutorialTimelineController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Global.tag_Player))
        {
            singRigidbody2D.velocity = Vector2.zero;
            Destroy(this.GetComponent<BoxCollider2D>());

            tutorialTimelineController.StartTutorialMovie();
        }
    }
}
