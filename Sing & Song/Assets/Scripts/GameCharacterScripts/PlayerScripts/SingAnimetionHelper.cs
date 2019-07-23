using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingAnimetionHelper : MonoBehaviour
{
    private void PlayerMoveSound()
    {
        SoundManagerScripts.PlaySound("playerWalking");
    }//make player move sound
}
