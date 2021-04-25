using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownTimer : MonoBehaviour
{
    Text textMesh;
    public float timeScale = 1.0f;
    public bool freezeCountdown = false;
    public float currentTime = 30.0f;
    int lastSecond;
    int currentTimeInt;
    AudioSource audioSource;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        currentTime = 31.0f;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeInt = (int)currentTime;
        if (lastSecond != currentTimeInt)
        {
            if (currentTimeInt <= 10)
            {
                lastSecond = currentTimeInt;
                audioSource.Play();
            }
        }

        if (!freezeCountdown)
        {
            currentTime -= Time.deltaTime * timeScale;
            
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                freezeCountdown = true;
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                if (player != null)
                    player.TakeDamage(1000);
            }

            textMesh.text = ((float)Mathf.Round(10000 * ((currentTime > 30f)?30f:currentTime)) / 10000).ToString((timeScale < 0.05f)?"0.0000":"0.00");
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
