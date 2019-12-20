using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class TutorialTimelineController : MonoBehaviour
{
    //[SerializeField] private GameObject inGameUIGameObject;
    //[SerializeField] private GameObject tutorialUIGameObject;
    [SerializeField] private GameObject dogEnemyGameObject;
    private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset attackToturialMovie = null;
    [SerializeField] private GameObject singGameObject = null;
    [SerializeField] private GameObject songGameObject = null;
    [SerializeField] private Animator singSpriteAnimator = null;
    [SerializeField] private Animator songSpriteAnimator = null;
    private RuntimeAnimatorController singDefalutRuntimeAnimatorController;
    private RuntimeAnimatorController songDefalutRuntimeAnimatorController;
    [SerializeField] private GameObject attackTutorialTriggerEventGameObject = null;
    [SerializeField] private DialogueManager dialogueManager = null;
    [SerializeField] private TutorialManager tutorialManager = null;
    [SerializeField] private List<DialogueTrigger> tutorialStartedDialogueTriggers = new List<DialogueTrigger>();
    [SerializeField] private List<DialogueTrigger> tutorialEndDialogueTriggers = new List<DialogueTrigger>();
    private int currentTutorialDialogueIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (Global.gameManager.IsTutorialMoviePlayed == false)
        {
            playableDirector = GetComponent<PlayableDirector>();
            singDefalutRuntimeAnimatorController = singSpriteAnimator.runtimeAnimatorController;
            songDefalutRuntimeAnimatorController = songSpriteAnimator.runtimeAnimatorController;
        }
        else
        {
            songGameObject.SetActive(true);
            DestroyAllTutorialGameObjects();
        }
    }

    public void StartTutorialMovie()
    {
        if (tutorialStartedDialogueTriggers != null)
        {
            if (!Global.gameManager.IsTutorialMoviePlayed)
            {
                Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);

                singSpriteAnimator.runtimeAnimatorController = null;
                songSpriteAnimator.runtimeAnimatorController = null;
                singGameObject.GetComponent<SingScript>().CanDoAction = false;
                singGameObject.GetComponent<SingScript>().doAnimationFollowPlayerState = false;
                songGameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                playableDirector.Play(attackToturialMovie, DirectorWrapMode.None);
            }
        }
    }

    public void PauseTheMovie()
    {
        playableDirector.Pause();

        currentTutorialDialogueIndex = 0;
        StopAllCoroutines();
        StartCoroutine(CheckCurrentTutorialStartedDialogueEnded());
    }

    private IEnumerator CheckCurrentTutorialStartedDialogueEnded()
    {
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        songSpriteAnimator.runtimeAnimatorController = songDefalutRuntimeAnimatorController;

        while (true)
        {
            if (dialogueManager.IsEndOfDialogue)
            {
                if (currentTutorialDialogueIndex < tutorialStartedDialogueTriggers.Count)
                {
                    tutorialStartedDialogueTriggers[currentTutorialDialogueIndex].OpenDialogue(true, false);
                    currentTutorialDialogueIndex++;
                }
                else
                {
                    break;
                }
            }
            yield return null;
        }

        singSpriteAnimator.runtimeAnimatorController = null;
        songSpriteAnimator.runtimeAnimatorController = null;
        playableDirector.Resume();
    }

    public void RequestPressAttackButtonToContinue()
    {
        playableDirector.Pause();
        tutorialManager.ShowTutorialUI(TutorialManager.Index_ButtonNameOfTutorial.MeleeAttackButton);
        Time.timeScale = 0.0f;

        StopAllCoroutines();
        StartCoroutine(CheckAttackButtonPressed());
    }

    private IEnumerator CheckAttackButtonPressed()
    {
        while (true)
        {
            if (Input.GetButtonDown("MeleeAttackButton"))
            {
                playableDirector.Resume();
                Time.timeScale = 1.0f;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public void EndOfTutorial()
    {
        if (singGameObject.GetComponent<SingScript>() != null)
        {
            singGameObject.GetComponent<SingScript>().doAnimationFollowPlayerState = true;
        }

        currentTutorialDialogueIndex = 0;
        StopAllCoroutines();
        StartCoroutine(CheckCurrentTutorialEndDialogueEnded());
    }

    private void PlayTutorialEndDialogue()
    {
        if (currentTutorialDialogueIndex == (tutorialEndDialogueTriggers.Count - 1))
        {
            tutorialEndDialogueTriggers[currentTutorialDialogueIndex].OpenDialogue(true);
        }
        else
        {
            tutorialEndDialogueTriggers[currentTutorialDialogueIndex].OpenDialogue(true, false);
        }
    }

    private IEnumerator CheckCurrentTutorialEndDialogueEnded()
    {
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        songSpriteAnimator.runtimeAnimatorController = songDefalutRuntimeAnimatorController;

        while (true)
        {
            if (dialogueManager.IsEndOfDialogue)
            {
                if(currentTutorialDialogueIndex < tutorialEndDialogueTriggers.Count)
                {
                    PlayTutorialEndDialogue();
                    currentTutorialDialogueIndex++;
                }
                else
                {
                    break;
                }
            }
            yield return null;
        }
        yield return null;
        Global.gameManager.IsTutorialMoviePlayed = true;
        Global.gameManager.SaveAllGameDatas();
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true);
        FindObjectOfType<TutorialManager>().ShowTutorialUI(TutorialManager.Index_ButtonNameOfTutorial.HealButton);
        DestroyAllTutorialGameObjects();
    }

    private void DestroyAllTutorialGameObjects()
    {
        //Destroy(tutorialUIGameObject);
        Destroy(tutorialManager.gameObject);
        Destroy(dogEnemyGameObject);
        Destroy(attackTutorialTriggerEventGameObject);
        Destroy(this.gameObject);
    }

}
