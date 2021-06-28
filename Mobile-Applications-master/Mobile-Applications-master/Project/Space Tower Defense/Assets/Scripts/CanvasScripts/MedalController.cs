using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedalController : MonoBehaviour {

    // Variables
    public static int medalAmount = 0; // Used to keep track of the players medal value
    Text medal; // Variable needed to make a connection to the medal text object
              // Use this for initialization

    void Start () {
        medal = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        // Sets the text value to "medals: medalAmount variable"
        medal.text = "Medals: " + PlayerPrefs.GetInt("Medals", 0);
    }
}
