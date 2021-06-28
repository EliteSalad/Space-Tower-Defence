using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchIcon : MonoBehaviour {

    public Sprite Using;
    public Sprite Usable;
    public Sprite NotUsable;
    private List<Sprite> Icons;
	// Use this for initialization
	void Awake () {
        Icons = new List<Sprite>();
        Icons.Add(Using);
        Icons.Add(Usable);
        Icons.Add(NotUsable);
    }

    public void SetImage(int imageNumber)
    {
        GetComponent<Image>().sprite = Icons[imageNumber];
    }
}
