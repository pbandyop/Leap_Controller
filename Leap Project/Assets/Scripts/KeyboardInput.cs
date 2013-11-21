/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

// Captures keyboard input - updates global AppData
// Uses AppData to communicated with other scripts
// Linked to persistent object

using UnityEngine;
using System.Collections;

public class KeyboardInput : MonoBehaviour {
	
	//AppData links
	GameObject dataObject;				//Persistent object holding AppData script
	AppData data;						//Script storing global app data required across scenes

	// Use this for initialization
	void Start () {
		
		//Link AppData
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();
	
	}
	
	// Update is called once per frame
	void Update () {
		//Capture keypresses
		
		//Toggle grab mode with 'm' key
		if (Input.GetKeyDown("m"))
		{
			data.bSpaceGrabMode = !data.bSpaceGrabMode;
		}
		
		//If grab mode is 'space bar' set bPointerGrab true when space bar down
		if (data.bSpaceGrabMode)
		{
			if (Input.GetKey("space"))
			{
				data.bPointerGrab = true;
			}
			else
			{
				data.bPointerGrab = false;
			}
		}
		
		//Return to Title screen on 'Escape'
		if (Input.GetKeyDown("escape"))
		{
			Application.LoadLevel("Title");
		}
		
		//TEST click mode
		if (Input.GetKeyDown("q"))
		{
			LeapAsMouse.MouseClickLeft();
		}
	
	}
	
	void OnGUI() {
		//Display grab mode on all scenes
		if (data.bSpaceGrabMode)
		{
			GUI.Label (new Rect (50, UnityEngine.Screen.height-30, 150, 20), "Space bar grab mode", data.menuStyle);
		}
		else
		{
			GUI.Label (new Rect (50, UnityEngine.Screen.height-30, 150, 20), "Thumb pinch grab mode", data.menuStyle);
		}
	}
}
