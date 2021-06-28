using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("MusicToggle", 1) == 1)
        {
            GetComponent<AudioSource>().volume = 1;
        }
        else if (PlayerPrefs.GetInt("MusicToggle", 0) == 0)
        {
            GetComponent<AudioSource>().volume = 0;
        }
    }
}
