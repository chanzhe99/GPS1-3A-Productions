using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera Component + Other Objects Variables
    // Camera Follow Variables
    [SerializeField] private float cameraSmoothingSpeed;
    [SerializeField] public Vector3 cameraOffset;
    private Vector3 cameraVelocity = Vector3.zero;
    private float cameraDesiredPositionX;
    private float cameraDesiredPositionY;
    private Vector3 cameraSmoothedPosition;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Updates every frame after "void Update()" (For camera follow)
    private void FixedUpdate()
    {
        cameraDesiredPositionX = playerTransform.position.x + cameraOffset.x;
        cameraDesiredPositionY = playerTransform.position.y + cameraOffset.y;

        if (cameraDesiredPositionX != transform.position.x)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(cameraDesiredPositionX, cameraOffset.y, cameraOffset.z), ref cameraVelocity, cameraSmoothingSpeed);
        }
        //else
        //{
        //    transform.position = cameraDesiredPosition;
        //}
    }
}
