using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    float angle;
    public float factor = 100f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        angle += Time.deltaTime * factor;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
