using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainVoiceController : MonoBehaviour
{

    public AudioMixer audioMixer;    // Control Mixer

    public void SetMasterVolume(float volume)    // Control all volume
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)    // Control Music volume
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSoundEffectVolume(float volume)    // Control sound effects volume
    {
        audioMixer.SetFloat("SoundEffectVolume", volume);
    }
}
