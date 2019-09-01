[System.Serializable] public class SaveData
{
    [System.Serializable] public class PlayerSpawnData
    {
        public int currentPointIndex;
        public int lastCheckPointLevelIndex;

        public PlayerSpawnData()
        {
            currentPointIndex = -1;
            lastCheckPointLevelIndex = 1;
        }

        public PlayerSpawnData(int currentPointIndex, int lastCheckPointLevelIndex)
        {
            this.currentPointIndex = currentPointIndex;
            this.lastCheckPointLevelIndex = lastCheckPointLevelIndex;
        }

    }

    [System.Serializable] public class TutorialData
    {
        public bool isTutorialMoviePlayed;

        public TutorialData()
        {
            isTutorialMoviePlayed = false;
        }

        public TutorialData(bool isTutorialMoviePlayed)
        {
            this.isTutorialMoviePlayed = isTutorialMoviePlayed;
        }

    }

    [System.Serializable] public class OpeningCutsceneData
    {
        public bool isOpeningCutsceneMoviePlayed;

        public OpeningCutsceneData()
        {
            isOpeningCutsceneMoviePlayed = false;
        }

        public OpeningCutsceneData(bool isOpeningCutsceneMoviePlayed)
        {
            this.isOpeningCutsceneMoviePlayed = isOpeningCutsceneMoviePlayed;
        }

    }

    [System.Serializable] public class GameSoundVolumeData
    {
        public float masterVolume;
        public float musicVolume;
        public float soundEffectVolume;

        public GameSoundVolumeData()
        {
            masterVolume = -19.5f;
            musicVolume = -19.5f;
            soundEffectVolume = -19.5f;
        }

        public GameSoundVolumeData(float masterVolume, float musicVolume, float soundEffectVolume)
        {
            this.masterVolume = masterVolume;
            this.musicVolume = musicVolume;
            this.soundEffectVolume = soundEffectVolume;
        }

        public void SetGameSoundVolumeData(float masterVolume, float musicVolume, float soundEffectVolume)
        {
            this.masterVolume = masterVolume;
            this.musicVolume = musicVolume;
            this.soundEffectVolume = soundEffectVolume;
        }
    }
}
