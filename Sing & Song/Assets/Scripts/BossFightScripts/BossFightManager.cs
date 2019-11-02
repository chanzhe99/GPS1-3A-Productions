using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private AudioClip rhinoBossFightAudioClip;
    [SerializeField] private AudioSource backgroundMusicAudioSource;
    //[SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private GameObject bossFightTriggerGameObject;
    private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset bossFightPreMovie;
    [SerializeField] private RhinoAI rhinoAI;
    [SerializeField] private SingScript singScript;
    [SerializeField] private Animator singSpriteAnimator;
    [SerializeField] private Animator rhinoBossSpriteAnimator;
    private RuntimeAnimatorController singDefalutRuntimeAnimatorController;
    private RuntimeAnimatorController rhinoBossDefalutRuntimeAnimatorController;
    [Header("Roar Diaologue:")]
    [SerializeField] private List<DialogueTrigger> roarDialogueTriggers = new List<DialogueTrigger>();

    // Start is called before the first frame update
    private void Start()
    {
        playableDirector = this.GetComponent<PlayableDirector>();
        singDefalutRuntimeAnimatorController = singSpriteAnimator.runtimeAnimatorController;
        rhinoBossDefalutRuntimeAnimatorController = rhinoBossSpriteAnimator.runtimeAnimatorController;
    }

    public void PlayBossFightMovie()
    {
        singScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        singScript.CanDoAction = false;
        rhinoAI.ableDoEnemyState = false;
        singSpriteAnimator.runtimeAnimatorController = null;
        rhinoBossSpriteAnimator.runtimeAnimatorController = null;
        playableDirector.Play(bossFightPreMovie, DirectorWrapMode.None);
    }
    
    public void PlayDialogue(int whichDialogueIndex)
    {
        roarDialogueTriggers[whichDialogueIndex].OpenDialogue();
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

    public void EndOfBossFightMovie()
    {
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true);
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        rhinoBossSpriteAnimator.runtimeAnimatorController = rhinoBossDefalutRuntimeAnimatorController;
        singScript.CanDoAction = true;
        rhinoAI.ableDoEnemyState = true;

        DestroyAll();
    }

    private void DestroyAll()
    {
        Destroy(bossFightTriggerGameObject);
        Destroy(this.gameObject);
    }
}
