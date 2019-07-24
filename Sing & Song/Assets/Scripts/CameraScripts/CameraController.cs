using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Component Variables
    private Transform playerTransform;
    #endregion
    #region Camera Position Variables
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0f, -100f);
    private Vector3 cameraTargetPosition;
    private CameraClampSetter cameraClampSetter;
    private float minX, maxX, minY, maxY;
    #endregion
    
    private void Start() { playerTransform = GameObject.FindGameObjectWithTag("Player").transform; }
    private void Update()
    {
        minX = cameraClampSetter.getMinX() + (this.GetComponent<Camera>().orthographicSize * 16 / 9);
        maxX = cameraClampSetter.getMaxX() - (this.GetComponent<Camera>().orthographicSize * 16 / 9);
        minY = cameraClampSetter.getMinY() + (this.GetComponent<Camera>().orthographicSize);
        maxY = cameraClampSetter.getMaxY() - (this.GetComponent<Camera>().orthographicSize);
    //}
    //private void FixedUpdate()
    //{
        cameraTargetPosition = playerTransform.position + cameraOffset;
        cameraTargetPosition.x = Mathf.Clamp(cameraTargetPosition.x, minX, maxX);
        cameraTargetPosition.y = Mathf.Clamp(cameraTargetPosition.y, minY, maxY);
        if(cameraTargetPosition != this.transform.position)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, cameraTargetPosition, 1f);
        }
    }
    public void GetCameraClampSetter(CameraClampSetter clampSetter) { cameraClampSetter = clampSetter; }
}
