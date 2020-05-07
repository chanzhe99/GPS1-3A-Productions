[System.Serializable] public class SaveData
{
    [System.Serializable] public class PlayerSpawnData
    {
        private int currentPointIndex = -1;
        private bool onPlayerRevive = false;

        public int CurrentPointIndex => currentPointIndex;

        public bool OnPlayerRevive => onPlayerRevive;

        public PlayerSpawnData()
        {
            currentPointIndex = -1;
        }

        public PlayerSpawnData(int currentPointIndex, bool onPlayerRevive)
        {
            this.currentPointIndex = currentPointIndex;
            this.onPlayerRevive = onPlayerRevive;
        }

        public void SetCurrentPointIndex(int currentPointIndex)
        {
            this.currentPointIndex = currentPointIndex;
        }

        public void SetOnPlayerRevive(bool onPlayerRevive)
        {
            this.onPlayerRevive = onPlayerRevive;
        }

        public void SetPlayerSpawnData(int currentPointIndex, bool onPlayerRevive)
        {
            this.currentPointIndex = currentPointIndex;
            this.onPlayerRevive = onPlayerRevive;
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

        public void SetIsTutorialMoviePlayed(bool isTutorialMoviePlayed)
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

        public void SetIsOpeningCutsceneMoviePlayed(bool isOpeningCutsceneMoviePlayed)
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

        public void SetMasterVolume(float masterVolume)
        {
            this.masterVolume = masterVolume;
        }

        public void SetMusicVolume(float musicVolume)
        {
            this.musicVolume = musicVolume;
        }

        public void SetSoundEffectVolume(float soundEffectVolume)
        {
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

        public void SetIsNotFirstTimeRunRhinoCutscene(bool isStartGameFirstTimeRunRhinoCutscene)
        {
            this.isNotFirstTimeRunRhinoCutscene = isStartGameFirstTimeRunRhinoCutscene;
        }
    }

    [System.Serializable] public class MenuStateData
    {
        private bool isJustStartGame = true;
        private bool isOnNewGame = true;
        private bool isReturnToMainMenu = false;

        public bool IsJustStartGame => isJustStartGame;
        public bool IsOnNewGame => isOnNewGame;
        public bool IsReturnToMainMenu => isReturnToMainMenu;

        public MenuStateData()
        {
            isJustStartGame = true;
            isOnNewGame = true;
            isReturnToMainMenu = false;
        }

        public MenuStateData(bool isJustStartGame, bool isOnNewGame, bool isReturnToMainMenu)
        {
            this.isJustStartGame = isJustStartGame;
            this.isOnNewGame = isOnNewGame;
            this.isReturnToMainMenu = isReturnToMainMenu;
        }

        public void SetIsJustStartGame(bool isJustStartGame)
        {
            this.isJustStartGame = isJustStartGame;
        }

        public void SetIsOnNewGame(bool isOnNewGame)
        {
            this.isOnNewGame = isOnNewGame;
        }

        public void SetIsReturnToMainMenu(bool isReturnToMainMenu)
        {
            this.isReturnToMainMenu = isReturnToMainMenu;
        }

        public void SetMenuStateData(bool isJustStartGame, bool isOnNewGame, bool isReturnToMainMenu)
        {
            this.isJustStartGame = isJustStartGame;
            this.isOnNewGame = isOnNewGame;
            this.isReturnToMainMenu = isReturnToMainMenu;
        }

    }

}
