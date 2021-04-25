/*
Handles all the scene switching work.
    LoadScene(sceneName) switches scenes instantly.
    QuitGame() quits the... game.
    FadeToLevel(string level) fades to black and transitions to the next level (intended for use in gameplay)
*/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    Animator animator;
    GameManager gameManager;
    private string levelToLoad;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameManager == null)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager.OnLevelLoad(scene.name);
        animator.SetBool("Right", true);
        
        // Debug.Log(mode);
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void FadeToLevelDeath(string level)
    {
        levelToLoad = level;
        animator.SetTrigger("FadeOutDeath");
    }

    public void FadeToLevel(string level)
    {
        levelToLoad = level;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        LoadScene(levelToLoad);
        Debug.Log("FadeComplete");
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(sceneName: scene);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }
}

