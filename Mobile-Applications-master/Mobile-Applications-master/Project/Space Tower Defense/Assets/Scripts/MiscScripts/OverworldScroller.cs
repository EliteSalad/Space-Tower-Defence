using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldScroller : MonoBehaviour
{
    public float startMouseXLocation;
    public float currentMouseXLocation;
    public Vector3 thisCurrentGOPosition;

    // Distance from current mouse location to start mouse location
    public float distance;

	// Use this for initialization
	void Start ()
    {
        thisCurrentGOPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Left mouse click down
		if(Input.GetMouseButtonDown(0))
        {
            // Sets start location of the mouse
            startMouseXLocation = Input.mousePosition.x;
        }

        // Only happens when mouse click is down
        if (Input.GetMouseButton(0))
        {
            currentMouseXLocation = Input.mousePosition.x;
            distance = currentMouseXLocation - startMouseXLocation;
            transform.localPosition = new Vector3(Mathf.Clamp(thisCurrentGOPosition.x + distance, -845.0f, 28.0f), thisCurrentGOPosition.y, thisCurrentGOPosition.z);
        }

        // When the click is let go sets the new position so it doesn't sbnap back
        if(Input.GetMouseButtonUp(0))
        {
            thisCurrentGOPosition = transform.localPosition;
        }
	}
}
