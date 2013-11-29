/*
 *  Filename    : KeyboardInput.cs
 *  Description : Captures keyboard input & updates global AppData. Uses AppData to communicate with other scripts.
 *                Linked to persistent object.
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

public class KeyboardInput : MonoBehaviour {

	// AppData links
	GameObject dataObject;				// Persistent object holding AppData script.
	AppData data;						// Script storing global app data required across scenes.

	// Use this for initialization
	void Start () {

		//Link AppData
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();

	}

	// Update is called once per frame
	void Update () {
		//Capture keypresses

		// Toggle grab mode with 'm' key.
		if (Input.GetKeyDown("m")) {
			data.bSpaceGrabMode = !data.bSpaceGrabMode;
		}

		// If grab mode is 'space bar' set bPointerGrab true when space bar down.
		if (data.bSpaceGrabMode) {
			if (Input.GetKey("space") || Input.GetKey("mouse 0")) {
				data.bPointerGrab = true;
			} else {
				data.bPointerGrab = false;
			}
		}

		// Return to Title screen on 'Escape'.
		if (Input.GetKeyDown("escape")) {
			Application.LoadLevel("Title");
		}

		// TEST click mode.
		if (Input.GetKeyDown("q")) {
			LeapAsMouse.MouseClickLeft();
		}
	}

	void OnGUI() {

		//Display grab mode on all scenes
		if (data.bSpaceGrabMode) {
			GUI.Label (new Rect (50, UnityEngine.Screen.height-30, 150, 20), "Space bar grab mode", data.menuStyle);
		} else {
			GUI.Label (new Rect (50, UnityEngine.Screen.height-30, 150, 20), "Thumb pinch grab mode", data.menuStyle);
		}
	}
}
