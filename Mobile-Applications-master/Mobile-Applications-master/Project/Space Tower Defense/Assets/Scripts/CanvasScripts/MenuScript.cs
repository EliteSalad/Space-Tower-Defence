using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;


public class MenuScript : MonoBehaviour
{

    public List<GameObject> MenuItems;
    public GameObject MusicToggle;
    public List<GameObject> TowerSlots;
    private GameObject DBGO;
    private Database_Control DB;
    public float rateOfFireMultiplier = 1.0f;
    public List<GameObject> ListOfAbilities;
    public GameObject Nuke;

    private void Start()
    {
        DBGO = GameObject.Find("DatabaseCanvas");

        DB = GameObject.Find("Database Controller").GetComponent<Database_Control>();
        //DB.GameState.Load();
        for (int i = 0; i < TowerSlots.Count; i++)
        {
            Vector3 textPos = Camera.main.WorldToScreenPoint(TowerSlots[i].transform.GetChild(0).GetChild(0).GetComponent<TowerController>().TowerParent.transform.position);
            TowerSlots[i].transform.position = textPos;
        }        
    }

    public void Open(string Menu_Item_Name)
    {
        foreach (GameObject item in MenuItems)
        {
            if (item.name == Menu_Item_Name)
            {
                item.SetActive(true);
            }
            else
                item.SetActive(false);
        }
    }

    public void LoadLevel(string level)
    {
        if (level.Substring(0, 5) == "Level")
        {
            int level_Number = Convert.ToInt32(level.Substring(5, 2));
            if (level_Number == 01)
            {
                PlayerPrefs.SetString("Level", level);
                SceneManager.LoadScene("Loading");
            }
            else
            {
                try
                {
                    if (DB.GameState.Level_Data[level_Number - 2] != null)
                    {
                        PlayerPrefs.SetString("Level", level);
                        SceneManager.LoadScene("Loading");
                    }
                }
                catch (System.Exception e)
                {
                    DB.d.Log(false, "There was no data in next level. > " + e.Message, true);
                }

            }
        }
        else if (level.Substring(0, 8) == "MainMenu")
        {
            PlayerPrefs.SetString("Level", level);
            SceneManager.LoadScene("Loading");
        }
    }
    public void toggleMusic()
    {

        if (MusicToggle.GetComponent<Toggle>().isOn == false)
        {
            PlayerPrefs.SetInt("MusicToggle", 0);
            // GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume = 0;
        }
        else if (MusicToggle.GetComponent<Toggle>().isOn == true)
        {
            PlayerPrefs.SetInt("MusicToggle", 1);
            //GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume = 1;
        }
    }
    private void Update()
    {
        if (MusicToggle != null)
        {
            if (PlayerPrefs.GetInt("MusicToggle", 1) == 1)
            {
                MusicToggle.GetComponent<Toggle>().isOn = true;
            }
            else if (PlayerPrefs.GetInt("MusicToggle", 1) == 0)
            {
                MusicToggle.GetComponent<Toggle>().isOn = false;
            }
        }

        if (ListOfAbilities.Count > 0)
        {
            if (PlayerPrefs.GetInt("SpeedUp", 0) == 0)
            {
                ListOfAbilities[0].GetComponent<Button>().interactable = false;
                ListOfAbilities[0].GetComponent<SwitchIcon>().SetImage(2);
            }
            if (PlayerPrefs.GetInt("KillAll", 0) == 0)
            {
                ListOfAbilities[1].GetComponent<Button>().interactable = false;
                ListOfAbilities[1].GetComponent<SwitchIcon>().SetImage(2);
            }
            if (PlayerPrefs.GetInt("SlowDown", 0) == 0)
            {
                ListOfAbilities[2].GetComponent<Button>().interactable = false;
                ListOfAbilities[2].GetComponent<SwitchIcon>().SetImage(2);
            }
        }
    }
    public void HideButtons()
    {
        foreach (GameObject Tower_Slot in TowerSlots)
        {
            foreach (GameObject btn in Tower_Slot.transform.GetChild(0).transform.GetChild(0).GetComponent<TowerController>().Buttons_)
                btn.SetActive(false);

            foreach (GameObject STB in GameObject.FindGameObjectsWithTag("SpawnTowerButton"))
                STB.GetComponent<TowerController>().TowerParent.transform.GetChild(1).GetComponent<RangeIndicatorController>().switchRangeIndicatorOff();

        }
    }

    public void ContiueTitleButton(string level)
    {
        if (PlayerPrefs.GetString("UserName") == "")
        {
            ReturnCode check = DB.leaderboard.checkDatabase("SELECT Player_Name FROM tbl_Highscores WHERE Player_Name = '" + GameObject.Find("UserNameText").GetComponent<Text>().text + "';");
            if (check == ReturnCode.True)
            {
                GameObject.Find("UserNameExists").GetComponent<Text>().enabled = true;
                return;
            }
            else
            {
                PlayerPrefs.SetString("UserName", GameObject.Find("UserNameText").GetComponent<Text>().text);
                DontDestroyOnLoad(DBGO);
                PlayerPrefs.SetString("Level", level);
                SceneManager.LoadScene("Loading");
            }
        }
        else
        {
            DontDestroyOnLoad(DBGO);
            PlayerPrefs.SetString("Level", level);
            SceneManager.LoadScene("Loading");
        }
    }


    public void ClickPowerUp(string abilities)
    {
        switch (abilities)
        {
            case "SpeedUp":
                StartCoroutine(SpeedUpTurrets());
                break;
            case "SlowDown":
                StartCoroutine(SlowDownEnemies());
                break;
            case "KillAll":
                GameController GM = GameObject.Find("GameManager").GetComponent<GameController>();
                for (int i = 0; i < GM.Enemies.Count; i++)
                {
                    if (GM.Enemies[i] != null)
                        Destroy(GM.Enemies[i]);
                }
                StartCoroutine(KillEnemies());                
                break;
            default:
                break;
        }
    }

    public IEnumerator SpeedUpTurrets()
    {
        PlayerPrefs.SetInt("SpeedUp", PlayerPrefs.GetInt("SpeedUp", 0) - 1);
        ListOfAbilities[0].GetComponent<Button>().interactable = false;
        rateOfFireMultiplier = 2.0f;
        ListOfAbilities[0].GetComponent<SwitchIcon>().SetImage(0);
        yield return new WaitForSeconds(3);
        rateOfFireMultiplier = 1.0f;
        if (PlayerPrefs.GetInt("SpeedUp") != 0)
        {
            ListOfAbilities[0].GetComponent<Button>().interactable = true;
            ListOfAbilities[0].GetComponent<SwitchIcon>().SetImage(1);
        }
    }

    public IEnumerator SlowDownEnemies()
    {
        PlayerPrefs.SetInt("SlowDown", PlayerPrefs.GetInt("SlowDown", 0) - 1);
        GameController GM = GameObject.Find("GameManager").GetComponent<GameController>();
        ListOfAbilities[2].GetComponent<Button>().interactable = false;
        ListOfAbilities[2].GetComponent<SwitchIcon>().SetImage(0);
        float previousSpeed = 0.0f;
        for (int i = 0; i < GM.Enemies.Count; i++)
        {
            if (GM.Enemies[i] != null)
            {
                previousSpeed = GM.Enemies[i].GetComponent<NavMeshAgent>().speed;
                GM.Enemies[i].GetComponent<NavMeshAgent>().speed = previousSpeed / 2.0f;
            }
        }
        yield return new WaitForSeconds(3);
        for (int i = 0; i < GM.Enemies.Count; i++)
        {
            if (GM.Enemies[i] != null)
                GM.Enemies[i].GetComponent<NavMeshAgent>().speed = previousSpeed;
        }
        if (PlayerPrefs.GetInt("SlowDown") != 0)
        {
            ListOfAbilities[2].GetComponent<Button>().interactable = true;
            ListOfAbilities[2].GetComponent<SwitchIcon>().SetImage(1);
        }
    }

    public IEnumerator KillEnemies()
    {
        PlayerPrefs.SetInt("KillAll", PlayerPrefs.GetInt("KillAll", 0) - 1);
        ListOfAbilities[1].GetComponent<Button>().interactable = false;
        ListOfAbilities[1].GetComponent<SwitchIcon>().SetImage(0);        
        Instantiate(Nuke);
        yield return new WaitForSeconds(3);
        if (PlayerPrefs.GetInt("KillAll") != 0)
        {
            ListOfAbilities[1].GetComponent<Button>().interactable = true;
            ListOfAbilities[1].GetComponent<SwitchIcon>().SetImage(1);
        }
    }
}
