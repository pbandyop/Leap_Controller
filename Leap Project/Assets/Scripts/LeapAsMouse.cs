/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

//Provides control of system mouse
//Global class - does not need attaching to object

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;			//Access to user32.dll

    public struct Point
    {
    	public int x, y;
    }

public class LeapAsMouse : MonoBehaviour 
{
    [DllImport("user32.dll")]
	//Control mouse pointer position
    public static extern bool SetCursorPos(int x, int y);
	//call using LeapAsMouse.SetCursorPos(x,y); 
	
	//control mouse clicks
	[DllImport("user32.dll")]
	public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
	
	[DllImport("user32.dll")]
	public static extern void GetCursorPos(ref Point lpPoint);
	
	
	//Trigger mouse click
	public static void MouseClickLeft()
    {
        //Call the imported function with the cursor's current position - uses unity window coordinates
        long X = (long)Input.mousePosition.x;
        long Y = (long)Input.mousePosition.y;
        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
    }
	
	//Get screen coordinates of mouse
	public static void GetCursorPos()
    {
        Point p = new Point();
		GetCursorPos(ref p);
		Vector3 v = new Vector3();
		v = Input.mousePosition;
		print("GetCursorPos X: " + p.x + " Y: " + p.y
			+ "mousePosition X: " + v.x + " Y: " + v.y);
    }
	
} 

