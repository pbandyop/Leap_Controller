/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

// GLOBAL POINTING AND GESTURE SCRIPT
// Updates AppData with coordinates to communicate with other modules 
// depending on pointing mode set in AppData

// Pointing methods update local position variables, then AppData updated from those

using UnityEngine;
using System.Collections;
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
	Vector2 vCursorPos = Vector2.zero;				//Final result of pointing methods - used to update AppData
	bool bGrabbed = false;							//Result of thumb pinch test - false if two 'fingers' detected, true if less
		//Screen dimensions
	int screenWidthUnity, screenHeightUnity, screenWidthWindows, screenHeightWindows;
		//Leap zone setup from AppData
	int leapWidth, leapCentreY;
	float leapScale;
		//Calculated leap zone dimensions
	int leapHeight, leapLeft, leapRight, leapTop, leapBottom;
	Vector3 vRelativeScale;							//Scaling vector for relative mode
		//Timing variables for smoothing
	long currentTime;
    long previousTime;
    long timeChange;

	
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
		screenWidthWindows = UnityEngine.Screen.currentResolution.width;		//Dimensions of whole screen (regardless of app window size)
		screenHeightWindows = UnityEngine.Screen.currentResolution.height;
		
		//Leap dimensions fom AppData
		leapWidth = data.leapWidth;
		leapCentreY = data.leapCentreY;
		leapScale = data.leapZoneScale;
		
		
		
		//Calculate Leap zone for relative mode
		//Calculate Leap zone based on screen dimensions + predefined width and vertical centre
		//Leap window width to screen width ratio
		float scale = (float)screenWidthUnity / leapWidth;					//cast to float to force float result
		vRelativeScale = new Vector2(scale, scale);
		//leapHeight as a ratio of screen height to width.
		leapHeight = Mathf.RoundToInt(screenHeightUnity / (screenWidthUnity / leapWidth));
		//Leap window calculations
		leapLeft = leapWidth/2 * -1;
		leapRight = leapWidth/2;
		leapTop = leapCentreY + leapHeight/2;
		leapBottom = leapCentreY - leapHeight/2;
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
		//MAIN LOOP
		
		//Update leap data
		UpdateLeap();
		
		//Check for gestures
		Gestures();
//print (data.pointingMode.ToString());
		//Select pointing method based on AppData pointing mode
		switch (data.pointingMode)
		{
		    case Mode.Mouse:
				MousePoint();
				break;
		    case Mode.Relative:
				RelativePoint();
				break;
			case Mode.Calibration:
				CalibrationPoint();
				break;
			case Mode.Absolute:
				AbsolutePoint2();
				break;
		    default:
				break;
		}
		
		//Finally update AppData with pointing and gesture results
		UpdateAppData();
	}
	
	///// UPDATE LEAP DATA /////
	void UpdateLeap()
	{
		frame = controller.Frame ();							//Get current frame data
		fingers = frame.Fingers;								//Update finger list from frame
	}
	
	///// CAPTURE AND HANDLE GESTURES /////
	void Gestures()
	{
		//detect and handle gestures
		gestures = frame.Gestures();
		//Mouse click with Screen Tap gesture
		if (gestures[0].Type == Gesture.GestureType.TYPESCREENTAP)
		{
			LeapAsMouse.MouseClickLeft();
		}
		
		//Test for grab action
		//If two fingers detected (finger and thumb spread)
		if (fingers.Count > 1){
			bGrabbed = false;
		}
		//otherwise true (thumb against hand so not detected)
		else{
			bGrabbed = true;
		}
	}
	
	///// MOUSE POINTING MODE /////
	void MousePoint()
	{
		//Get position of leading finger
		Vector2 vFingerPos = RelativeLeapCoordinates();
		
		//Scale leap data to screen dimensions
		float screenX = vFingerPos.x * (screenWidthWindows / leapWidth);
		float screenY = vFingerPos.y * (screenHeightWindows / leapHeight);
		
		//If finger detected by leap update mouse position
		if (fingers.Count > 0)
		{
			LeapAsMouse.SetCursorPos((int)screenX, (int)screenY);
		}
	}
	
	///// RELATIVE POINTING MODE /////
	void RelativePoint()
	{
		//Hide mouse if finger detected
		if (fingers.Count > 0)
		{
			UnityEngine.Screen.showCursor = false;
		}
		else
		{
			UnityEngine.Screen.showCursor = true;
		}
		
		//Get position of leading finger
		Vector2 vFingerPos = RelativeLeapCoordinates();

		//Scale leap data to Unity window dimensions
		vCursorPos = Vector2.Scale(vFingerPos, vRelativeScale);		
	}
	
	///// ABSOLUTE CALIBRATION POINTING MODE ///// <summary>
	/// Calibrations the point.
	/// </summary>
	void CalibrationPoint()
	{
		//Hide mouse if finger detected
		if (fingers.Count > 0)
		{
			UnityEngine.Screen.showCursor = false;
		}
		else
		{
			UnityEngine.Screen.showCursor = true;
		}
		
	}
	
	///// ABSOLUTE POINTING MODE /////
	//Main algorithm - Meshack Musundi - http://www.codeproject.com/Articles/550336/Leap-Motion-Move-Cursor
	void AbsolutePoint()
	{
		//Hide mouse if finger detected
		if (fingers.Count > 0)
		{
			UnityEngine.Screen.showCursor = false;
		}
		else
		{
			UnityEngine.Screen.showCursor = true;
		}
		
		//Update timing for smoothing
		currentTime = frame.Timestamp;
        timeChange = currentTime - previousTime;
		
		if (timeChange > 10000)
            {
                if (!frame.Hands.IsEmpty)
                {
                    // Get the first finger in the list of fingers
                    Finger finger = frame.Fingers[0];
                    // Get the closest screen intercepting a ray projecting from the finger
                    Leap.Screen screen = controller.LocatedScreens.ClosestScreenHit(finger);

                    if (screen != null && screen.IsValid)
                    {
                        // Get the velocity of the finger tip
                        var tipVelocity = (int)finger.TipVelocity.Magnitude;

                        // Use tipVelocity to reduce jitters when attempting to hold
                        // the cursor steady
                        if (tipVelocity > 25)
                        {
                            var xScreenIntersect = screen.Intersect(finger, true).x;
                            var yScreenIntersect = screen.Intersect(finger, true).y;
                            
                            if (xScreenIntersect.ToString() != "NaN")
                            {
                                var x = (int)(xScreenIntersect * screen.WidthPixels);
                                var y = (int)(screen.HeightPixels - (yScreenIntersect * screen.HeightPixels));

                                print("Screen intersect X: " + xScreenIntersect.ToString());
                                print("Screen intersect Y: " + yScreenIntersect.ToString());
                                print("Width pixels: " + screen.WidthPixels.ToString());
                                print("Height pixels: " + screen.HeightPixels.ToString());

                                print("\n");

                                print("x: " + x.ToString());
                                print("y: " + y.ToString());

                                print("\n");

                                print("Tip velocity: " + tipVelocity.ToString());
							
								//Update cursor coordinates
								vCursorPos = new Vector2(x, y);	

                                // Move the cursor
                                //MouseCursor.MoveCursor(x, y);

                            }

                        }
                    }

                }

                previousTime = currentTime;
            }
		
		
		
//		Leap.Vector vLeapIntersect;					//Position of intersection of finger [0]
//		Vector3 vScreenIntersect;					//Vector for conversion of Leap. Position of intersection vLeapIntersect to Unity.Vector3
//		Leap.Screen screen;							//Screen object used for Intersection of ray from finger
		
		
//print (vFingerPos.x + " +++ " + vFingerPos.y);
	}
	
	///// ABSOLUTE POINTING MODE /////
	void AbsolutePoint2()
	{
		//Hide mouse if finger detected
		if (fingers.Count > 0)
		{
			UnityEngine.Screen.showCursor = false;
		}
		else
		{
			UnityEngine.Screen.showCursor = true;
		}
		
		// Get the first finger in the list of fingers
        Finger finger = frame.Fingers[0];
		// Get the closest screen intercepting a ray projecting from the finger
        Leap.Screen screen = controller.LocatedScreens.ClosestScreenHit(finger);  			//NOTE - CallibratedScreens depreciated in favour of LocatedScreens
		// Intersect point of ray with projected Leap screen
		Leap.Vector vLeapIntersect = screen.Intersect(finger, true);
		//Convert Leap.Vector to Unity.Vector3 (floats)
		Vector2 vScreenIntersect = new Vector2(vLeapIntersect.ToFloatArray()[0],vLeapIntersect.ToFloatArray()[1]);
		
		print ("vScreenIntersect X: " + vScreenIntersect.x + " Y: " + vScreenIntersect.y);
		
		float xTop = 400;
		float YTop = 300;
		
		float xBottom = 1550;
		float YBottom = 760;
		
		Vector2 vLeapCoords = new Vector2(vScreenIntersect.x * screenWidthUnity, (vScreenIntersect.y * screenHeightUnity) * -1 + screenHeightUnity);

		print ("vLeapCoords X: " + vLeapCoords.x + " Y: " + vLeapCoords.y);
		
		
		Vector2 vScreenConversion = new Vector2(vLeapCoords.x - xTop, vLeapCoords.y - YTop);
		
		float xRatio = screenWidthWindows / (xBottom - xTop);
		float yRatio = screenHeightWindows / (YBottom - YTop);
		
		print ("xRation: " + xRatio);
		print ("yRation: " + yRatio);
		
		print ("vScreenConversion X: " + vScreenConversion.x + " Y: " + vScreenConversion.y);
		
		
		//Update cursor coordinates
		vCursorPos.x = vScreenConversion.x * xRatio;
		vCursorPos.y = vScreenConversion.y * yRatio;
		
		
		
		
		print ("vCursorPos X: " + vCursorPos.x + " Y: " + vCursorPos.y);
	}
	
	
	//Update AppData after pointing method has calculated coords
	void UpdateAppData()
	{
		//Update cursor position
		data.vCursorPos = vCursorPos;
		//IF grab mode is using thumb pinch update, otherwise leave to KeyboardInput to update with space bar
		if(!data.bSpaceGrabMode)
		{
			data.bPointerGrab = bGrabbed;	
		}
	}
	
	//Returns coordinates of finger in leap zone transposed for Unity coordinates
	Vector2 RelativeLeapCoordinates()
	{
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
