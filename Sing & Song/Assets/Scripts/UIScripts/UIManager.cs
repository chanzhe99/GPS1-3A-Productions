using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingScript
{
    #region Health Crystal UI Variables
    [SerializeField] private Image[] healthCrystals;
    #endregion

    #region Spirit Well UI Variables
    private Image spiritWell;
    private float spiritWellFlowSpeed;
    private Vector2 maximumSpiritPosition;
    private Vector2 minimumSpiritPosition;
    private Vector2 currentSpiritPosition;
    private bool spiritWellFlowingRight;
    #endregion
}
