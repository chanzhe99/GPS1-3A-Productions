using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockfallHitDetecter : MonoBehaviour
{
    private Rigidbody2D rockfallRigidbody2D;
    private SpriteRenderer rockfallSpriteRenderer;
    private Color tempSlowlyDisappearColor;
    private Collider2D rockfallCollider2D;
    private Collider2D rhinoCollider2D;
    private Collider2D playerCollider2D;
    private float originalColorAlpha;
    private bool onDisappearing = false;
    [SerializeField] private Vector2 bounceForce;

    private void Start()
    {
        rockfallSpriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        rockfallCollider2D = this.GetComponent<Collider2D>();
        rockfallRigidbody2D = this.GetComponent<Rigidbody2D>();
        originalColorAlpha = rockfallSpriteRenderer.color.a;
        rhinoCollider2D = FindObjectOfType<RhinoAI>().GetComponentInChildren<Collider2D>();
        playerCollider2D = FindObjectOfType<SingScript>().GetComponentInChildren<CapsuleCollider2D>();
        Physics2D.IgnoreCollision(rockfallCollider2D, rhinoCollider2D, true);
    }

    private void OnEnable()
    {
        rockfallSpriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        rockfallCollider2D = this.GetComponent<Collider2D>();
        rockfallRigidbody2D = this.GetComponent<Rigidbody2D>();
        originalColorAlpha = rockfallSpriteRenderer.color.a;
        rhinoCollider2D = FindObjectOfType<RhinoAI>().GetComponentInChildren<Collider2D>();
        playerCollider2D = FindObjectOfType<SingScript>().GetComponentInChildren<CapsuleCollider2D>();
        Physics2D.IgnoreCollision(rockfallCollider2D, rhinoCollider2D, true);

        tempSlowlyDisappearColor.r = rockfallSpriteRenderer.color.r;
        tempSlowlyDisappearColor.g = rockfallSpriteRenderer.color.g;
        tempSlowlyDisappearColor.b = rockfallSpriteRenderer.color.b;
        tempSlowlyDisappearColor.a = originalColorAlpha;
        rockfallSpriteRenderer.color = tempSlowlyDisappearColor;

        onDisappearing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (!onDisappearing)
            {
                onDisappearing = true;
                collision.gameObject.GetComponent<SingScript>().DamagePlayer(transform);
                Physics2D.IgnoreCollision(rockfallCollider2D, collision.collider, true);
                rockfallRigidbody2D.velocity = Vector2.zero;
                rockfallRigidbody2D.AddForce(bounceForce, ForceMode2D.Impulse);
                StartCoroutine(RockfallSlowlyDisappear(collision.collider));
            }
        }
        else if(collision.collider == true)
        {
            if (!onDisappearing)
            {
                onDisappearing = true;
                Physics2D.IgnoreCollision(rockfallCollider2D, playerCollider2D, true);
                rockfallRigidbody2D.velocity = Vector2.zero;
                rockfallRigidbody2D.AddForce(bounceForce, ForceMode2D.Impulse);
                StartCoroutine(RockfallSlowlyDisappear(playerCollider2D));
            }
        }
        /*
        //This one only for ground, when rockfall hit to other tag object it won't set the rockfall Actice to false if using this one, so mean that will showing until game end or player go touching or trigger it.
        if (collision.transform.tag == "Ground")
        {
            if (!onDisappearing)
            {
                onDisappearing = true;
                Physics2D.IgnoreCollision(rockfallCollider2D, playerCollider2D, true);
                rockfallRigidbody2D.velocity = Vector2.zero;
                rockfallRigidbody2D.AddForce(bounceForce, ForceMode2D.Impulse);
                StartCoroutine(RockfallSlowlyDisappear(playerCollider2D));
            }
        }
        */
    }

    private IEnumerator RockfallSlowlyDisappear(Collider2D collider)
    {
        while (onDisappearing)
        {
            tempSlowlyDisappearColor.r = rockfallSpriteRenderer.color.r;
            tempSlowlyDisappearColor.g = rockfallSpriteRenderer.color.g;
            tempSlowlyDisappearColor.b = rockfallSpriteRenderer.color.b;
            tempSlowlyDisappearColor.a = rockfallSpriteRenderer.color.a - Time.deltaTime;

            if (tempSlowlyDisappearColor.a > 0.0f)
            {
                rockfallSpriteRenderer.color = tempSlowlyDisappearColor;
            }
            else
            {
                tempSlowlyDisappearColor.a = originalColorAlpha;
                rockfallSpriteRenderer.color = tempSlowlyDisappearColor;

                Physics2D.IgnoreCollision(rockfallCollider2D, collider, false);
                onDisappearing = false;
                this.gameObject.SetActive(false);
            }
            yield return null;
        }
        
    }

    public Collider2D GetCollider2D()
    {
        return rockfallCollider2D;
    }

}
