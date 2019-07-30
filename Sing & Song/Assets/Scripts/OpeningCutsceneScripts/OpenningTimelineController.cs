using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OpenningTimelineController : MonoBehaviour
{
    private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset menuWaiting;
    [SerializeField] private PlayableAsset openingMovie;
    [SerializeField] private GameObject singGameObject;
    [SerializeField] private Transform playerPivotPoint;
    [SerializeField] private GameObject boatGameObject;
    [SerializeField] private GameObject inGameUIGameObject;
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
        openingDialogueTrigger.OpenDialogue(false);
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

    private void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (playableDirector == aDirector)
        {
            singGameObject.transform.SetParent(null, true);
            //singGameObject.GetComponent<SingScript>().enabled = true;
            EnableSingController();

            //! Here for save the isMoviePlayed data

            FindObjectOfType<TutorialManager>().ShowTutorialUI(TutorialManager.Index_ButtonNameOfTutorial.Horizontal);

            Global.gameManager.IsOpeningCutsceneMoviePlayed = true;
            playableDirector.stopped -= OnPlayableDirectorStopped;
            DestroyAllOpeningGameObjects();
        }
            
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

        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    private void DisableSingController()
    {
        inGameUIGameObject.SetActive(false);
        singGameObject.GetComponent<SingScript>().canDoAction = false;
        singGameObject.GetComponent<SingScript>().enabled = false;
        singGameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
    }

    private void EnableSingController()
    {
        inGameUIGameObject.SetActive(true);
        singGameObject.GetComponent<SingScript>().enabled = true;
        singGameObject.GetComponent<SingScript>().canDoAction = true;
        singGameObject.GetComponent<Rigidbody2D>().gravityScale = singDefaultGravityScaleValue;
    }

    private void DestroyAllOpeningGameObjects()
    {
        Destroy(boatGameObject.GetComponent<Animator>());
        Destroy(playerPivotPoint.gameObject);
        Destroy(this.gameObject);

    }

}
