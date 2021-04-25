/*
Apply to an object you want to parallax scroll with the camera.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Tooltip("Intensity of the parallax effect (negative values for foreground elements)")]
    public float intensity = 0f;
    Transform cameraTransform;
    void Start()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Round((cameraTransform.position.x * +intensity)*100f)*.01f,
                                         Mathf.Round((cameraTransform.position.y * +intensity)*100f)*.01f);
    }
}
