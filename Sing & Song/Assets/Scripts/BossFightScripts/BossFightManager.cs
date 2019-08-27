﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private AudioClip rhinoBossFightAudioClip;
    [SerializeField] private AudioSource backgroundMusicAudioSource;
    [SerializeField] private GameObject inGameUIGameObject;
    [SerializeField] private GameObject bossFightTriggerGameObject;
    private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset bossFightPreMovie;
    [SerializeField] private RhinoAI rhinoAI;
    [SerializeField] private SingScript singScript;
    [SerializeField] private Animator singSpriteAnimator;
    private RuntimeAnimatorController singDefalutRuntimeAnimatorController;
    //[SerializeField] private List<DialogueTrigger> dialogueTriggers = new List<DialogueTrigger>();

    // Start is called before the first frame update
    private void Start()
    {
        playableDirector = this.GetComponent<PlayableDirector>();
        singDefalutRuntimeAnimatorController = singSpriteAnimator.runtimeAnimatorController;
    }

    public void PlayBossFightMovie()
    {
        singScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        inGameUIGameObject.SetActive(false);
        singScript.canDoAction = false;
        singSpriteAnimator.runtimeAnimatorController = null;
        playableDirector.Play(bossFightPreMovie, DirectorWrapMode.None);
    }
    /*
    public void PlayDialogue(int whichDialogueIndex)
    {
        dialogueTriggers[whichDialogueIndex].OpenDialogue();
    }
    */

    public void StopForestBackgroundMusic()
    {
        backgroundMusicAudioSource.Stop();
    }

    public void StartBossFightBackgroundMusic()
    {
        backgroundMusicAudioSource.clip = rhinoBossFightAudioClip;
        backgroundMusicAudioSource.Play();
    }

    public void EndOfBossFightMovie()
    {
        inGameUIGameObject.SetActive(true);
        singSpriteAnimator.runtimeAnimatorController = singDefalutRuntimeAnimatorController;
        singScript.canDoAction = true;
        rhinoAI.ableDoEnemyState = true;

        DestroyAll();
    }

    private void DestroyAll()
    {
        Destroy(bossFightTriggerGameObject);
        Destroy(this.gameObject);
    }
}
