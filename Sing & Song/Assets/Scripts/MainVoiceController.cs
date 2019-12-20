using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainVoiceController : MonoBehaviour
{
    public AudioMixer audioMixer;    // Control Mixer

    [SerializeField] private List<Slider> masterSliders;
    [SerializeField] private List<Slider> musicSliders;
    [SerializeField] private List<Slider> soundEffectSliders;
    private SaveData.GameSoundVolumeData gameSoundVolumeData;

    public void Start()
    {
        gameSoundVolumeData = (SaveData.GameSoundVolumeData)SaveDataManager.LoadDataGetObject(Global.pathOfData_GameSoundVolumeData);

        if (gameSoundVolumeData == null)
        {
            gameSoundVolumeData = new SaveData.GameSoundVolumeData();
        }

        audioMixer.SetFloat("MasterVolume", gameSoundVolumeData.MasterVolume);
        audioMixer.SetFloat("MusicVolume", gameSoundVolumeData.MusicVolume);
        audioMixer.SetFloat("SoundEffectVolume", gameSoundVolumeData.SoundEffectVolume);

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SaveDataManager.DeleteData(Global.pathOfData_GameSoundVolumeData);
            }
        }
    }

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

    // ! save volume data functions
    public void LaodGameSoundVolumeData()
    {
        foreach (Slider masterSlider in masterSliders) masterSlider.value = gameSoundVolumeData.MasterVolume;
        foreach (Slider musicSlider in musicSliders) musicSlider.value = gameSoundVolumeData.MusicVolume;
        foreach (Slider soundEffectSlider in soundEffectSliders) soundEffectSlider.value = gameSoundVolumeData.SoundEffectVolume;
    }

    public void SaveGameSoundVolumeData()
    {
        float masterVolume, musicVolume, soundEffectVolume;

        audioMixer.GetFloat("MasterVolume", out masterVolume);
        audioMixer.GetFloat("MusicVolume", out musicVolume);
        audioMixer.GetFloat("SoundEffectVolume", out soundEffectVolume);

        gameSoundVolumeData = new SaveData.GameSoundVolumeData(masterVolume, musicVolume, soundEffectVolume);

        //! Debug
        //Debug.Log(nameof(gameSoundVolumeData.masterVolume) + " : " + gameSoundVolumeData.masterVolume + ", " + nameof(gameSoundVolumeData.musicVolume) + " : " + gameSoundVolumeData.musicVolume + ", " + nameof(gameSoundVolumeData.soundEffectVolume) + " : " + gameSoundVolumeData.soundEffectVolume);

        SaveDataManager.SaveData(gameSoundVolumeData, Global.pathOfData_GameSoundVolumeData);
    }
    // ! End of save volume data functions
}
