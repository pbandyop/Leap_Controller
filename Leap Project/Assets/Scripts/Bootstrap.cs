/*
 *  Filename    : Bootstrap.cs
 *  Description : Bootstap scene, purely to load persistent DataObject to avoid duplication. Removes the need for
 *   			  singleton.
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

public class Bootstrap : MonoBehaviour {

	// Use this for initialization.
	void Start () {

	}

	// Update method called once per frame.
	void Update () {

		// Go straight to Title scene.
		Application.LoadLevel ("Title");

	}

}
