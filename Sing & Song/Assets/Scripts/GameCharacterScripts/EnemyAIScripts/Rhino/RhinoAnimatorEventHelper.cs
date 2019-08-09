using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoAnimatorEventHelper : MonoBehaviour
{
    [SerializeField] private RhinoAI rhinoAI;

    public void FlipToOtherDirection()
    {
        rhinoAI.RhinoFliping();
    }

    public void PreAttackAnimationFinish()
    {
        rhinoAI.SetIsFinishPhaseChargeTrue();
    }

    public void AttackInThisTime()
    {
        rhinoAI.SetOnPreAttackAnimationFinishTrue();
    }

    public void PlaySound(string name)
    {
        SoundManagerScripts.PlaySound(name);
    }
}
