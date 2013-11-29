/*
 *  Filename    : TaskTimer.cs
 *  Description : Used by Circles to implement onscreen timer. Create GUIText object in scene and link to guiTextTimer
 *				  in inspector. ADD TO CAMERA.
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

public class TaskTimer : MonoBehaviour {

	public GUIText guiTextTimer;
	float time = 0f;
	public bool running = false;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (running) {
			time += Time.deltaTime;
		}

		guiTextTimer.text = "Time: " + Round(time, 2);

	}

	public void StartTimer () {
		running = true;
	}

	public void StopTimer (){
		running = false;
	}

	public void Reset () {
		running = false;
		time = 0f;
	}

	public static float Round (float value, int digits) {
    	float mult = Mathf.Pow (10.0f, (float) digits);
    	return Mathf.Round (value * mult) / mult;
    }

}
