/*
 *  Filename    : AppData.cs
 *  Description : Class which stores all the data that is used across multiple scenes. MUST BE ATTACHED TO OBJECT LOADED
 *				  BY BOOTSTRAP SCENE.
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

// Global enum for pointing mode.
public enum Mode {Mouse, Relative, Absolute, Calibration};

public class AppData : MonoBehaviour {

	// APP DATA
	public bool bSpaceGrabMode;				//true for SPACE bar for grab command, false for thumb pinch action
	public bool bPointerGrab;				//Records if pointer grabbed
	public Mode pointingMode;				//Current pointing mode
	public Vector2 vCursorPos;				//Position of cursor in Unity window
//	public Vector2 vMousePos;				//Position of mouse on screen (irispective of Unity window size)
	public float fSensitivity;				//Sensitivity of pointing motion - range 0.50 to 1.50 Default 1
	public int leapWidth;					//width of leap zone (for relative mode)
	public int leapCentreY;					//Height of CENTRE of leap zone (for relative mode)
	public int targetSize;
	public float leapZoneScale;				//Scaling used to fine tune size of leap zone (for relative mode)
	public Vector2 vAbsTopReference;		//Reference point of top left of screen - from absolute calibration
	public Vector2 vAbsBottomReference;		//Reference point of bottom right of screen - from absolute calibration
	public GUIStyle menuStyle;				//Linked default menu style - used in GUI text
	public int targetRadius;				//Radius of targets - used to change size of targets
	public int circleRadius;				//Radius of circles - used to change size of circles

	//Make Object persistent across all levels
	void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

	// Use this for initialization
	void Start () {
		bSpaceGrabMode = false;				//Initialise grab mode mode: true for spave bar, false for thumb grab
		bPointerGrab = false;
		pointingMode = Mode.Mouse;			//Default to mouse mode
		fSensitivity = 1.0f;				//Default sensitivity
		leapWidth = 150;					//Default leap zone width
		leapCentreY = 200;					//Default height of CENTRE of leap zone
		leapZoneScale = 1.0f;				//Default scaling of leap zone
		targetRadius = 50;
		circleRadius = 30;
	}

	// Update is called once per frame
	void Update () {

	}
}
