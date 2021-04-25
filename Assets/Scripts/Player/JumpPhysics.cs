using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPhysics : MonoBehaviour
{
    // Start is called before the first frame update
    public float fallFactor = 1.5f;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Falling
        if (rb.velocity.y < -0.01f) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallFactor * Time.deltaTime;
        }
    }
}
