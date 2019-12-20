[System.Serializable] public class SaveData
{
    [System.Serializable] public class PlayerSpawnData
    {
        private int currentPointIndex = 0;
        private int lastCheckPointLevelIndex = 0;

        public int CurrentPointIndex => currentPointIndex;
        public int LastCheckPointLevelIndex => lastCheckPointLevelIndex;

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
        private bool isTutorialMoviePlayed = false;

        public bool IsTutorialMoviePlayed => isTutorialMoviePlayed;

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
        private bool isOpeningCutsceneMoviePlayed = false;

        public bool IsOpeningCutsceneMoviePlayed => isOpeningCutsceneMoviePlayed;

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
        private float masterVolume = 0.0f;
        private float musicVolume = 0.0f;
        private float soundEffectVolume = 0.0f;

        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SoundEffectVolume => soundEffectVolume;

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

    [System.Serializable] public class RhinoBossData
    {
        private bool isNotFirstTimeRunRhinoCutscene = false;

        public bool IsNotFirstTimeRunRhinoCutscene => isNotFirstTimeRunRhinoCutscene;

        public RhinoBossData()
        {
            isNotFirstTimeRunRhinoCutscene = false;
        }

        public RhinoBossData(bool isStartGameFirstTimeRunRhinoCutscene)
        {
            this.isNotFirstTimeRunRhinoCutscene = isStartGameFirstTimeRunRhinoCutscene;
        }
    }
}
