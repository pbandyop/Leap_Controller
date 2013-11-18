/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

// Creates title screen

using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	
	//GUI elements
	string toggleText = "Space bar";	//Initialise for space bar pointing mode
	int buttonWidth = 150;
	int buttonHeight = 50;
	int spacing = 30;
	
	//AppData links
	GameObject dataObject;				//Persistent object holding AppData script
	AppData data;						//Script storing global app data required across scenes

	// Use this for initialization
	void Start () {
		//Link AppData
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();
		//Setup AppData
		data.bSpaceGrabMode = true;		//Initialise for space bar pointing mode
		data.pointingMode = Mode.Mouse;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI() {
		//Main menu buttons
		GUILayout.BeginArea(new Rect(Screen.width/2 - buttonWidth/2, Screen.height/2 - 200, buttonWidth, 400));
			if(GUILayout.Button("Relative pointing", GUILayout.Height(buttonHeight)))
			{
				data.pointingMode = Mode.Relative;
				Application.LoadLevel("RelativePointing");
			}
			GUILayout.Space(spacing);
			if(GUILayout.Button("Absolute pointing", GUILayout.Height(buttonHeight)))
			{
				data.pointingMode = Mode.Calibration;
				Application.LoadLevel("Calibration");
			}
			GUILayout.Space(spacing);
			if(GUILayout.Button("Spare scene", GUILayout.Height(buttonHeight)))
			{
				data.pointingMode = Mode.Absolute;
				Application.LoadLevel("AbsolutePointing");
			}
			GUILayout.Space(spacing);
			if(GUILayout.Button("Quit", GUILayout.Height(buttonHeight)))
			{
				Application.Quit();
			}
			
		GUILayout.EndArea();
		
		//Toggle grab mode from Spacebar to Thumb pinch
		GUI.Label (new Rect (20, 10, 200, 20), "Toggle grab mode");
		if (GUI.Button (new Rect(10, 40, 120, buttonHeight), toggleText))
		{
			if (data.bSpaceGrabMode)
			{
				data.bSpaceGrabMode = false;
				toggleText = "Thumb pinch";
			}
			else
			{
				data.bSpaceGrabMode = true;
				toggleText = "Space bar";
			}	
		}
		


	}
	
}
	
