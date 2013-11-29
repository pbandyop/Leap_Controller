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
	
	//Graphics
	public Texture	texRedCircle;
	Vector2 vCirclePos;
	int radiusCircle;

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
		screenWidthUnity = UnityEngine.Screen.width;							
		screenHeightUnity = UnityEngine.Screen.height;

		//Initialise Leap controller and enable require gestures
		controller = new Controller();

		stage = 1;
		stageCount = 1;
		
		//Target circle size
		radiusCircle = 100;

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
		Rect labelBox = new Rect(UnityEngine.Screen.width/2-100, UnityEngine.Screen.height-100,200,20);

		//Update interface based on stage
		if (stage == 1) {
			//position circle
			vCirclePos = new Vector2(0,0);
			//Draw circle
			Draw(vCirclePos, radiusCircle, texRedCircle);
			
			//Instructions
			GUI.Label (new Rect (UnityEngine.Screen.width/2 -100, UnityEngine.Screen.height/2, 200, 20),
					"POINT FINGER AT RED CORNER", data.menuStyle);
			GUI.Label (new Rect (UnityEngine.Screen.width/2 -100, UnityEngine.Screen.height/2 + 40, 200, 20),
					"and CLICK LEFT MOUSE or SPACE BAR", data.menuStyle);
			GUI.Label (labelBox, "STAGE " + stageCount + "/7", data.menuStyle);
			
		} else if (stage == 2){
			
			//position circle
			vCirclePos = new Vector2(UnityEngine.Screen.width,UnityEngine.Screen.height);
			//Draw circle
			Draw(vCirclePos, radiusCircle, texRedCircle);
			
			//Instructions
			GUI.Label (new Rect (UnityEngine.Screen.width/2 -100, UnityEngine.Screen.height/2, 200, 20),
					"POINT FINGER AT RED CORNER", data.menuStyle);
			GUI.Label (new Rect (UnityEngine.Screen.width/2 -100, UnityEngine.Screen.height/2 + 40, 200, 20),
					"and CLICK LEFT MOUSE or SPACE BAR", data.menuStyle);
			GUI.Label (labelBox, "STAGE " + stageCount + "/7", data.menuStyle);
			
		} else {
			
			//Instructions
			GUI.Label (new Rect (UnityEngine.Screen.width/2 -100, UnityEngine.Screen.height/2, 200, 20),
					"IS THIS CALIBRATION ACCEPTABLE?", data.menuStyle);
			GUI.Label (new Rect (UnityEngine.Screen.width/2 -100, UnityEngine.Screen.height/2 + 40, 200, 20),
					"CLICK LEFT MOUSE or SPACE BAR TO ACCEPT", data.menuStyle);
			GUI.Label (new Rect (UnityEngine.Screen.width/2 -100, UnityEngine.Screen.height/2 + 80, 200, 20),
					"or PRESS R TO RECALIBRATE", data.menuStyle);
			GUI.Label (labelBox, "STAGE " + stageCount + "/7", data.menuStyle);
		}

    }

	void Stage1() {
		//Record first top left cords
		if (stageCount == 1) {
			if (Input.GetKeyDown("space") || Input.GetKeyDown("mouse 0")) {
				vAveTopLeft = new Vector2(vLeapCoords.x, vLeapCoords.y);
				stageCount++;
				stage = 2;
			}
		} else {
			//If space bar down record top left coord
			if (Input.GetKeyDown("space") || Input.GetKeyDown("mouse 0")) {
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
			if (Input.GetKeyDown("space") || Input.GetKeyDown("mouse 0")) {
				vAveBottomRight = new Vector2(vLeapCoords.x, vLeapCoords.y);
				stageCount++;
				stage = 1;
			}
		} else {
			//If space bar down record top left coord
			if (Input.GetKeyDown("space") || Input.GetKeyDown("mouse 0")) {
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

		//Select mode
		data.pointingMode = Mode.Absolute;
		
		//Accept calibration
		if (Input.GetKeyDown("space") || Input.GetKeyDown("mouse 0")) {
			Application.LoadLevel("AbsolutePointing");
		}
		
		//Reset calibration to re-run full calibration mode
		if (Input.GetKeyDown("r")) {
			Reset ();
		}

	}

	///// ABSOLUTE POINTING DATA/////
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
	
	//Draw texture @ position
	void Draw (Vector2 pos, int radius, Texture texture) {
		GUI.DrawTexture(new Rect(pos.x - radius, pos.y - radius, radius*2, radius*2), texture);
	}
	
	//Reset calibration to stage 1
	void Reset ()
	{
		stage = 1;
		stageCount = 1;
		// reset mode
		data.pointingMode = Mode.Calibration;
		//place pointer off screen
		data.vCursorPos = new Vector2(0,0);
	}

}
