using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScripts : MonoBehaviour
{
    [System.Serializable] public class Audios
    {
        [SerializeField] private string name;
        [SerializeField] private AudioClip source;
        public string Name
        {
            get
            {
                return name;
            }
        }
        public AudioClip Source
        {
            get
            {
                return source;
            }
        }
        public Audios(string name, AudioClip source)
        {
            this.name = name;
            this.source = source;
        }
    }

    [SerializeField] public static List<Audios> sounds;
    private static AudioSource audioSource;

    void Start()
    {
        sounds = new List<Audios>();
        sounds.Add(new Audios("playerWalking", Resources.Load<AudioClip>("playerWalking")));
        sounds.Add(new Audios("playerJump", Resources.Load<AudioClip>("playerJump")));
        sounds.Add(new Audios("playerDash", Resources.Load<AudioClip>("playerDash")));
        sounds.Add(new Audios("playerKnife", Resources.Load<AudioClip>("playerKnife")));
        sounds.Add(new Audios("playerGetHit", Resources.Load<AudioClip>("playerGetHit")));
        sounds.Add(new Audios("playerDeath", Resources.Load<AudioClip>("playerDeath")));
        sounds.Add(new Audios("crocodileAttackBite", Resources.Load<AudioClip>("crocodileAttackBite")));
        sounds.Add(new Audios("wilddogAttack", Resources.Load<AudioClip>("wilddogAttack")));
        sounds.Add(new Audios("flyinglemurAttack", Resources.Load<AudioClip>("flyinglemurAttack")));
        sounds.Add(new Audios("pangolinAttackRolling", Resources.Load<AudioClip>("pangolinAttackRolling")));
        audioSource = transform.Find("SFXSource").GetComponent<AudioSource>();
    }

    public static void PlaySound (string clip)
    {
        foreach (Audios audios in sounds)
        {
            if (audios.Name == clip)
            {
                audioSource.PlayOneShot(audios.Source);
            }
        }
    }
}
