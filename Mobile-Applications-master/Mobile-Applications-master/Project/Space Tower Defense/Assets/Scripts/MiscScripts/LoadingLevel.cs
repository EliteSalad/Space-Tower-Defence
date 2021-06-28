using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
       // StartCoroutine(LoadYourAsyncScene());
            StartCoroutine(LoadYourAsyncScene());
    }
	
	// Update is called once per frame
	void Update () {
        // Use a coroutine to load the Scene in the background
        
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(PlayerPrefs.GetString("Level", "Level01"));

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
