using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class TutorialTimelineController : MonoBehaviour
{
    [SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private GameObject tutorialUIGameObject;
    [SerializeField] private GameObject dogEnemyGameObject;
    private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset attackToturialMovie;
    [SerializeField] private GameObject singGameObject;
    [SerializeField] private GameObject songGameObject;
    [SerializeField] private Animator singSpriteAnimator;
    [SerializeField] private Animator songSpriteAnimator;
    private RuntimeAnimatorController singDefalutRuntimeAnimatorController;
    private RuntimeAnimatorController songDefalutRuntimeAnimatorController;
    [SerializeField] private TutorialTrigger tutotrialTrigger;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private List<DialogueTrigger> tutorialEndDialogueTriggers;
    private int currentTutorialEndDialogueIndex = 0;

    // Start is called before the first frame update
    void Start()
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

    private void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (playableDirector == aDirector)
        {
            singGameObject.GetComponent<SingScript>().doAnimationFollowPlayerState = true;

            StopAllCoroutines();
            StartCoroutine(CheckCurrentTutorialEndDialogueEnded());

            playableDirector.stopped -= OnPlayableDirectorStopped;
        }
    }

    public void StartTutorialMovie()
    {
        if (tutotrialTrigger != null)
        {
            if (!Global.gameManager.IsTutorialMoviePlayed)
            {
                inGameUIGameObject.SetActive(false);

                singSpriteAnimator.runtimeAnimatorController = null;
                songSpriteAnimator.runtimeAnimatorController = null;
                singGameObject.GetComponent<SingScript>().canDoAction = false;
                singGameObject.GetComponent<SingScript>().doAnimationFollowPlayerState = false;

                playableDirector.Play(attackToturialMovie, DirectorWrapMode.None);

                playableDirector.stopped += OnPlayableDirectorStopped;
            }
        }
    }

    public void PauseTheMovie()
    {
        playableDirector.Pause();
        tutotrialTrigger.PlayTutorialDialogue();

        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        songSpriteAnimator.runtimeAnimatorController = songDefalutRuntimeAnimatorController;

        StopAllCoroutines();
        StartCoroutine(CheckTutorialDialogueEnd());
    }

    public void RequestPressAttackButtonToContinue()
    {
        playableDirector.Pause();
        tutorialManager.ShowTutorialUI(TutorialManager.Index_ButtonNameOfTutorial.MeleeAttackButton);
        Time.timeScale = 0.0f;

        StopAllCoroutines();
        StartCoroutine(CheckAttackButtonPressed());
    }

    private void PlayTutorialEndDialogue()
    {
        if(currentTutorialEndDialogueIndex == (tutorialEndDialogueTriggers.Count - 1))
        {
            tutorialEndDialogueTriggers[currentTutorialEndDialogueIndex].OpenDialogue(false);
        }
        else
        {
            tutorialEndDialogueTriggers[currentTutorialEndDialogueIndex].OpenDialogue(false, false);
        }
    }

    private IEnumerator CheckTutorialDialogueEnd()
    {
        while (true)
        {
            if(dialogueManager.IsEndOfDialogue)
            {
                singSpriteAnimator.runtimeAnimatorController = null;
                songSpriteAnimator.runtimeAnimatorController = null;

                playableDirector.Resume();
                break;
            }
            yield return null;
        }
        yield return null;
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
    private IEnumerator CheckCurrentTutorialEndDialogueEnded()
    {
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        songSpriteAnimator.runtimeAnimatorController = songDefalutRuntimeAnimatorController;

        while (true)
        {
            if (dialogueManager.IsEndOfDialogue)
            {
                if(currentTutorialEndDialogueIndex < tutorialEndDialogueTriggers.Count)
                {
                    PlayTutorialEndDialogue();
                    currentTutorialEndDialogueIndex++;
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
        inGameUIGameObject.SetActive(true);
        DestroyAllTutorialGameObjects();
    }

    private void DestroyAllTutorialGameObjects()
    {
        Destroy(tutorialUIGameObject);
        Destroy(tutorialManager.gameObject);
        Destroy(dogEnemyGameObject);
        Destroy(tutotrialTrigger.gameObject);
        Destroy(this.gameObject);
    }
}
