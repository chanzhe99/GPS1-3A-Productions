﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActiveManager : MonoBehaviour
{
    private Canvas userInterfaceCanvas;

    private List<CanvasGroup> menusCanvasGroup = new List<CanvasGroup>();
    private List<bool> menusTempVisibilityState = new List<bool>();
    private List<bool> menusVisibilityState = new List<bool>();
    private List<bool> menusOnTrasition = new List<bool>();

    public bool[] MenusVisibilityState { get { return menusVisibilityState.ToArray(); } }
    public bool[] MenusOnTrasition { get { return menusOnTrasition.ToArray(); } }

    private void Awake()
    {
        if (Global.userInterfaceActiveManager == null) { Global.userInterfaceActiveManager = this; }
        else { Destroy(gameObject); return; }

        userInterfaceCanvas = GameObject.Find(Global.nameGameObject_UI).GetComponent<Canvas>();

        for (int i = 0; i < (int)Global.MenusType.Length; i++)
        {
            menusCanvasGroup.Add(null);
            menusVisibilityState.Add(true);
            menusOnTrasition.Add(false);
        }

        int checkMenusAmount = 0;
        foreach (CanvasGroup tempCanvasGroup in userInterfaceCanvas.GetComponentsInChildren<CanvasGroup>())
        {
            for (int i = 0; i < (int)Global.MenusType.Length; i++)
            {
                if (tempCanvasGroup.name == Global.nameGameObject_Menus[i])
                {
                    checkMenusAmount++;

                    menusCanvasGroup[i] = tempCanvasGroup;
                    menusVisibilityState[i] = tempCanvasGroup.interactable;

                    break;
                }
            }
        }



        if (checkMenusAmount != (int)Global.MenusType.Length) // check the menus amount
        {
            Debug.Log("Menus Amount = " + checkMenusAmount);
            Debug.LogError("Error! Menus are not setting corectlly!");
        }
    }

    private void Start()
    {
        SetMenusVisibilityWhileGameStart();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetMenuVisibilitySmoothly(Global.MenusType.BossHPBarUI, !menusVisibilityState[(int)Global.MenusType.BossHPBarUI]);
        }
        */
    }

    private void SetMenusVisibilityWhileGameStart()
    {
        SetMenuVisibilityDirectly(Global.MenusType.PauseMenuUI, false);
        SetMenuVisibilityDirectly(Global.MenusType.TutorialUI, false);
        SetMenuVisibilityDirectly(Global.MenusType.AbilityGainUI, false);
        SetMenuVisibilityDirectly(Global.MenusType.InGameUI, false);
        SetMenuVisibilityDirectly(Global.MenusType.DialogueUI, false);
        //SetMenuVisibilityDirectly(Global.MenusType.StartMenuUI, true);
        SetMenuVisibilityDirectly(Global.MenusType.BossHPBarUI, false);
    }

    /// <summary>
    /// Switch the visibility of all menus.
    /// </summary>
    /// <param name="visibilityState">The visibility of all menus</param>
    public void AllMenusVisibility(bool visibilityState)
    {
        for (int i = 0; i < menusCanvasGroup.Count; i++)
        {
            SetMenuVisibilityDirectly((Global.MenusType)i, visibilityState);
        }
    }

    /// <summary>
    /// Inverse the visibility of all menus.
    /// </summary>
    public void AllMenusInverseVisibility()
    {
        for (int i = 0; i < menusCanvasGroup.Count; i++)
        {
            SetMenuVisibilityDirectly((Global.MenusType)i, !menusCanvasGroup[i].interactable);
        }
    }

    /// <summary>
    /// Disable the visibility of all menus, and will temporary save the visibility state.
    /// After Used : Enable to invoking the AllMenusLoadTemporaryVisibilityStateAndOpen() function and
    ///              disable to invoking the AllMenusSaveTemporaryVisibilityStateAndClose() function.
    /// </summary>
    public void AllMenusSaveTemporaryVisibilityStateAndClose()
    {
        if (menusTempVisibilityState.Count <= 0)
        {
            for (int i = 0; i < menusCanvasGroup.Count; i++)
            {
                menusTempVisibilityState.Add(menusCanvasGroup[i].interactable);
                SetMenuVisibilityDirectly((Global.MenusType)i, false);
            }
        }
        else
        {
            Debug.LogError("Fail to SaveCurrentMenusActiveStateAndCloseAllMenus!");
        }
    }

    /// <summary>
    /// Set the visibility of all menus to the temporary visibility state, and will clear all the temporary visibility state.
    /// After Used : Enable to invoking the AllMenusSaveTemporaryVisibilityStateAndClose() function and
    ///              disable to invoking the AllMenusLoadTemporaryVisibilityStateAndOpen() function again.
    /// </summary>
    public void AllMenusLoadTemporaryVisibilityStateAndOpen()
    {
        if (menusTempVisibilityState.Count > 0)
        {
            Debug.Log(menusTempVisibilityState);
            for (int i = 0; i < menusTempVisibilityState.Count; i++)
            {
                SetMenuVisibilityDirectly((Global.MenusType)i, menusTempVisibilityState[i]);
            }
            menusTempVisibilityState.Clear();
            Debug.Log(menusTempVisibilityState);
        }
        else
        {
            Debug.LogError("Fail to LoadAndOpenAllMenusToBeforeState!");
        }
    }

    public void InverseMenuVisibilityDirectly(Global.MenusType menusType)
    {
        SetMenuVisibilityDirectly(menusType, !menusVisibilityState[(int)menusType]);
    }

    public void InverseMenuVisibilitySmoothly(CanvasGroup menusType)
    {
        for (int i = 0; i < menusCanvasGroup.Count; i++)
        {
            if (menusType == menusCanvasGroup[i])
            {
                SetMenuVisibilitySmoothly((Global.MenusType)i, !menusVisibilityState[i]);
                break;
            }
        }


    }

    /// <summary>
    /// Directly switch the visibility of the menu.
    /// </summary>
    /// <param name="menusType">The visibility of the menu needs to be set.</param>
    /// <param name="visibilityState">The visibility state will set to the menu's visibility.</param>
    public void SetMenuVisibilityDirectly(Global.MenusType menusType, bool visibilityState)
    {
        menusVisibilityState[(int)menusType] = visibilityState; // Update the menus active state while switch the active of menu
        menusCanvasGroup[(int)menusType].interactable = menusCanvasGroup[(int)menusType].blocksRaycasts = visibilityState; // Switch the menu interactable and the mouse raycast block
        menusCanvasGroup[(int)menusType].alpha = visibilityState ? 1.0f : 0.0f; // Switch the visibility

    }

    /// <summary>
    /// Smoothly switch the visibility of the menu.
    /// </summary>
    /// <param name="menusType">The visibility of the menu needs to be set.</param>
    /// <param name="visibilityState">The visibility state will set to the menu's visibility.</param>
    /// <param name="smoothlTime">The time of the smoothly transition.</param>
    public void SetMenuVisibilitySmoothly(Global.MenusType menusType, bool visibilityState, float smoothlTime = 1.0f, bool slowToFast = true)
    {
        if (menusVisibilityState[(int)menusType] != visibilityState)
        {
            menusOnTrasition[(int)menusType] = true;
            menusVisibilityState[(int)menusType] = visibilityState;
            //StopAllCoroutines();
            StartCoroutine(VisibilitySmoothlySwitchCoroutine(menusType, menusVisibilityState[(int)menusType], smoothlTime, slowToFast));
        }

    }

    public void ShowMenuSmoothly(string menuName)
    {
        for (int i = 0; i < Global.nameGameObject_Menus.Length; i++)
        {
            if (menuName == Global.nameGameObject_Menus[i])
            {
                if (menusVisibilityState[i] != true)
                {
                    menusOnTrasition[i] = true;
                    menusVisibilityState[i] = true;
                    StopAllCoroutines();
                    StartCoroutine(VisibilitySmoothlySwitchCoroutine((Global.MenusType)i, menusVisibilityState[i], 1.0f, true));
                }
                break;
            }
        }
    }

    public void HideMenuSmoothly(string menuName)
    {
        for (int i = 0; i < Global.nameGameObject_Menus.Length; i++)
        {
            if (menuName == Global.nameGameObject_Menus[i])
            {
                if (menusVisibilityState[i] != false)
                {
                    menusOnTrasition[i] = true;
                    menusVisibilityState[i] = false;
                    StopAllCoroutines();
                    StartCoroutine(VisibilitySmoothlySwitchCoroutine((Global.MenusType)i, menusVisibilityState[i], 1.0f, true));
                }
                break;
            }
        }
    }

    private IEnumerator VisibilitySmoothlySwitchCoroutine(Global.MenusType menusType, bool visibilityState, float smoothlTime, bool slowToFast = true)
    {
        float alpha = menusCanvasGroup[(int)menusType].alpha; // Get the latest alpha value when everytime invoking VisibilitySmoothlySwitchCoroutine()
        float time = (visibilityState ? alpha : 1.0f - alpha);
        bool visibility = visibilityState;

        //if (menusVisibilityState[(int)menusType]) menusCanvasGroup[(int)menusType].interactable = menusCanvasGroup[(int)menusType].blocksRaycasts = visibilityState; // Switch the menu interactable and the mouse raycast block before switching while open the menu
        menusCanvasGroup[(int)menusType].interactable = menusCanvasGroup[(int)menusType].blocksRaycasts = visibilityState; // Switch the menu interactable and the mouse raycast block before switching while open the menu

        while (true)
        {
            if (slowToFast) time += ((1.0f * Time.deltaTime) + (time * 0.2f)) / smoothlTime;
            else time += ((1.0f * Time.deltaTime) + ((1.0f - time) * 0.2f)) / smoothlTime;//(1.0f * Time.deltaTime) / smoothlTime;
            time = (time > 1.0f) ? 1.0f : time;


            menusCanvasGroup[(int)menusType].alpha = Mathf.Lerp(visibilityState ? 0.0f : 1.0f, visibilityState ? 1.0f : 0.0f, time);// Smoothly switch the visibility

            if (visibility != menusVisibilityState[(int)menusType]) break; // Stop visibility transition if the coroutine be invoking again
            if (time >= 1.0f) break;

            yield return null;
        }

        //if (!menusVisibilityState[(int)menusType]) menusCanvasGroup[(int)menusType].interactable = menusCanvasGroup[(int)menusType].blocksRaycasts = visibilityState; // Switch the menu interactable and the mouse raycast block after switched while close the menu

        menusOnTrasition[(int)menusType] = false;

        yield return null;
    }

}
