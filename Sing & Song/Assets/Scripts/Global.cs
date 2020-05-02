using UnityEngine;

public class Global
{
    public static readonly bool onSaveDataManagerDebug = false;

    public static GameManager gameManager;
    public static UIActiveManager userInterfaceActiveManager;

    public static readonly string pathOfData_PlayerSpawnData = "/data/PlayerSpawnData.dat";
    public static readonly string pathOfData_TutorialData = "/data/TutorialData.dat";
    public static readonly string pathOfData_OpeningCutsceneData = "/data/OpeningCutsceneData.dat";
    public static readonly string pathOfData_GameSoundVolumeData = "/data/GameSoundVolumeData.dat";
    public static readonly string pathOfData_RhinoBossData = "/data/RhinoBossData.dat";
    public static readonly string pathOfData_MenuStateData = "/data/MenuStateData.dat";

    public static readonly string tag_Player = "Player";

    public static readonly string nameAnimatorTrigger_TrasitionFade_FadeOut = "FadeOut";
    public static readonly string nameAnimatorTrigger_TrasitionFade_FadeIn = "FadeIn";
    public static readonly string nameAnimatorTrigger_EndScene_FadeOut = "FadeOut";
    public static readonly string nameAnimatorTrigger_EndScene_FadeIn = "FadeIn";
    public static readonly string nameAnimatorTrigger_SplashScreen_PassKDU_Logo = "PassKDU_Logo";
    public static readonly string nameAnimatorTrigger_SplashScreen_Pass3A_Production_Logo = "Pass3A_Production_Logo";
    public static readonly string nameAnimatorInteger_RhinoAI_AttackType = "AttackType";
    public static readonly string nameAnimatorTrigger_RhinoAI_Roar = "Roar";

    public static readonly string nameAnimatorBool_RhinoAI_Phase2 = "Phase2";
    public static readonly string nameAnimatorBool_RhinoAI_OnStompWave = "OnStompWave";
    public static readonly string nameAnimatorBool_RhinoAI_ContinueAttack = "ContinueAttack";
    public static readonly string nameAnimatorBool_Sing_IsRunning = "isRunning";
    public static readonly string nameAnimatorBool_Song_IsRunning = "isRunning";
    public static readonly string nameAnimatorBool_Song_IsHealing = "isHealing";

    public static readonly string[] nameInputs = { "Horizontal", "Vertical", "InteractButton", "JumpButton", "HealButton", "DashButton", "MeleeAttackButton", "SpiritAttackButton" };

    public static readonly string nameAnimatorLayer_SpecialAnimationLayer = "Special Animation Layer";

    public static readonly string nameGameObject_UI = "UI";
    public static readonly string[] nameGameObject_Menus = { "PauseMenuUI", "TutorialUI", "AbilityGainUI", "InGameUI", "DialogueUI", "StartMenuUI", "BossHPBarUI", "TrasitionFade" };

    public static readonly float gameStartFadeInSpeed = 2.5f;
    public static readonly float gameLevelTransitionFadeInSpeed = 1.0f;

    public enum SceneIndex{ Splash, Area1, EndScene }
    public enum InputsType { Horizontal, Vertical, InteractButton, JumpButton, HealButton, DashButton, MeleeAttackButton, SpiritAttackButton, Length };
    public enum MenusType { PauseMenuUI, TutorialUI, AbilityGainUI, InGameUI, DialogueUI, StartMenuUI, BossHPBarUI, TrasitionFade, Length };
    public enum Index_ButtonNameOfTutorial { JumpButton, Horizontal, MeleeAttackButton };
}

