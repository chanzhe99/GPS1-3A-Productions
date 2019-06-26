using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionSetter : CameraController
{
    private ParallaxBackground parallaxBG;
    [SerializeField] private CameraController cameraController;

    private void Start()
    {
        parallaxBG = GetComponentInParent<ParallaxBackground>();
        cameraController = GetComponent<CameraController>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        cameraController.cameraOffset = new Vector3(0, 0, -10);
        parallaxBG.enabled = false;
    }
    /*
    [SerializeField] private float cameraSmoothingSpeed;
    [SerializeField] Transform singTransform;
    [SerializeField] private Vector3 cameraOffsetSet;

    private Vector3 cameraVelocity = Vector3.zero;    
    private Vector2 cameraDesiredPosition;

    private void Start()
    {
        transform.position = Vector3.SmoothDamp(transform.position, singTransform.position, ref cameraVelocity, cameraSmoothingSpeed);
    }

    private void FixedUpdate()
    {
        if (cameraDesiredPosition.x != transform.position.x)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(cameraDesiredPosition.x, cameraDesiredPosition.y, cameraOffset.z), ref cameraVelocity, cameraSmoothingSpeed);
        }
    }

    private void OnTriggerEnter2D(Collision2D collision)
    {
        cameraOffset = cameraOffsetSet;
    }
    */
}
