/*
 *  Filename    : LeapPointing.cs
 *  Description : GLOBAL POINTING AND GESTURE SCRIPT. Updates AppData with coordinates to communicate with other
 *				  modules depending on pointing mode set in AppData. Pointing methods update local position variables,
 *				  then AppData updated from those.
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
using System.Collections.Generic;
using Leap;
using System.Runtime.InteropServices;

public class LeapPointing : MonoBehaviour {

	//Leap
	Controller controller;							//Main Leap controller
	Frame frame;									//Frame of Leap data - updated every frame
	FingerList fingers;								//List of fingers from frame
	Leap.Vector vFinger;							//Leap data for coords of foremost finger
	GestureList gestures;							//Gestures from frame

	//Internal
	//Final result of pointing methods - used to update AppData
	Vector2 vCursorPos = Vector2.zero;

	//Stores the result from smoothing functions in order to smooth absolute pointing in the next frame
	Vector2[] vArray = new Vector2[9];

	bool bGrabbed = false;					//Result of thumb pinch test - false if two 'fingers' detected, true if less
	bool bMouseClick = false;				//Record if mouse clicked for title screen

	//Screen dimensions
	int screenWidthUnity, screenHeightUnity, screenWidthWindows, screenHeightWindows;

	//Leap zone setup from AppData
	int leapWidth, leapCentreY;

	//Calculated leap zone dimensions
	int leapHeight;

	//Scaling vector for relative mode
	Vector3 vRelativeScale;

	//Timing variables for smoothing
	long currentTime;
    long previousTime;
    long timeChange;

	//Thumb pinch Smoothing
	Queue<bool> qThumbPinched;			//Records last 3 values of thumb grabbing results - use to smooth grab action.

	//AppData links
	GameObject dataObject;							//Persistent object holding AppData script
	AppData data;									//Script storing global app data required across scenes

	// Use this for initialization
	void Start () {

		//Link AppData
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();

		//Initialise Leap controller and enable require gestures
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);

		//screen dimensions
		screenWidthUnity = UnityEngine.Screen.width;							//Dimensions of Unity window
		screenHeightUnity = UnityEngine.Screen.height;

		//Dimensions of whole screen (regardless of app window size)
		screenWidthWindows = UnityEngine.Screen.currentResolution.width;

		screenHeightWindows = UnityEngine.Screen.currentResolution.height;

		//Leap dimensions fom AppData
		leapWidth = data.leapWidth;
		leapCentreY = data.leapCentreY;

		//Calculate Leap zone for relative mode based on screen dimensions + predefined width and vertical centre
		//Leap window width to screen width ratio
		float scale = (float)screenWidthUnity / leapWidth;					//cast to float to force float result
		vRelativeScale = new Vector2(scale, scale);

		//leapHeight as a ratio of screen height to width.
		leapHeight = Mathf.RoundToInt(screenHeightUnity / (screenWidthUnity / leapWidth));

		//Thumb pinch Smoothing
		qThumbPinched = new Queue<bool>();

	}

	// Update is called once per frame
	void Update () {

		//MAIN LOOP

		//Update leap data
		UpdateLeap();

		//Check for gestures
		Gestures();

		//Select pointing method based on AppData pointing mode
		switch (data.pointingMode) {
		    case Mode.Mouse:
				MousePoint();
				break;
		    case Mode.Relative:
				RelativePoint();
				break;
			case Mode.Calibration:
				//Calibration pointing handled by seperate AbsoluteCalibration script
				//Keep pointer off screen
				vCursorPos = new Vector2(-100,-100);
				break;
			case Mode.Absolute:
				AbsolutePoint();
				break;
		    default:
				break;
		}

		//Finally update AppData with pointing and gesture results
		UpdateAppData();
	}

	///// UPDATE GUI /////
	void OnGUI()
	{
		switch (data.pointingMode) {
		    case Mode.Mouse:
				break;
		    case Mode.Relative:
				GUI.Label (new Rect (screenWidthUnity/2 -100, screenHeightUnity - 40, 200, 20),
					"RELATIVE POINTING MODE", data.menuStyle);
				break;
			case Mode.Calibration:
				GUI.Label (new Rect (screenWidthUnity/2 -100, screenHeightUnity - 40, 200, 20),
					"CALIBRATION MODE", data.menuStyle);
				break;
			case Mode.Absolute:
				GUI.Label (new Rect (screenWidthUnity/2 -100, screenHeightUnity - 40, 200, 20),
					"ABSOLUTE POINTING MODE", data.menuStyle);
				break;
		    default:
				break;
		}
	}

	///// UPDATE LEAP DATA /////
	void UpdateLeap () {
		frame = controller.Frame ();							//Get current frame data
		fingers = frame.Fingers;								//Update finger list from frame
	}

	///// CAPTURE AND HANDLE GESTURES /////
	void Gestures() {
		//detect and handle gestures
		gestures = frame.Gestures();

		//Mouse click with Screen Tap gesture
		if (gestures[0].Type == Gesture.GestureType.TYPESCREENTAP) {
			LeapAsMouse.MouseClickLeft();
		}

		//Reset mouse click
		bMouseClick = false;

		//Test for grab action

		//If any fingers detected
		if (fingers.Count > 0) {
			//If two fingers detected (finger and thumb spread)
			if (fingers.Count > 1) {
				//Add result to queue
				qThumbPinched.Enqueue(false);

				//if last 3 results are same (not true) then change pointer grabbed state to false
				if (!qThumbPinched.Contains(true)) {
					bGrabbed = false;
				}
			} else { 					//otherwise true (thumb against hand so not detected)
				//Add result to smoothing queue
				qThumbPinched.Enqueue(true);

				//if last 3 results are same (not false) then change pointer grabbed state to true
				if (!qThumbPinched.Contains(false)) {
					//If changing from false to true, mouse click (for menu buttons)
					if (bGrabbed == false) {
						bMouseClick = true;
					}

					bGrabbed = true;
				}
			}
		}

		//Limit qThumbPinched smoothing queue to 3 elements
		if (qThumbPinched.Count > 3) {
			qThumbPinched.Dequeue();
		}
	}

	///// MOUSE POINTING MODE /////
	void MousePoint () {
		//Place purple cursor just off screen
		vCursorPos = new Vector2(-50,-50);

		//Get position of leading finger - call Relative Pointing Algorithm
		Vector2 vFingerPos = RelativeLeapCoordinates();

		//Scale leap data to screen dimensions
		float screenX = vFingerPos.x * (screenWidthWindows / leapWidth);
		float screenY = vFingerPos.y * (screenHeightWindows / leapHeight);

		//If finger detected by leap update mouse position
		if (fingers.Count > 0) {
			LeapAsMouse.SetCursorPos((int)screenX, (int)screenY);
			//If mouse click detected while fingers detected AND mouse within unity screen
			if (bMouseClick) {
				LeapAsMouse.MouseClickLeft();
			}
		}
	}

	///// RELATIVE POINTING MODE /////
	void RelativePoint() {

		//Get position of leading finger - call Relative Pointing Algorithm
		Vector2 vFingerPos = RelativeLeapCoordinates();

		//Scale leap data to Unity window dimensions
		vCursorPos = Vector2.Scale(vFingerPos, vRelativeScale);
	}

	///// ABSOLUTE POINTING MODE /////
	void AbsolutePoint () {

		// Get the first finger in the list of fingers
        Finger finger = frame.Fingers[0];

		// Get the closest screen intercepting a ray projecting from the finger.
		//NOTE - CallibratedScreens depreciated in favour of LocatedScreens.
        Leap.Screen screen = controller.LocatedScreens.ClosestScreenHit(finger);

		// Intersect point of ray with projected Leap screen
		Leap.Vector vLeapIntersect = screen.Intersect(finger, true);

		//Convert Leap.Vector to Unity.Vector3 (floats)
		Vector2 vScreenIntersect = new Vector2(vLeapIntersect.ToFloatArray()[0],vLeapIntersect.ToFloatArray()[1]);

		//Get calibration points from AppData
		float xTop = data.vAbsTopReference.x;
		float YTop = data.vAbsTopReference.y;
		float xBottom = data.vAbsBottomReference.x;
		float YBottom = data.vAbsBottomReference.y;
		
		//Apply screen dimension scaling to raw data to create virtual screen coordinates
		Vector2 vLeapCoords = new Vector2(vScreenIntersect.x * screenWidthUnity,
			(vScreenIntersect.y * screenHeightUnity) * -1 + screenHeightUnity);
		
		//Scale virtual screen coordinates to real screen coordinates using calibration data
		Vector2 vScreenConversion = new Vector2(vLeapCoords.x - xTop, vLeapCoords.y - YTop);
		float xRatio = screenWidthUnity / (xBottom - xTop);
		float yRatio = screenHeightUnity / (YBottom - YTop);

		//Apply smoothing to final pointer coordinates
		vCursorPos = SmoothingMethod(new Vector2(vScreenConversion.x * xRatio,vScreenConversion.y * yRatio));
	}
	
	///// ABSOLUTE SMOOTHING METHOD /////
	Vector2 SmoothingMethod(Vector2 inputVector) {
		vArray[8]=inputVector;
		Vector2 returnPos;
		returnPos.x = ((-21f*vArray[0].x) + (14f*vArray[1].x) + (39f*vArray[2].x) + (54f*vArray[3].x) + (59f*vArray[4].x) 
			+ (54f*vArray[5].x) + (39f*vArray[6].x) + (14f*vArray[7].x) + (-21f*vArray[8].x))/231f;
		returnPos.y = ((-21f*vArray[0].y) + (14f*vArray[1].y) + (39f*vArray[2].y) + (54f*vArray[3].y) + (59f*vArray[4].y) 
			+ (54f*vArray[5].y) + (39f*vArray[6].y) + (14f*vArray[7].y) + (-21f*vArray[8].y))/231f;
		vArray[0]=vArray[1];
		vArray[1]=vArray[2];
		vArray[2]=vArray[3];
		vArray[3]=vArray[4];
		vArray[4]=vArray[5];
		vArray[5]=vArray[6];
		vArray[6]=vArray[7];
		vArray[7]=vArray[8];
		return returnPos;
	}


	//Update AppData after pointing method has calculated coords
	void UpdateAppData() {
		//Update cursor position
		data.vCursorPos = vCursorPos;

		//IF grab mode is using thumb pinch update, otherwise leave to KeyboardInput to update with space bar
		if(!data.bSpaceGrabMode) {
			data.bPointerGrab = bGrabbed;
		}
	}
	
	///// RELATIVE POINTING ALGORITHM /////
	//Returns coordinates of finger in leap zone transposed for Unity coordinates
	Vector2 RelativeLeapCoordinates() {
		//Get position of leading finger
		Leap.Vector vFingerStab = fingers.Frontmost.StabilizedTipPosition;

		//Transpose x axis for width offset and y axis for Leap window height
		float transX = vFingerStab.x + leapWidth/2;

		//Remove Leap window height and invert y axis
		float transY = (vFingerStab.y - leapCentreY + leapHeight/2) * -1 + leapHeight;

		Vector2 vTransposedFingerUnity = new Vector3(transX, transY);

		return vTransposedFingerUnity;
	}

}
