/*
Abstract class for everything that needs hp, shield, and being killed

TODO : Add a roaming path, if pacific, passive or agressive
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractEnemy : MonoBehaviour
{
    public float hp;
    protected bool isAlive {get; set;} = true;
    protected bool isInvulnerable {get; set;}
    protected bool canSeePlayer {get; set;}
    public float power;
    public float visionRange;
    public GameManager gameManager;
    new Collider2D collider2D;
    protected Rigidbody2D rigidbody2d;
    protected Animator animator;
    protected Transform player;
    protected Vector2 relativePos {get; set;}
    RaycastHit2D[] playerHit = new RaycastHit2D[1];

    protected void Start()
    {
        collider2D = GetComponent<Collider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isAlive", true);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        relativePos = player.position - transform.position;
        StartCoroutine("PlayerChecking");
    }

    protected void FixedUpdate()
    {
        relativePos = player.position - transform.position;
        // Debug.DrawRay(transform.position, relativePos, Color.yellow);
    }

    IEnumerator PlayerChecking() 
    {
        for(;;) 
        {
            if (isAlive)
                LookForPlayer();
            yield return new WaitForSeconds(.1f);
        }
    }

    public void LookForPlayer()
    {
        if (collider2D.Raycast(relativePos, playerHit, visionRange) > 0)
        {
            GameObject player = playerHit[0].collider.gameObject;
            if (player.tag == "Player")
            {
                
                if (player.GetComponent<PlayerController>().isAlive)
                {
                    canSeePlayer = true;
                    
                } else { canSeePlayer = false; }
                
            } else { canSeePlayer = false; }
        }
    }

    public void TakeDamage(float rawDmg)
    {
        if (!isInvulnerable)
        {
            hp -= rawDmg;
            isInvulnerable = true;
            if (isAlive && hp<=0)
                isAlive = false;

            AddRecoil(0.1f, -relativePos.normalized + Vector2.up);
            animator.SetTrigger("Blink");
        }
    }

    public void AddRecoil(float originalVelFactor, Vector2 newVelVector)
    {
        rigidbody2d.velocity *= originalVelFactor;
        rigidbody2d.velocity += newVelVector;
    }

    public void setVulnerable()
    {
        isInvulnerable = false;
    }

    public void Die()
    {   
        transform.Find("DeathParticles").gameObject.SetActive(true);
        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        foreach(Collider2D col in colliders)
        {
            // col.enabled = false;
            Destroy(col);
        }
    }

    public void Despawn()
    {
        Destroy(this.gameObject);
    }
}