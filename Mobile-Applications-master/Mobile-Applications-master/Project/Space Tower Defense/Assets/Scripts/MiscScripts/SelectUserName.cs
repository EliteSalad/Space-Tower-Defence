using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUserName : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(PlayerPrefs.GetString("UserName") == "")
        {
            transform.parent.GetChild(3).gameObject.SetActive(true);
            GetComponent<Button>().interactable = false;
            transform.GetChild(0).gameObject.SetActive(false);
            if(transform.parent.GetChild(3).GetChild(1).GetComponent<Text>().text != "")
            {
                GetComponent<Button>().interactable = true;
                transform.GetChild(0).gameObject.SetActive(true);
                Debug.Log(transform.parent.GetChild(3).GetChild(1).GetComponent<Text>().text);
            }
        }
        else
        {
            transform.parent.GetChild(3).gameObject.SetActive(false);
            GetComponent<Button>().interactable = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void stopUserNameExists()
    {
        GameObject.Find("UserNameExists").GetComponent<Text>().enabled = false;
    }
}
