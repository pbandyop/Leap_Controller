/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

//ADD TO CAMERA
//REQUIRES Pointer and Circles scripts
//Captures data from leap
//Detects and handles gestures
//Implements Absolute Pointing algoritm based on projected angle of finger translated to screen position
//Explicitely updates Pointer and Circles scripts

using UnityEngine;
using System.Collections;
using Leap;

public class AbsolutePointing : MonoBehaviour {
	
	//Leap
	Controller controller = new Controller();
	Frame frame;								//Frame of Leap data - updated every frame
	FingerList fingers;							//List of fingers from frame
	Leap.Vector vLeapIntersect;					//Position of intersection of finger [0]
	Vector3 vScreenIntersect;					//Vector for conversion of Leap. Position of intersection vLeapIntersect to Unity.Vector3
	Leap.Screen screen;							//Screen object used for Intersection of ray from finger
	
	//Screen
	float scale;								//Scale of Leap window to screen size
	Vector3 vScale;								//Vector prepped for scale value to apply to Leap data vectors
	Vector3 vScreenCoords;						//Converted screen coordinates of Leap data - front most finger
	
	//Leap Zone Dimensions
	public int screenWidth;			//Public just to display runtime value in Inspector
	public int screenHeight;		//Public just to display runtime value in Inspector
	
	//GUI scripts
	Pointer pointer;
	
	//Bools
	bool pointerGrab = false;		//true if two fingers (ready to grab circle) / false if one finger (grab circle if collision)
	
	//TEMP GUI outputs
	public GUIText guiTextRaw;
	public GUIText guiTextMouse;
	public GUIText guiTextMouseToScreen;
	public GUIText guiTextUnityToScreen;
	
	
	

	// Use this for initialization
	void Start () {
		//screen dimensions
		screenWidth = UnityEngine.Screen.width;
		screenHeight = UnityEngine.Screen.height;
	
	}
	
	// Update is called once per frame
	void Update () {
		//Update Leap data
		UpdateLeap();
		
		//Convert Leap data to screen coordinates
		LeapToScreen();
		
		//Update Pointer
//		pointer.Update(vScreenCoords, pointerGrab);

		//TEMP update GUIText output
//		UpdateGuiText();
	
	}
	
	//Create static labels on screen
	void OnGUI(){
		Rect labelBox = new Rect(UnityEngine.Screen.width/2-100, UnityEngine.Screen.height-50,200,20);
		Rect instructionBox = new Rect(UnityEngine.Screen.width/2-100, UnityEngine.Screen.height-30,200,20);
		GUI.Label(labelBox,"ABSOLUTE POINTING MODE");
		GUI.Label(instructionBox,"Press 'SPACE' to drag circles");
    }
	
	//Update leap data
	void UpdateLeap(){
		frame = controller.Frame();							//Get current frame data
		fingers = frame.Fingers;								//Update finger list from frame
		// Get the first finger in the list of fingers
        Finger finger = frame.Fingers[0];
		// Get the closest screen intercepting a ray projecting from the finger
        screen = controller.LocatedScreens.ClosestScreenHit(finger);  			//NOTE - CallibratedScreens depreciated in favour of LocatedScreens
		// Intersect point of ray with projected Leap screen
		vLeapIntersect = screen.Intersect(finger, true);
		//Convert Leap.Vector to Unity.Vector3 (floats)
		vScreenIntersect = new Vector3(vLeapIntersect.ToFloatArray()[0],vLeapIntersect.ToFloatArray()[1],vLeapIntersect.ToFloatArray()[2]);

////TEMP///////////		
		guiTextRaw.text = "Raw data: X: " + Round(vScreenIntersect.x, 2) + " Y: " + Round(vScreenIntersect.y,2) + " Z: " + Round(vScreenIntersect.z, 2);
		
		
		
	}
	
	//Convert Leap input to screen coordinates
	void LeapToScreen(){
//		//Transpose x axis for width offset and y axis for Leap window height
//		float transX = vFingerUnity.x + leapWidth/2;
//		//Remove Leap window height and invert y axis
//		float transY = (vFingerUnity.y - leapCentreY + leapHeight/2) * -1 + leapHeight;
//		Vector3 vTransposedFingerUnity = new Vector3(transX, transY, vFingerUnity.z);
//		//Scale leap data to screen dimensions
//		vScreenCoords = Vector3.Scale(vTransposedFingerUnity, vScale);	
	}
	
	
	
	
	
	
	//TEMP update GUIText output
	void UpdateGuiText(){
//		guiTextRaw.text = "Raw data: X: " + Round(vFinger.x, 2) + " Y: " + Round(vFinger.y,2) + " Z: " + Round(vFinger.z, 2);
		guiTextMouse.text = "Mouse pos: X " + Input.mousePosition.x + "   Y " + Input.mousePosition.y;
			float mousePosX = Input.mousePosition.x;
			float mousePosY = Input.mousePosition.y;
			float screenX = mousePosX;
			float screenY = (mousePosY * -1) + UnityEngine.Screen.height;
//		guiTextMouseToScreen.text = "Mouse coords: X " + screenX + "   Y " + screenY;	
//		guiTextUnityToScreen.text = "Leap coords: X " + Round(vScreenCoords.x, 2) + "   Y " + Round(vScreenCoords.y, 2);
		
		
	}
	
	public static float Round(float value, int digits)
    {
    	float mult = Mathf.Pow(10.0f, (float)digits);
    	return Mathf.Round(value * mult) / mult;
    }
}
