using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongScript : SingScript
{
    private void Update()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(nodePosition.x, this.transform.position.y), moveSpeed * Time.deltaTime);
    }

    private void SongMove()
    {
        
        
    }
    private void SongJump()
    {

    }
}
