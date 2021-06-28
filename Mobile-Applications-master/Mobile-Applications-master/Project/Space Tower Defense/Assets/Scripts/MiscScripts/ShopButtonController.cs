using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonController : MonoBehaviour {

    // ID = currency using
    private string ID;
    private string subID;
    // value = cost
    private int value;

    private Database_Control DB;

    private void Start()
    {
        DB = GameObject.Find("Database Controller").GetComponent<Database_Control>();
    }
    // Update is called once per frame
    void Update()
    {
        if (ID == "G")
        {
            int gemCounter = PlayerPrefs.GetInt("Gems", 0) - value;
            if (gemCounter < 0)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
        }
        else if(ID == "P")
        {
            GetComponent<Button>().interactable = true;
        }
        else if (ID == "M")
        {
            int medalCounter = DB.GameState.Current_Medals - value;
            if (medalCounter < 0)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
        }
    }

    public void SetValues(string idValue, string subIDValue, int valueOfButton)
    {
        ID = idValue;
        subID = subIDValue;
        value = valueOfButton;
    }

    public void BoughtItem()
    {
        if (ID == "P")
        {
            PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems", 0) + value);
        }
        else if (ID == "G")
        {
            int gemCounter = PlayerPrefs.GetInt("Gems", 0) - value;
            if (gemCounter >= 0)
            {
                PlayerPrefs.SetInt("Gems", gemCounter);
                if (subID == "Speed Up")
                {
                    PlayerPrefs.SetInt("SpeedUp", PlayerPrefs.GetInt("SpeedUp",0)+1);
                }
                else if (subID == "Kill All")
                {
                    PlayerPrefs.SetInt("KillAll", PlayerPrefs.GetInt("KillAll", 0) + 1);
                }
                else if (subID == "Slow Down")
                {
                    PlayerPrefs.SetInt("SlowDown", PlayerPrefs.GetInt("SlowDown", 0) + 1);
                }
            }
        }
        else if (ID == "M")
        {
            int medalCounter = DB.GameState.Current_Medals - value;
            if (medalCounter >= 0)
            {
                DB.GameState.Current_Medals -= (byte)value;
                PlayerPrefs.SetInt("Medals", PlayerPrefs.GetInt("Medals", 0) - value);
                PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems", 0) + value);

            }
        }
        DB.GameState.Current_Gems = PlayerPrefs.GetInt("Gems", 0);
    }
    
}
