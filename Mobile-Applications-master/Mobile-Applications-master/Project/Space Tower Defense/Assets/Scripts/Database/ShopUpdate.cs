using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpdate : MonoBehaviour {

    private Database_Control DB;
    private Text text;
    private Text costText;
    public GameObject buttonPrefab;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        costText = transform.parent.GetChild(1).GetComponent<Text>();

        DB = GameObject.Find("Database Controller").GetComponent<Database_Control>();
        float buttonYPos = -10.0f;
        string IDNumber = "";
        foreach (KeyValuePair<string, float> StoreItem in DB.store.store())
        {
            IDNumber = StoreItem.Key.Substring(0, 1);
            text.text = text.text + StoreItem.Key.Substring(1) + "\n";

            switch (IDNumber)
            {
                case "P":
                    costText.text =  costText.text +  "£"+(StoreItem.Value/100.0f).ToString() + "\n";
                    break;
                case "G":
                    costText.text = costText.text + StoreItem.Value.ToString() + " Gems" + "\n";
                    break;
                case "M":
                    costText.text = costText.text + StoreItem.Value.ToString() + " Medals" + "\n";
                    break;
            }
            
            GameObject Button = Instantiate(buttonPrefab);
            Button.transform.SetParent(GameObject.Find("ButtonPanel").transform);          

            Button.GetComponent<ShopButtonController>().SetValues(IDNumber, StoreItem.Key.Substring(1), (int)StoreItem.Value);            
            Button.transform.localScale = Vector3.one;

            Button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,buttonYPos,0);
            
            buttonYPos -= 55.0f;
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
