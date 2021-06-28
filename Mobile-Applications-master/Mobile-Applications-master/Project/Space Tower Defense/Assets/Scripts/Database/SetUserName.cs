using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetUserName : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = GetComponent<Text>().text + PlayerPrefs.GetString("UserName");
    }	
	
}
