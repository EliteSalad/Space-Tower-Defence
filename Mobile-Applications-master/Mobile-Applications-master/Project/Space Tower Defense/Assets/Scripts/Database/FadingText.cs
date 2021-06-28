using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadingText : MonoBehaviour {

    float changingvalue = 1.0f;
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Text>().color = new Color(this.GetComponent<Text>().color.r, this.GetComponent<Text>().color.g, this.GetComponent<Text>().color.b, this.GetComponent<Text>().color.a + (changingvalue * Time.deltaTime));

        if (this.GetComponent<Text>().color.a >= 0.9f || this.GetComponent<Text>().color.a <= 0.1f)
        {
            changingvalue *= -1.0f;
        }
    }
}
