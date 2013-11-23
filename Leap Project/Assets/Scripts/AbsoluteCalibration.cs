/*
 *  Filename    : AbsoluteCalibration.cs
 *  Description : Implements Absolute Pointing algoritm based on projected angle of finger translated to screen
 *				  position. Captures data from leap. Detects and handles gestures. Explicitely updates Pointer and
 *				  Circles scripts. REQUIRES Pointer and Circles scripts. ADD TO CAMERA.
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
using Leap;

public class AbsoluteCalibration : MonoBehaviour {

	//Leap
	Controller controller;							//Main Leap controller
	Frame frame;									//Frame of Leap data - updated every frame
	Leap.Vector vFinger;							//Leap data for coords of foremost finger
	GestureList gestures;							//Gestures from frame

	//Screen dimensions
	int screenWidthUnity, screenHeightUnity;

	//Internal
	int stage;
	int stageCount;

	Vector2 vLeapCoords;
	Vector2 vAveTopLeft;
	Vector2 vAveBottomRight;

	//AppData links
	GameObject dataObject;							//Persistent object holding AppData script
	AppData data;									//Script storing global app data required across scenes


	// Use this for initialization
	void Start () {
		//Link AppData
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();

				//screen dimensions
		screenWidthUnity = UnityEngine.Screen.width;							//Dimensions of Unity window
		screenHeightUnity = UnityEngine.Screen.height;

		//Initialise Leap controller and enable require gestures
		controller = new Controller();

		stage = 1;
		stageCount = 1;

	}

	// Update is called once per frame
	void Update () {
		//Update leap data
		UpdateLeap();


		//Update interface based on stage
		if (stage == 1) {
			Stage1();
		} else if (stage == 2) {
			Stage2();
		} else if (stage == 3) {
			Stage3();
		}

	}

	//Create static labels on screen
	void OnGUI() {
		Rect labelBox = new Rect(UnityEngine.Screen.width/2-100, UnityEngine.Screen.height-50,200,20);

		//Update interface based on stage
		if (stage == 1) {
			GUI.Label(labelBox,"STAGE " + stageCount + "/7");
		} else if (stage == 2){
			GUI.Label(labelBox,"STAGE " + stageCount + "/7");
		} else {
			GUI.Label(labelBox,"STAGE " + stageCount + "/7");
		}

    }

	void Stage1() {
		//Record first top left cords
		if (stageCount == 1) {
			if (Input.GetKeyDown("space")) {
				vAveTopLeft = new Vector2(vLeapCoords.x, vLeapCoords.y);
				print ("vAveTopLeftX: " + vAveTopLeft.x + " vAveTopLeftY: " + vAveTopLeft.y);
				stageCount++;
				stage = 2;
			}
		} else {
			//If space bar down record top left coord
			if (Input.GetKeyDown("space")) {
				//Average new reading with current average
				vAveTopLeft = (vAveTopLeft + new Vector2(vLeapCoords.x, vLeapCoords.y)) / 2;
				stageCount++;
				if (stageCount != 7) {
					stage = 2;
				} else {
					stage = 3;
				}
			}
		}
	}

	void Stage2() {
		//Record first bottom right cords
		if (stageCount == 2) {
			if (Input.GetKeyDown("space")) {
				vAveBottomRight = new Vector2(vLeapCoords.x, vLeapCoords.y);
				print ("vAveBottomRightX: " + vAveBottomRight.x + " vAveBottomRightY: " + vAveBottomRight.y);
				stageCount++;
				stage = 1;
			}
		} else {
			//If space bar down record top left coord
			if (Input.GetKeyDown("space")) {
				//Average new reading with current average
				vAveBottomRight = (vAveBottomRight + new Vector2(vLeapCoords.x, vLeapCoords.y)) / 2;
				stageCount++;
				if (stageCount != 7) {
					stage = 1;
				} else {
					stage = 3;
				}
			}
		}
	}

	void Stage3() {
		//Update AppData with calibration coordinates
		data.vAbsTopReference = vAveTopLeft;
		data.vAbsBottomReference = vAveBottomRight;

		print ("vAbsTopReferenceX: " + data.vAbsTopReference.x + " vAbsTopReferenceY: " + data.vAbsTopReference.y);
		print ("vAbsBottomReferenceX: " + data.vAbsBottomReference.x + " vAbsBottomReferenceY: "
			+ data.vAbsBottomReference.y);

		//Select mode
		data.pointingMode = Mode.Absolute;

		//Load scene
		Application.LoadLevel("AbsolutePointing");

	}

	///// ABSOLUTE POINTING MODE /////
	void UpdateLeap() {
		frame = controller.Frame ();							//Get current frame data

		// Get the first finger in the list of fingers
        Finger finger = frame.Fingers[0];

		// Get the closest screen intercepting a ray projecting from the finger
		//NOTE - CallibratedScreens depreciated in favour of LocatedScreens
        Leap.Screen screen = controller.LocatedScreens.ClosestScreenHit(finger);

		// Intersect point of ray with projected Leap screen
		Leap.Vector vLeapIntersect = screen.Intersect(finger, true);

		//Convert Leap.Vector to Unity.Vector3 (floats)
		Vector2 vScreenIntersect = new Vector2(vLeapIntersect.ToFloatArray()[0],vLeapIntersect.ToFloatArray()[1]);

		vLeapCoords = new Vector2(vScreenIntersect.x * screenWidthUnity, (vScreenIntersect.y * screenHeightUnity) *
		 -1 + screenHeightUnity);

	}

}
