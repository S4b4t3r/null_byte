/*
The main player controller.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    Rigidbody2D rigidbody2d;
    Animator animator;
    CapsuleCollider2D capsuleCollider2d;
    public Material materialWhite;
    Material materialDefault;
    public GameObject aimCursor;
    SpriteRenderer acSr;
    Coroutine co;
    new Camera camera;
    GameManager gameManager;
    public struct PlayerGun {
        public bool ready;
        public float currentCooldown;
        public Color bulletColor;
    }
    PlayerGun playerGun;
    public LineRenderer bulletTrace;

    // Movement & Animation
    public bool isGrounded = true;
    public bool isSliding = false;
    bool invulnerable = false;
    public bool controlled;
    public float maxhp = 4;
    public float hp = 4;
    public bool isAlive = true;
    float inputHorizontal = 0f;
    float inputVertical = 0f;
    RaycastHit2D[] groundHit = new RaycastHit2D[1];
    public AudioSource audioSourceShoot;
    public AudioSource audioSourceHurt;
    public AudioSource audioSourceDeath;
    

    // Debug Graph
    public AnimationCurve debugGraph = new AnimationCurve();
    // Usage : debugGraph.AddKey(Time.realtimeSinceStartup, value)

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
        acSr = aimCursor.GetComponent<SpriteRenderer>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerGun.bulletColor = new Color(1f, 0.9f, 0.4f);
        playerGun.ready = true;
        animator.SetBool("IsAlive", true);
    }

    Color enabledColor = new Color(1,1,1,1);
    Color disabledColor = new Color(1,1,1,0.4f);
    void Update()
    {
        if (controlled) 
        {
            inputHorizontal = 0f;
            inputVertical = 0f;
        } else {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
            inputVertical = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Fire1")) // Left Click
            {
                if (playerGun.ready)
                    StartCoroutine(Shoot());
            }
            if (Input.GetButtonDown("Jump")) // Space
            {
                if (isGrounded)
                    Jump();
            }
            if (Input.GetButtonDown("Fire2")) // Right Click
            {
                // IDK
            }
            if (Input.GetButtonDown("Submit")) // Enter or something
            {
                // Neither
            }
        }

        if (playerGun.currentCooldown > 0f)
        {
            playerGun.ready = false;
            playerGun.currentCooldown -= Time.deltaTime;
            
            if (playerGun.currentCooldown < 0f)
            {
                playerGun.currentCooldown = 0f;
                playerGun.ready = true;
            }
        }

        acSr.color = playerGun.ready ? enabledColor : disabledColor;

        if(!isAlive)
        {
            animator.SetBool("IsAlive", false);
            controlled = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 cursorDirection = GetCursorDirection();

        // Left Right movement
        rigidbody2d.AddForce((3f*Vector3.right) * (3f*(inputHorizontal-(rigidbody2d.velocity.x*.55f)))); 

        // Cursor positionning
        aimCursor.transform.localPosition = cursorDirection*0.2f;
        float angle = Vector3.SignedAngle(Vector3.right, aimCursor.transform.localPosition, Vector3.forward);
        aimCursor.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        if (angle > 90f || angle < -90f)
            acSr.flipY = true;
        else 
            acSr.flipY = false;

        animator.SetFloat("HorizontalVel", rigidbody2d.velocity.x);
        animator.SetFloat("VerticalVel", rigidbody2d.velocity.y);

        // Ground check
        isGrounded = (capsuleCollider2d.Raycast(Vector2.down, groundHit, 0.25f) > 0);
    }

    public IEnumerator Shoot()
    {
        gameManager.PlayerShoot();
        audioSourceShoot.Play();

        Vector2 cursorDirection = GetCursorDirection();
        RaycastHit2D[] hitInfo = new RaycastHit2D[1];
        capsuleCollider2d.Raycast(cursorDirection, hitInfo, 3.5f);

        if (hitInfo[0])
        {
            AbstractEnemy enemy = hitInfo[0].transform.GetComponent<AbstractEnemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(40);
            } 

            bulletTrace.SetPosition(0, (Vector2)transform.position + (cursorDirection*.2f));
            bulletTrace.SetPosition(1, hitInfo[0].point);
            /*
            Debug.Log(hitInfo[0].transform.name);
            Debug.DrawRay(hitInfo[0].point, -cursorDirection, Color.yellow, 1f);
            */
        } else {
            bulletTrace.SetPosition(0, (Vector2)transform.position + (cursorDirection*.2f));
            bulletTrace.SetPosition(1, (Vector2)transform.position + (cursorDirection*5f));
        }

        playerGun.currentCooldown = 0.25f;
        playerGun.ready = false;

        
        for (int i = 0; i < 6; i++)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(playerGun.bulletColor, 0.0f), new GradientColorKey(playerGun.bulletColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0.0f), new GradientAlphaKey(1-(float)i/5 , 1.0f) }
            );
            bulletTrace.colorGradient = gradient;

            yield return new WaitForSeconds(0.02f);
        }
    }

    public void Jump()
    {
        rigidbody2d.AddForce(Vector2.up*50f);
    }

    public void TransitionLevel()
    {
        controlled = true;
        // TODO : Diving animation into next level
        // co = StartCoroutine(Walk(direction, 1f));
        animator.SetTrigger("Dive");
    }

    public Vector3 GetCursorDirection()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        
        Vector3 mouse = Input.mousePosition;
        mouse.z = 0;
        mouse = camera.ScreenToWorldPoint(mouse);
        mouse.z = 0;

        return Vector3.Normalize(mouse - transform.position);
    }

    public void AddRecoil(float originalVelFactor, Vector2 newVelVector) // originalVelFactor is how much of the original velocity you want kept, newVelVector is the recoil vector.
    {
        rigidbody2d.velocity *= originalVelFactor;
        rigidbody2d.velocity += newVelVector;
    }

    public void setVulnerable()
    {
        invulnerable = false;
    }

    public void TakeDamage(float rawDmg)
    {
        if (!invulnerable)
        {
            hp -= rawDmg;
            // isInvulnerable = true;
            if (isAlive && hp<=0)
            {
                isAlive = false;
                audioSourceDeath.Play();
            }
            Debug.Log(hp);
            
            AddRecoil(0.1f, Vector2.up);
            animator.SetTrigger("Blink");
            audioSourceHurt.Play();
        }
    }

    public void Die()
    {
        
    }

    public void RestartLevel()
    {
        gameManager.RestartLevel();
    }
}
