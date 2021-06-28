using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemController : MonoBehaviour
{

    // Variables
    public static int gemAmount = 0; // Used to keep track of the players gem value
    Text gem; // Variable needed to make a connection to the gem text object

    // Use this for initialization
    void Start()
    {
        // Makes a reference to the gem text object
        gem = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the text value to "Gems: gemAmount variable"
        gem.text = "Gems: " + PlayerPrefs.GetInt("Gems", 0);
    }
}
