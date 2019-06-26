using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    protected Transform colliderTransform;
    protected Rigidbody2D rigidbody2D;
    protected CapsuleCollider2D capsuleCollider2D;
    protected Animator animator;

    [SerializeField] protected Transform groundCheck;
    protected bool isGrounded;
    protected Vector2 checkSize;
    protected LayerMask terrainLayer;

    [SerializeField] protected GameObject playerObject;

    [SerializeField] protected int maximumHealth;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float moveSpeed;
    protected int currentHealth;

    [SerializeField] protected Vector2 knockbackForce;
    protected Vector2 knockbackDirection;
    protected bool isHit;

    private void Start()
    {
        colliderTransform = GetComponentInChildren<Transform>();
        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        terrainLayer = LayerMask.GetMask("Terrain");


        knockbackDirection = knockbackForce;
        currentHealth = maximumHealth;
        Initialise();
    }

    protected virtual void Initialise()
    {
        //InitialiseChildren
    }
}
