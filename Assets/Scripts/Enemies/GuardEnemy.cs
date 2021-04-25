using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public  class GuardEnemy : AbstractEnemy
{
    bool couldSeePlayer = false;
    public GameObject weapon;

    public float timeBetweenShots = 0.4f;
    float shootCountdown = 0.4f;
    public SpriteRenderer weaponSr;
    public LineRenderer bulletTrace;
    public AudioSource audioSourceShoot;
    public AudioSource audioSourceHurt;

    new void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (canSeePlayer)
        {
            if (!couldSeePlayer)
            {
                // Spotted! Trigger some animation
                couldSeePlayer = true;
            }

            Vector2 aimVector = relativePos.normalized;

            weapon.transform.localPosition = aimVector*0.2f;

            float angle = Vector3.SignedAngle(Vector3.right, weapon.transform.localPosition, Vector3.forward);
            weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            if (angle > 90f || angle < -90f)
                weaponSr.flipY = true;
            else 
                weaponSr.flipY = false;


            shootCountdown -= Time.deltaTime;
            if (shootCountdown < 0f)
            {
                StartCoroutine(Shoot(aimVector));
                    shootCountdown = timeBetweenShots;
            }

        } else if (couldSeePlayer)
        {
            // Lost em'...
            couldSeePlayer = false;
            shootCountdown = timeBetweenShots;
        }

        if(!isAlive)
        {
            if (animator.GetBool("isAlive"))
            {                
                animator.SetBool("isAlive", false);
                canSeePlayer = false;
                couldSeePlayer = false;
                shootCountdown = timeBetweenShots;
                rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                audioSourceHurt.Play();
            }
        }
    }

    public IEnumerator Shoot(Vector2 cursorDirection)
    {
        // Add random to aim
        cursorDirection += new Vector2(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f));

        audioSourceShoot.Play();

        gameManager.EnemyShoot(0.02f);

        RaycastHit2D[] hitInfo = new RaycastHit2D[1];
        GetComponent<Collider2D>().Raycast(cursorDirection, hitInfo, 5.5f);

        if (hitInfo[0])
        {
            PlayerController player = hitInfo[0].transform.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(10);
            } 

            bulletTrace.SetPosition(0, (Vector2)transform.position + (cursorDirection*.2f));
            bulletTrace.SetPosition(1, hitInfo[0].point);
            /*
            Debug.Log(hitInfo[0].transform.name);
            Debug.DrawRay(hitInfo[0].point, -cursorDirection, Color.yellow, 1f);
            */
        } else {
            bulletTrace.SetPosition(0, (Vector2)transform.position + (cursorDirection*.2f));
            bulletTrace.SetPosition(1, (Vector2)transform.position + (cursorDirection*7f));
        }

        
        for (int i = 0; i < 6; i++)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0.0f), new GradientAlphaKey(1-(float)i/5 , 1.0f) }
            );
            bulletTrace.colorGradient = gradient;

            yield return new WaitForSeconds(0.02f);
        }
    }
    
}