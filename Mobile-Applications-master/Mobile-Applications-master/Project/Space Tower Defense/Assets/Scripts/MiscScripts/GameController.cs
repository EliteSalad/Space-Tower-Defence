using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // [Header("Database")]
    //This needs the Database Controller object dragged and dropped onto it:
    //public Database_Control Database_Controller;

    [Header("StandardEnemy")]
    public GameObject StandardEnemyPrefab;
    [Header("SlowEnemy")]
    public GameObject SlowEnemyPrefab;
    [Header("StealthyEnemy")]
    public GameObject StealthyEnemyPrefab;
    [Header("FastEnemy")]
    public GameObject FastEnemyPrefab;
    [Header("Other")]
    [Tooltip("Base Health")]

    [Header("Resources")]

    // public List<GameObject> Enemies2;
    [Header("Player Base")]

    public int Health = 10;
    public List<Transform> EnemySpawnLocations;

    // public Transform EnemySpawnLocation;

    [Header("Canvas")]
    public GameObject NextWaveButton;

    private string LevelData;
    private int currentWaveNumber = 1;
    private int StandardEnemies = 0;
    private int SlowEnemies = 0;
    private int StealthyEnemies = 0;
    private int FastEnemies = 0;
    private int maxWaveNumber;
    public GameObject CurrentEnemy = null;
    public bool wait = true;
    public static GameController Instance { get; private set; }

    public List<GameObject> Enemies = null;
    public GameObject waveController;
    private Text HealthText;

    [Header("Paths")]
    private List<List<GameObject>> pathsPossible;
    public List<GameObject> firstPathPossible;
    public List<GameObject> secondPathPossible;

    [Header("Database")]
    private Database_Control DB;

    private byte CurrentMedalsEarned = 0;
    // private string path;

    void Start()
    {
        DB = GameObject.Find("Database Controller").GetComponent<Database_Control>();
        //DB.GameState.Load();
        //path = Application.persistentDataPath;
        //FileStream file = new FileStream(path + "/" + "Level" + PlayerPrefs.GetInt("LevelNumber", 1) + ".txt", FileMode.OpenOrCreate);
        //StreamWriter writer = new StreamWriter(file);
        // writer.WriteLine("0/00/00/00/00");
        //for testing only
        ReadLevelData();
        HealthText = GameObject.Find("Health").GetComponent<Text>();
        HealthText.text = "Health: " + Health.ToString();
        pathsPossible = new List<List<GameObject>>();
        pathsPossible.Add(firstPathPossible);
        pathsPossible.Add(secondPathPossible);
        //ReadNextWaveData(currentWaveNumber);
    }

    private void Update()
    {
        if (Health <= 0)
        {
            PlayerPrefs.SetString("Level", "MainMenu");
            SceneManager.LoadScene("Loading");
        }
        if (currentWaveNumber == maxWaveNumber)
        {
            if (checkEnemies() == 0)
            {
                StartCoroutine(EndGame());
            }
        }
    }
    public void reduceHealth()
    {
        Health--;
        HealthText.text = "Health: " + Health.ToString();
    }


    void ReadLevelData()
    {
        TextAsset asset = Resources.Load<TextAsset>(PlayerPrefs.GetString("Level", "Level01"));
        string loadPath = asset.text;

        //Read the text from directly from the test.txt file
        /// StreamReader reader = new StreamReader(loadPath);
        LevelData = loadPath;// reader.ReadToEnd();
                             // reader.Close();
        ReadNextWaveData(1);
    }

    bool ReadNextWaveData(int waveNumber)
    {
        char[] b = new char[LevelData.Length];
        if (waveNumber <= 5)
        {
            using (StringReader sr = new StringReader(LevelData))
            {
                sr.Read(b, 0, LevelData.Length);
                maxWaveNumber = 0;
                for (int i = 1; i < LevelData.Length; i++)
                {

                    if (b[i] == '\n' && (b[i + 1] - '0') == waveNumber)
                    {
                        StandardEnemies = (((b[i + 3] - '0') * 10) + (b[i + 4]) - '0');
                        SlowEnemies = (((b[i + 6] - '0') * 10) + (b[i + 7]) - '0');
                        FastEnemies = (((b[i + 9] - '0') * 10) + (b[i + 10]) - '0');
                        StealthyEnemies = (((b[i + 12] - '0') * 10) + (b[i + 13]) - '0');
                    }
                    if (b[i] == '\n')
                    {
                        maxWaveNumber++;
                    }
                }
                waveController.GetComponent<WaveController>().maxWave(maxWaveNumber - 1);
            }
            return true;
        }
        else
        {
            StandardEnemies = 0;
            SlowEnemies = 0;
            FastEnemies = 0;
            StealthyEnemies = 0;
            return false;
        }
    }

    void CreateEnemies()
    {
        InvokeRepeating("CreateStandardEnemies", 0f, 1.0f);
    }




    void CreateStandardEnemies()
    {
        if (StandardEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(StandardEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }
        if (--StandardEnemies <= 0)
        {
            CancelInvoke("CreateStandardEnemies");
            InvokeRepeating("CreateSlowEnemies", 0.0f, 0.8f);
        }
    }

    void CreateSlowEnemies()
    {
        if (SlowEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(SlowEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }
        if (--SlowEnemies <= 0)
        {
            CancelInvoke("CreateSlowEnemies");
            InvokeRepeating("CreateStealthyEnemies", 0f, 0.5f);

        }
    }

    void CreateStealthyEnemies()
    {
        if (StealthyEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(StealthyEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }
        if (--StealthyEnemies <= 0)
        {
            CancelInvoke("CreateStealthyEnemies");
            InvokeRepeating("CreateFastEnemies", 0f, 1.2f);
        }
    }

    void CreateFastEnemies()
    {
        if (FastEnemies > 0)
        {
            int SpawanLocation = UnityEngine.Random.Range(0, EnemySpawnLocations.Count);
            CurrentEnemy = Instantiate(FastEnemyPrefab, EnemySpawnLocations[SpawanLocation].position, EnemySpawnLocations[SpawanLocation].rotation);
            CurrentEnemy.GetComponent<EnemyController>().setPath(pathsPossible[SpawanLocation]);
            AddEnemy(CurrentEnemy);
        }

        if (--FastEnemies <= 0)
        {
            CancelInvoke("CreateFastEnemies");
            NextWaveButton.GetComponent<Button>().interactable = true;
            currentWaveNumber++;
            StartCoroutine(NextWaveTimer());
            waveController.GetComponent<WaveController>().lastEnemySent = true;
        }
    }

    public void NextWave()
    {
        waveController.GetComponent<WaveController>().increaseWave();
        NextWaveButton.GetComponent<Button>().interactable = false;
        NextWaveButton.transform.GetChild(0).GetComponent<Text>().enabled = false;

        if (!ReadNextWaveData(currentWaveNumber))
        {
            EndOfWaves();
        }
        CreateEnemies();
    }

    void EndOfWaves()
    {
        CurrentMedalsEarned = CalculateMedalsEarned();
        if (!CheckLevel(Convert.ToInt32(PlayerPrefs.GetString("Level", "Level01").Substring(5, 2))))
        {
            DB.GameState.Level_Data.Add(new Level_Info(PlayerPrefs.GetString("Level", "Level01"), Convert.ToByte(PlayerPrefs.GetString("Level", "Level01").Substring(5, 2)), CurrentMedalsEarned));
            DB.GameState.Current_Medals += CurrentMedalsEarned;
            DB.GameState.Total_Medals_Earned += CurrentMedalsEarned;
            PlayerPrefs.SetInt("Medals", PlayerPrefs.GetInt("Medals", 0) + CurrentMedalsEarned);
            DB.GameState.Save();
        }
        else
        {
            if (CurrentMedalsEarned > DB.GameState.Level_Data[Convert.ToInt32(PlayerPrefs.GetString("Level", "Level01").Substring(5, 2)) - 1].Medals)
            {
                int differenceInMedals = CurrentMedalsEarned - DB.GameState.Level_Data[Convert.ToInt32(PlayerPrefs.GetString("Level", "Level01").Substring(5, 2)) - 1].Medals;
                DB.GameState.Level_Data[Convert.ToInt32(PlayerPrefs.GetString("Level", "Level01").Substring(5, 2)) - 1].Medals = CurrentMedalsEarned;
                DB.GameState.Current_Medals += (byte)differenceInMedals;
                DB.GameState.Total_Medals_Earned += (byte)differenceInMedals;
                PlayerPrefs.SetInt("Medals", PlayerPrefs.GetInt("Medals", 0) + differenceInMedals);
                DB.GameState.Save();
            }
        }
        PlayerPrefs.SetFloat("Highscore", (PlayerPrefs.GetFloat("Highscore", 0.0f) + (GameObject.Find("GoldText").GetComponent<GoldController>().goldAmount * CurrentMedalsEarned)));
        DB.leaderboard.Update(PlayerPrefs.GetString("UserName", DB.d.DUI), PlayerPrefs.GetFloat("Highscore", 0.0f));
        DB.leaderboard.Save();
        PlayerPrefs.SetString("Level", "MainMenu");
        SceneManager.LoadScene("Loading");
    }

    public void AddEnemy(GameObject Enemy)
    {
        Enemies.Add(Enemy);
    }

    public int checkEnemies()
    {
        int enemiesLeft = 0;
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i] != null)
            {
                enemiesLeft++;
            }
        }
        return enemiesLeft;
    }

    IEnumerator NextWaveTimer()
    {
        yield return new WaitForSeconds(3.0f);
        if (currentWaveNumber != maxWaveNumber)
        {
            NextWaveButton.transform.GetChild(0).GetComponent<Text>().enabled = true;
        }        
    }

    private byte CalculateMedalsEarned()
    {
        if (Health >= 10)
        {
            return 3;
        }
        else if (Health >= 5 && Health < 10)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private bool CheckLevel(int value)
    {
        try
        {
            if (DB.GameState.Level_Data[value - 1] != null)
                return true;
        }
        catch (Exception e)
        {
            DB.d.Log(false, "Couldn't find level data for level number: " + value + " > " + e.Message, true);
            return false;
        }
        return false;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3);
        NextWaveButton.transform.GetChild(0).GetComponent<Text>().text = "END GAME";
        NextWaveButton.transform.GetChild(0).GetComponent<Text>().enabled = true;

    }

}
