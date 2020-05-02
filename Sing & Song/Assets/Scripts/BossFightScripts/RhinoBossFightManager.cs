using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class RhinoBossFightManager : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager = null;
    [SerializeField] private AudioClip rhinoBossFightAudioClip = null;
    [SerializeField] private AudioSource backgroundMusicAudioSource = null;
    //[SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private GameObject bossFightTriggerGameObject = null;
    private PlayableDirector playableDirector = null;
    [SerializeField] private PlayableAsset rhinoBossFightPreMovie = null;
    [SerializeField] private PlayableAsset rhinoBossFightSkipPreMovie = null;
    [SerializeField] private PlayableAsset rhinoBossFightPhase2Movie = null;
    [SerializeField] private PlayableAsset rhinoBossFightEndMovie = null;
    [SerializeField] private RhinoAI rhinoAI = null;
    [SerializeField] private SingScript singScript = null;
    [SerializeField] private Animator singSpriteAnimator = null;
    [SerializeField] private Animator rhinoBossSpriteAnimator = null;
    private RuntimeAnimatorController singDefalutRuntimeAnimatorController = null;
    private RuntimeAnimatorController rhinoBossDefalutRuntimeAnimatorController = null;
    [Header("Roar Diaologue:")]
    [SerializeField] private List<DialogueTrigger> roarDialogueTriggers = new List<DialogueTrigger>();

    // Start is called before the first frame update
    private void Start()
    {
        playableDirector = this.GetComponent<PlayableDirector>();
        singDefalutRuntimeAnimatorController = singSpriteAnimator.runtimeAnimatorController;
        rhinoBossDefalutRuntimeAnimatorController = rhinoBossSpriteAnimator.runtimeAnimatorController;
    }

    public void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayBossFightEndMovie();
        }
        */
    }

    public void PlayBossFightPreMovie()
    {
        singScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, false);
        singScript.CanDoAction = false;
        rhinoAI.ableStartAnimation = false;
        singSpriteAnimator.runtimeAnimatorController = null;
        rhinoBossSpriteAnimator.runtimeAnimatorController = null;

        playableDirector.Play(rhinoBossFightPreMovie, DirectorWrapMode.None);
    }
    
    public void PlayBossFightPreMovieDialogue(int whichDialogueIndex)
    {
        roarDialogueTriggers[whichDialogueIndex].OpenDialogue(true, false, (whichDialogueIndex == 0) ? true : false);
        playableDirector.Pause();

        StopCoroutine("CheckBossFightPreMovieDialogueDialogueEnded");
        StartCoroutine("CheckBossFightPreMovieDialogueDialogueEnded");
    }

    private IEnumerator CheckBossFightPreMovieDialogueDialogueEnded()
    {
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        while (true)
        {
            if (dialogueManager.IsEndOfDialogue)
            {
                break;
            }
            yield return null;
        }

        singSpriteAnimator.runtimeAnimatorController = null;
        playableDirector.Resume();
    }

    public void StopForestBackgroundMusic()
    {
        backgroundMusicAudioSource.Stop();
    }

    public void StartBossFightBackgroundMusic()
    {
        backgroundMusicAudioSource.clip = rhinoBossFightAudioClip;
        backgroundMusicAudioSource.loop = true;
        backgroundMusicAudioSource.Play();
    }

    public void EndOfBossFightPreMovie()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true, 5.0f);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, true, 5.0f);
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        rhinoBossSpriteAnimator.runtimeAnimatorController = rhinoBossDefalutRuntimeAnimatorController;
        singScript.CanDoAction = true;
        rhinoAI.ableStartAnimation = true;

        Global.gameManager.rhinoBossData.SetIsNotFirstTimeRunRhinoCutscene(true);
        Global.gameManager.SaveRhinoBossData();

        DestroyBossFightTriggerObject();
    }

    private void DestroyBossFightTriggerObject()
    {
        Destroy(bossFightTriggerGameObject);
        //Destroy(this.gameObject);
    }

    public void PlayBossFightSkipPreMovie()
    {
        singScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, false);
        singScript.CanDoAction = false;
        rhinoAI.ableStartAnimation = false;
        singSpriteAnimator.runtimeAnimatorController = null;
        rhinoBossSpriteAnimator.runtimeAnimatorController = null;

        playableDirector.Play(rhinoBossFightSkipPreMovie, DirectorWrapMode.None);
    }

    public void EndOfBossFightSkipPreMovie()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true, 5.0f);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, true, 5.0f);
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        rhinoBossSpriteAnimator.runtimeAnimatorController = rhinoBossDefalutRuntimeAnimatorController;
        singScript.CanDoAction = true;
        rhinoAI.ableStartAnimation = true;

        DestroyBossFightSkipPreMovieObject();
    }

    private void DestroyBossFightSkipPreMovieObject()
    {
        // Anythings need destroy when boss fight skip pre movie is finish
    }

    public void PlayBossFightPhase2Movie()
    {
        singScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, false);
        singScript.CanDoAction = false;
        rhinoAI.allowRhinoAttackBehaviour = false;
        singSpriteAnimator.runtimeAnimatorController = null;
        rhinoBossSpriteAnimator.runtimeAnimatorController = null;

        playableDirector.Play(rhinoBossFightPhase2Movie, DirectorWrapMode.None);
    }

    public void EndOfBossFightPhase2Movie()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true, 3.0f);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, true, 3.0f);
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        rhinoBossSpriteAnimator.runtimeAnimatorController = rhinoBossDefalutRuntimeAnimatorController;
        singScript.CanDoAction = true;
        rhinoAI.allowRhinoAttackBehaviour = true;

        DestroyBossFightPhase2MovieObject();
    }

    private void DestroyBossFightPhase2MovieObject()
    {
        // Anythings need destroy when boss fight phase 2 movie is finish
    }

    public void PlayBossFightEndMovie()
    {
        singScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, false);
        singScript.CanDoAction = false;
        rhinoAI.allowRhinoAttackBehaviour = false;
        singSpriteAnimator.runtimeAnimatorController = null;
        //rhinoBossSpriteAnimator.runtimeAnimatorController = null;

        playableDirector.Play(rhinoBossFightEndMovie, DirectorWrapMode.None);
    }

    public void FadeOutSceneView()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.TrasitionFade, true, 3.0f);
        //StopCoroutine("DoubleComfirmToFadeOut");
        //StartCoroutine("DoubleComfirmToFadeOut");
    }

    private IEnumerator DoubleComfirmToFadeOut()
    {
        while (true)
        {
            if (!Global.userInterfaceActiveManager.MenusOnTrasition[(int)Global.MenusType.TrasitionFade])
            {
                Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TrasitionFade, false);
                Global.userInterfaceActiveManager.SetMenuVisibilityDirectly(Global.MenusType.TrasitionFade, true);
                break;
            }

            yield return null;
        }
    }

    public void EndOfBossFightEndMovie()
    {
        
        //Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true, 5.0f);
        //Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, true, 5.0f);
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        //rhinoBossSpriteAnimator.runtimeAnimatorController = rhinoBossDefalutRuntimeAnimatorController;
        //singScript.CanDoAction = true;
        //rhinoAI.allowRhinoAttackBehaviour = true;

        Global.gameManager.TimeToEndAndSayThankYou();

        DestroyBossFightEndMovieObject();
    }

    private void DestroyBossFightEndMovieObject()
    {
        // Anythings need destroy when boss fight end movie is finish
    }
}
