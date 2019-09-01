using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Animator animator;
    private bool isUsingButton = false;
    private Queue<string> sentences = new Queue<string>();
    public SingScript singScript;
    private bool isEndOfDialogue = true;
    private bool endWillAssignedToControl = true;
    private bool hasMultipleDialogue = false;

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
        //singScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SingScript>();
    }

    private void Update()
    {
        if (isUsingButton && !isEndOfDialogue)
        {
            if (Input.GetButtonDown("InteractButton"))
            {
                DisplayNextSentence();
            }

        }
    }

    public void StartDialogue(ObjectDialogue dialogue, bool isClicked, bool endWillAssignedToControl, bool stillHaveDialogue = false)
    {
        this.endWillAssignedToControl = endWillAssignedToControl;
        isEndOfDialogue = false;
        singScript.canDoAction = false;
        isUsingButton = isClicked;
        hasMultipleDialogue = stillHaveDialogue;
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
            if (!hasMultipleDialogue)
            {
                EndDialogue();
            }
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
