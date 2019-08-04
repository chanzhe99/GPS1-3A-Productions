using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMaster : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;
    private float oldPosition;

    private void Start()
    {
        oldPosition = transform.position.x;
    }

    private void Update()
    {
        if(this.gameObject.activeSelf)
        {
            this.transform.position = mainCamera.transform.position;
        }
        if(transform.position.x != oldPosition)
        {
            if(onCameraTranslate != null)
            {
                float delta = oldPosition - transform.position.x;
                onCameraTranslate(delta);
            }
            oldPosition = transform.position.x;
        }
    }
}
