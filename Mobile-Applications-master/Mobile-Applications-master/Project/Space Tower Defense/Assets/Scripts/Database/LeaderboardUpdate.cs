using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LeaderboardUpdate : MonoBehaviour {

    
    private Database_Control DB;
    private Text text;
    private Text scoreText;
	// Use this for initialization
	void Start () {
        DB = GameObject.Find("Database Controller").GetComponent<Database_Control>();
        text = GetComponent<Text>();
        scoreText = transform.parent.GetChild(1).GetComponent<Text>();

        int rank = 0;
        foreach (KeyValuePair<string, float> LeaderboardItem in DB.leaderboard.leaderboard())
        {
            rank++;
            text.text = text.text + "Rank #" + rank.ToString() + "\t" + LeaderboardItem.Key + "\n";
            scoreText.text = scoreText.text + LeaderboardItem.Value.ToString()+ "\n" ;

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
