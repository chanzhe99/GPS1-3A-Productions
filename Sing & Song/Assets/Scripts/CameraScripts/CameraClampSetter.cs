using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClampSetter : MonoBehaviour
{
    [SerializeField] private Vector2 levelSize;
    private float minX, minY;
    private float maxX, maxY;

    public float getMinX() { return minX = -(levelSize.x * 0.5f); }
    public float getMinY() { return minX = -(levelSize.y * 0.5f); }
    public float getMaxX() { return maxX = (levelSize.x * 0.5f); }
    public float getMaxY() { return maxY = (levelSize.y * 0.5f); }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, levelSize);
    }
}
