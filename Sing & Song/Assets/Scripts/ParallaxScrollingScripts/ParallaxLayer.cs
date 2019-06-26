using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactor;
    private Vector3 newPosition;

    public void Move(float delta)
    {
        newPosition = transform.localPosition;
        newPosition.x -= delta * parallaxFactor;

        transform.localPosition = newPosition;
    }
}
