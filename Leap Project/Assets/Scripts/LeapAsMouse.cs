/*
 *  Filename    : LeapAsMouse.cs
 *  Description : Provides control of system mouse. Global class - does not need attaching to object.
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
using System.Runtime.InteropServices;			//Access to user32.dll

public struct Point {
    public int x, y;
}

public class LeapAsMouse : MonoBehaviour {
    [DllImport("user32.dll")]

	//Control mouse pointer position
    public static extern bool SetCursorPos (int x, int y);

	//control mouse clicks
	[DllImport("user32.dll")]

	public static extern void mouse_event (long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

	[DllImport("user32.dll")]
	public static extern void GetCursorPos(ref Point lpPoint);

	//Trigger mouse click
	public static void MouseClickLeft () {
        //Call the imported function with the cursor's current position - uses unity window coordinates
        long X = (long)Input.mousePosition.x;
        long Y = (long)Input.mousePosition.y;
        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
    }

	//Get screen coordinates of mouse
	public static void GetCursorPos () {
        Point p = new Point();
		GetCursorPos(ref p);
		Vector3 v = new Vector3();
		v = Input.mousePosition;
		print("GetCursorPos X: " + p.x + " Y: " + p.y + "mousePosition X: " + v.x + " Y: " + v.y);
    }

}

