using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadingScript : MonoBehaviour {

    float changingvalue = 0.5f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
        this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, this.GetComponent<Image>().color.a + (changingvalue*Time.deltaTime));

        if (this.GetComponent<Image>().color.a >= 1.0f || this.GetComponent<Image>().color.a <= 0.0f)
        {
            changingvalue *= -1.0f;
        }
	}
}
