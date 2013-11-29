/*
 *  Filename    : Pointer.cs
 *  Description : Draws pointer to coordinates. Self updates using cursor position from global AppData.
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
using System.Runtime.InteropServices;

public class Pointer : MonoBehaviour {

	public int radius = 5;
	public Vector2 vPosition;
	public Vector3 v3Position;
	public bool bPointerGrabbed;
	public Texture	pointerTexture;
	public Texture	pointerGrabbedTexture;

	//Scripts
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
		//Get current data
		vPosition = data.vCursorPos;
		bPointerGrabbed = data.bPointerGrab;
	}

	//Draw pointer at most recent coordinates
	void OnGUI() {
		//Change texture based on if pointer in 'grabbed' mode and draw to pointer position
		GUI.depth =0;

		if (bPointerGrabbed) {
			GUI.DrawTexture (new Rect (vPosition.x - radius, vPosition.y - radius, radius*2, radius*2),
				pointerGrabbedTexture);
		} else {
			GUI.DrawTexture (new Rect (vPosition.x - radius, vPosition.y - radius, radius*2, radius*2), pointerTexture);
		}
	}


	void DrawPointerCircle(float xCoord, float yCoord){

		//Change texture based on if pointer in 'grabbed' mode and draw to coords
		if (bPointerGrabbed) {
			GUI.DrawTexture (new Rect (xCoord - radius, yCoord - radius, radius*2, radius*2), pointerGrabbedTexture);
		} else {
			GUI.DrawTexture (new Rect (xCoord - radius, yCoord - radius, radius*2, radius*2), pointerTexture);
		}
	}
}

