using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    [SerializeField] private BossFightManager bossFightManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Global.tag_Player))
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            bossFightManager.PlayBossFightMovie();
        }
    }
}
