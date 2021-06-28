using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldController : MonoBehaviour
{

    // Variables
    public int goldAmount = 50; // Used to keep track of the players gold value
    Text gold; // Variable needed to make a connection to the gold text object

    // Use this for initialization
    void Start ()
    {
        // Makes a reference to the gold text object
        gold = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Sets the text value to "Gold: goldAmount variable"
        gold.text = "Gold: " + goldAmount;
	}

    public void ChangeGoldAmount(int changeAmount)
    {
        goldAmount += changeAmount;
    }
    public int GetGoldAmount()
    {
        return goldAmount;
    }
}
