/*
 *  Filename    : TitleGUI.cs
 *  Description : Creates the title screeen.
 *
 *  Copyright   : © 2013 Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain & Maninder Singh.
 * 				  University of Helsinki, Finland.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {

	// GUI elements
	string toggleText;					// Initialise for space bar pointing mode.

	int buttonWidth = 150;
	int buttonHeight = 50;
	int spacing = 30;

	// AppData links
	GameObject dataObject;				// Persistent object holding AppData script.
    AppData data; 						// Script storing global app data required across scenes.

	// Use this for initialization
	void Start () {

		// Link AppData.
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();

		// Setup AppData.
		data.pointingMode = Mode.Mouse;

		// Show mouse cursor.
		UnityEngine.Screen.showCursor = true;

		// Setup button text.
		if (data.bSpaceGrabMode) {
			toggleText = "Space bar";
		} else {
			toggleText = "Thumb pinch";
		}
	}

	// Update method called once per frame.
	void Update () {

	}

	void OnGUI () {

		// Main menu buttons.
		GUILayout.BeginArea (new Rect (Screen.width/2 - buttonWidth/2, Screen.height/2 - 150, buttonWidth, 300));

		if (GUILayout.Button("Relative pointing", GUILayout.Height(buttonHeight))) {
			// Select mode
			data.pointingMode = Mode.Relative;

			// Hide mouse cursor
			UnityEngine.Screen.showCursor = false;

			// Load scene
			Application.LoadLevel ("RelativePointing");
		}

		GUILayout.Space (spacing);

		if (GUILayout.Button ("Absolute pointing", GUILayout.Height (buttonHeight))) {

			// Select mode
			data.pointingMode = Mode.Calibration;

			// Hide mouse cursor
			UnityEngine.Screen.showCursor = false;

			// Load scene
			Application.LoadLevel ("Calibration");
        }

		GUILayout.Space (spacing);

		if (GUILayout.Button ("Quit", GUILayout.Height (buttonHeight))) {

			//Quit app
			Application.Quit();
		}

		GUILayout.EndArea();

		// Toggle grab mode from spacebar to thumb pinch.
		GUI.Label (new Rect (Screen.width/2 + 150, Screen.height/2 - 90, 200, 20), "Toggle grab mode", data.menuStyle);

		if (GUI.Button (new Rect (Screen.width/2 + 190, Screen.height/2 -50, 120, buttonHeight), toggleText)) {
			if (data.bSpaceGrabMode) {
				data.bSpaceGrabMode = false;
				toggleText = "Thumb pinch";
			} else {
				data.bSpaceGrabMode = true;
				toggleText = "Space bar";
			}
		}
	}
}

