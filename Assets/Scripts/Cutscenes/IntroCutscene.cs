using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    Animator animator;
    public AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName: "Level1");
    }

    public void PlayBeep()
    {
        audioSource.Play();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
