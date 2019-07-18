using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraClampSetter : MonoBehaviour
{   
    [SerializeField] private Vector2 levelSize;
    private CameraController cameraController;
    private Vector2 levelCenter;
    private float minX, minY;
    private float maxX, maxY;

    private void Start()
    {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        levelCenter = GetComponent<Transform>().position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) { cameraController.GetCameraClampSetter(this.GetComponent<CameraClampSetter>()); }
    }
    public float getMinX() { return minX = levelCenter.x - (levelSize.x * 0.5f); }
    public float getMinY() { return minX = levelCenter.y - (levelSize.y * 0.5f); }
    public float getMaxX() { return maxX = levelCenter.x + (levelSize.x * 0.5f); }
    public float getMaxY() { return maxY = levelCenter.y + (levelSize.y * 0.5f); }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(levelCenter, levelSize);
    }
}
