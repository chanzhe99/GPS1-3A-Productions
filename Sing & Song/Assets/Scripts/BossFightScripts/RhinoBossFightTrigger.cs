using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoBossFightTrigger : MonoBehaviour
{
    [SerializeField] private RhinoBossFightManager rhinoBossFightManager = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Global.tag_Player))
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            rhinoBossFightManager.PlayBossFightPreMovie();
        }
    }
}
