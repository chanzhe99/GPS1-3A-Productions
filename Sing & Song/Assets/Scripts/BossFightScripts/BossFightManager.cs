using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private AudioClip rhinoBossFightAudioClip = null;
    [SerializeField] private AudioSource backgroundMusicAudioSource = null;
    //[SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private GameObject bossFightTriggerGameObject = null;
    private PlayableDirector playableDirector = null;
    [SerializeField] private PlayableAsset bossFightPreMovie = null;
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

    public void PlayBossFightMovie()
    {
        singScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, false);
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
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true, 5.0f);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, true, 5.0f);
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
