using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    PlayerController playerpc;
    GameManager gameManager;
    public string level;
    public AudioSource audioSource;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerpc = player.GetComponent<PlayerController>();
        if(player == null) { Debug.LogWarning("[Door]" + gameObject.name + " : Player not found!"); }
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            audioSource.Play();
            gameManager.SwitchLevel(level);
        }
    }   
}
