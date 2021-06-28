using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaveController : MonoBehaviour
{
    public bool lastEnemySent = false;
    // Variables
    public int waveValue = 0; // Used to keep track of the amount of waves the player has completed
    Text waves; // Variable needed to make a connection to the wave counter text object
    public int wavesInLevel = 0;
    private GameObject gameManager;
    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");

        // Makes a reference to the wave counter text object
        waves = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    { 
        // Sets the text value to "Waves Completed: waveValue variable"
        waves.text = "Current wave: " + waveValue + "/" + wavesInLevel;
        if (lastEnemySent)
        {
            if (waveValue >= wavesInLevel)
            {
                if (gameManager.GetComponent<GameController>().checkEnemies() == 0)
                {
                    PlayerPrefs.SetString("Level", "MainMenu");
                    SceneManager.LoadScene("Loading");
                }
            }
            lastEnemySent = false;
        }
    }

    public void increaseWave()
    {
        waveValue++;
        if(waveValue > wavesInLevel)
        {
            waveValue = wavesInLevel;
        }
    }
    public void maxWave(int maxWave)
    {
        wavesInLevel = maxWave;
    }
}