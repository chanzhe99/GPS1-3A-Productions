using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingAnimetionHelper : MonoBehaviour
{
    private void PlayerMoveSound()
    {
        SoundManagerScripts.PlaySound("playerWalking");
    }//make player move sound

    private void PlayerKnifeSound()
    {
        SoundManagerScripts.PlaySound("playerKnife");
    }//make player attack sound

    private void PlayerJumpSound()
    {
        SoundManagerScripts.PlaySound("playerJump");
    }//make player jump sound
}


