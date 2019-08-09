﻿public class Global
{
    public static GameManager gameManager;

    public static readonly string pathOfData_PlayerSpawnData = "/data/PlayerSpawnData.dat";
    public static readonly string pathOfData_TutorialData = "/data/TutorialData.dat";
    public static readonly string pathOfData_OpeningCutsceneData = "/data/OpeningCutsceneData.dat";

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
    public enum SceneIndex{ Splash, Area1, EndScene }
}
