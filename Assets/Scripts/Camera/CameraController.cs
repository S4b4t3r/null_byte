/*
(Attached to the main Camera)
Moves the camera to follow the player inside the CameraBounds of the background tilemap 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public float cameraShake;
    GameObject player;
    Rigidbody2D playerRb;
    Vector3 targetPosition; 
    Vector3 currentPosition;
    new Camera camera;
    Tilemap bgTilemap;
    
    struct CameraBounds
    {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
    }

    CameraBounds cameraBounds;
    public Vector3 targetOffset = new Vector3(1.6f, .4f, -10f);

    bool boundMode = false;

    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        camera = GetComponent<Camera>();

        // RecalculateBounds();

        targetPosition = player.transform.position + targetOffset;

        // Go to player at start
        if (boundMode)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, cameraBounds.xMin, cameraBounds.xMax);
            targetPosition.y = Mathf.Clamp(targetPosition.y, cameraBounds.yMin, cameraBounds.yMax);
        }

        currentPosition = transform.position;
    }

    void Update()
    {
        
    }

    
    void FixedUpdate()
    {
        // Get target position
        targetPosition = (player.transform.position + (Vector3)playerRb.velocity*.6f) + targetOffset;
        // Clamp inside map CameraBounds
        if (boundMode)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, cameraBounds.xMin, cameraBounds.xMax);
            targetPosition.y = Mathf.Clamp(targetPosition.y, cameraBounds.yMin, cameraBounds.yMax);
        }

        // Smooth towards player
        currentPosition = targetPosition - (targetPosition - currentPosition)*(0.98f);

        transform.position = currentPosition;

        // Round to 0.005f(pixel perfect)
        transform.position = new Vector3(Mathf.Round(transform.position.x*200)*.005f+.0001f, // Epsilon to prevent glitches with parallax tilemapped backgrounds
                                         Mathf.Round(transform.position.y*200)*.005f+.0001f,
                                         -10f);
        
    }

    IEnumerator CameraShake(float cameraShake)
    {
        for(;;)
        {
            if (cameraShake > 0.005f) // Treshold
            {
                cameraShake *= .95f;
                transform.position = currentPosition + new Vector3(Random.Range(-cameraShake, cameraShake), Random.Range(-cameraShake, cameraShake), 0f);;
            } else { cameraShake = 0f; break; }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator CameraZoom(float zoomFactor)
    {
        for(;;)
        {
            float diff = camera.orthographicSize - zoomFactor;
            if (Mathf.Abs(diff) > 0.005f) // Treshold
            {
                // TODO : I might never need it actually.
                
            } else { camera.orthographicSize = zoomFactor; break; }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    public void RecalculateBounds()
    {
        if (bgTilemap == null)
        {
            if (GameObject.FindGameObjectWithTag("Tilemap Background") && GameObject.FindGameObjectWithTag("Tilemap Background").TryGetComponent<Tilemap>(out bgTilemap))
            {
                boundMode = true;
                bgTilemap.CompressBounds();
                cameraBounds = new CameraBounds();

                // TODO : 16f and 9f depend on the screen aspect ratio, so it would be wise to calculate them accorting to the user's screen's aspect ratio.
                // .2f is one tile size in units
                cameraBounds.xMin = (bgTilemap.cellBounds.xMin + 16f)*.2f;
                cameraBounds.xMax = (bgTilemap.cellBounds.xMax - 16f)*.2f;
                cameraBounds.yMin = (bgTilemap.cellBounds.yMin + 9f)*.2f;
                cameraBounds.yMax = (bgTilemap.cellBounds.yMax - 9f)*.2f;
            } else {
                boundMode = false;
            }
        }
    }
}
