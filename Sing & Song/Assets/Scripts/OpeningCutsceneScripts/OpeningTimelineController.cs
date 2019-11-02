using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OpeningTimelineController : MonoBehaviour
{
    private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset menuWaiting;
    [SerializeField] private PlayableAsset openingMovie;
    [SerializeField] private GameObject singGameObject;
    [SerializeField] private Transform playerPivotPoint;
    [SerializeField] private GameObject boatGameObject;
    //[SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private GameObject triggerEvent;
    private DialogueTrigger openingDialogueTrigger;
    private DialogueManager dialogueManager;
    private float singDefaultGravityScaleValue;

    private bool isLastOpeningMovie = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Global.gameManager.IsOpeningCutsceneMoviePlayed == false)
        {
            openingDialogueTrigger = transform.GetComponentInChildren<DialogueTrigger>();
            dialogueManager = FindObjectOfType<DialogueManager>();
            singDefaultGravityScaleValue = singGameObject.GetComponent<Rigidbody2D>().gravityScale;
            playableDirector = GetComponent<PlayableDirector>();
            PlayMenuWaiting();
        }
        else
        {
            Destroy(triggerEvent);
            DestroyAllOpeningGameObjects();
        }
    }

    public void ShowOpeningDialogue() //! When the player are first time playing this game, and the player press the start game button it will show the cutscene
    {
        openingDialogueTrigger.OpenDialogue(true);
        StartCoroutine(CheckDialogueEndState());
    }

    private IEnumerator CheckDialogueEndState() //! For check the dialogue are finish while dialogue button clicked
    {
        while (true)
        {
            if(dialogueManager.IsEndOfDialogue)
            {
                PlayOpeningMovie();
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public void EndOfOpeningCutscene()
    {
        singGameObject.transform.SetParent(null, true);
        //singGameObject.GetComponent<SingScript>().enabled = true;
        EnableSingController();

        //! Here for save the isMoviePlayed data

        FindObjectOfType<TutorialManager>().ShowTutorialUI(TutorialManager.Index_ButtonNameOfTutorial.Horizontal);

        Global.gameManager.IsOpeningCutsceneMoviePlayed = true;
        DestroyAllOpeningGameObjects();
    }

    private void PlayMenuWaiting()
    {
        singGameObject.transform.position = playerPivotPoint.position;
        singGameObject.transform.SetParent(playerPivotPoint, true);

        playableDirector.Play(menuWaiting, DirectorWrapMode.Loop);
        DisableSingController();
    }

    public void PlayOpeningMovie()
    {
        playableDirector.Play(openingMovie, DirectorWrapMode.None);

        DisableSingController();
    }

    private void DisableSingController()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        singGameObject.GetComponent<SingScript>().CanDoAction = false;
        singGameObject.GetComponent<SingScript>().enabled = false;
        singGameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
    }

    private void EnableSingController()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true);
        singGameObject.GetComponent<SingScript>().enabled = true;
        singGameObject.GetComponent<SingScript>().CanDoAction = true;
        singGameObject.GetComponent<Rigidbody2D>().gravityScale = singDefaultGravityScaleValue;
    }

    private void DestroyAllOpeningGameObjects()
    {
        Destroy(boatGameObject.GetComponent<Animator>());
        Destroy(playerPivotPoint.gameObject);
        Destroy(this.gameObject);

    }

}
