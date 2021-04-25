/*
Tracks the game state, scene transitions, objects and saves.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager gameManager;
    public CountdownTimer countdownTimer;
    PlayerController player;
    LevelChanger levelChanger;
    Coroutine coroutine;
    static CameraController cameraController;

    void Awake()
    {
        // Make sure only one instance exists
        if (gameManager == null)
        {
            DontDestroyOnLoad(this);
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        FindGameObjects();
    }

    void FindGameObjects()
    {
        GameObject playergc = GameObject.FindGameObjectWithTag("Player");
        player = playergc.GetComponent<PlayerController>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    public void PlayerHit()
    {
        StopAllCoroutines();
        coroutine = StartCoroutine("DamageTimeFreezeCoroutine");
    }

    public void PlayerShoot()
    {
        cameraController.StartCoroutine("CameraShake", 0.02f);
    }

    public void EnemyShoot(float intensity)
    {
        cameraController.StartCoroutine("CameraShake", intensity);
    }

    IEnumerator DamageTimeFreezeCoroutine()
    {
        Time.timeScale = 0f;
        cameraController.StartCoroutine("CameraShake", 0.1f);
        yield return new WaitForSecondsRealtime(.2f);
        for (int i = 0; i <= 20; i++)
        {
            Time.timeScale = i*0.05f;
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    public void OnLevelLoad(string sceneName)
    {
        /*
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            Door doorScript = door.GetComponent<Door>();
            if (doorScript.level == lastRoom && doorScript.doorLetter == lastDoor)
            {
                Debug.Log("New room");
                player.transform.position = door.transform.position + new Vector3(0f, .1f, 0f);
                player.TransitionLevel(doorScript.direction);
                break;
            }
        }
        */
        // player.TransitionLevel();

        switch (sceneName){
            case "Level1":
                countdownTimer.timeScale = 1.0f;
                break;
            case "Level2":
                countdownTimer.timeScale = 0.05f;
                break; 
            case "Level3":
                countdownTimer.timeScale = 0.025f;
                break;
            case "Outro":
                countdownTimer.Destroy();
                levelChanger.Destroy();
                break;
        }
        countdownTimer.freezeCountdown = false;

        FindGameObjects();
    }

    public void SwitchLevel(string level)
    {
        levelChanger = GameObject.FindGameObjectWithTag("LevelChanger").GetComponent<LevelChanger>();
        levelChanger.FadeToLevel(level);
        player.TransitionLevel();
        countdownTimer.freezeCountdown = true;
    }

    public void RestartLevel()
    {
        levelChanger = GameObject.FindGameObjectWithTag("LevelChanger").GetComponent<LevelChanger>();
        levelChanger.FadeToLevelDeath("Level1");
        countdownTimer.currentTime = 31.0f;
    }
}
