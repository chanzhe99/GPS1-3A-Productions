﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Animator animator;
    private bool isUsingButton = false;
    private Queue<string> sentences;
    public SingScript singScript;
    private bool isEndOfDialogue = true;
    private bool endWillAssignedToControl = true;

    public bool IsEndOfDialogue
    {
        get
        {
            return isEndOfDialogue;
        }
    }

    public Queue<string> Sentences
    {
        get
        {
            return sentences;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        //singScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SingScript>();
    }

    private void Update()
    {
        if (!isUsingButton)
        {
            if (Input.GetButtonDown("InteractButton"))
            {
                DisplayNextSentence();
            }

        }
    }

    public void StartDialogue(ObjectDialogue dialogue, bool isClicked, bool endWillAssignedToControl)
    {
        this.endWillAssignedToControl = endWillAssignedToControl;
        isEndOfDialogue = false;
        singScript.canDoAction = false;
        isUsingButton = isClicked;
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            isUsingButton = true;
            isEndOfDialogue = true;
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        if (endWillAssignedToControl)
        {
            singScript.canDoAction = true;
        }
        animator.SetBool("IsOpen", false);
    }
}
