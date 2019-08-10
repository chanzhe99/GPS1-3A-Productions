using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    [SerializeField] Animator rockAnimator;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            rockAnimator.SetBool("isSaving", true);
            //print(rockAnimator.GetBool("isSaving"));
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            rockAnimator.SetBool("isSaving", false);
            //print(rockAnimator.GetBool("isSaving"));
        }
    }
}
