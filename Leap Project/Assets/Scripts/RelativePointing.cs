/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

//ADD TO CAMERA
//REQUIRES Pointer and Circles scripts
//Captures data from leap
//Detects and handles gestures
//Implements Relative Pointing algoritm based on defined Leap window of coords translated to screen position
//Explicitely updates Pointer and Circles scripts

using UnityEngine;
using System.Collections;
using Leap;

public class RelativePointing : MonoBehaviour {
	
	//Leap
	Controller controller;					//Main Leap controller
	Frame frame;							//Frame of Leap data - updated every frame
	FingerList fingers;						//List of fingers from frame		
	Leap.Vector vFinger;					//Leap data for coords of foremost finger
	Vector3 vFingerUnity;					//Vector for conversion of Leap.Vector data vFinger to Unity.Vector3
	
	//Screen
	float scale;							//Scale of Leap window to screen size
	Vector3 vScale;							//Vector prepped for scale value to apply to Leap data vectors
	Vector3 vScreenCoords;					//Converted screen coordinates of Leap data - front most finger
	
	//Leap Zone Dimensions
	public int leapCentreY = 200;			//Define centre point for Leap window - Configurable in inspector
	public int leapWidth = 250;				//Define width of Leap window - Configurable in inspector
	public int leapHeight;					//Public just to display runtime value in Inspector
	public int leapLeft;					//Public just to display runtime value in Inspector
	public int leapRight;					//Public just to display runtime value in Inspector
	public int leapTop;						//Public just to display runtime value in Inspector
	public int leapBottom;					//Public just to display runtime value in Inspector
	public int screenWidth;					//Public just to display runtime value in Inspector
	public int screenHeight;				//Public just to display runtime value in Inspector
	
	//GUI scripts
	Pointer pointer;						//Pointer script
	GameObject dataObject;					//Persistent object holding AppData script
	AppData data;							//Script storing global app data required across scenes


	//TEMP GUI outputs
	public GUIText guiTextRaw;
	public GUIText guiTextMouse;
	public GUIText guiTextMouseToScreen;
	public GUIText guiTextUnityToScreen;

	// Use this for initialization
	void Start () {
		//Initialise Leap controller
		controller = new Controller();
		//Set up Leap window dimensions
		//screen dimensions
		screenWidth = UnityEngine.Screen.width;
		screenHeight = UnityEngine.Screen.height;
		//Calculate Leap window based on screen dimensions + predefined width and vertical centre
		//Leap window width to screen width ratio
		scale = (float)screenWidth / leapWidth;					//cast to float to force float result
		vScale = new Vector3(scale, scale, scale);
		//leapHeight as a ratio of screen height to width.
		leapHeight = Mathf.RoundToInt(screenHeight / scale);
		//Leap window calculations
		leapLeft = leapWidth/2 * -1;
		leapRight = leapWidth/2;
		leapTop = leapCentreY + leapHeight/2;
		leapBottom = leapCentreY - leapHeight/2;
		//Link scripts
		pointer = GetComponent<Pointer>();
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();
	
	}
	
	// Update is called once per frame
	void Update () {
		//Update Leap data
		UpdateLeap();
		//Convert Leap data to screen coordinates
		LeapToScreen();
		//IF not in SPACE bar mode, Test for pointer grab - else leave grab detection to KeyboardInput
		if (!data.bSpaceGrabMode)
		{
			TestGrab();
		}
		//Update Pointer
//		pointer.Update(vScreenCoords, data.bPointerGrab);

		//TEMP update GUIText output
		UpdateGuiText();
	
	}
	
	//Create static labels on screen
	void OnGUI(){
		Rect labelBox = new Rect(UnityEngine.Screen.width/2-100, UnityEngine.Screen.height-50,200,20);
		Rect instructionBox = new Rect(UnityEngine.Screen.width/2-100, UnityEngine.Screen.height-30,200,20);
		GUI.Label(labelBox,"RELATIVE POINTING MODE");
    }
	
	//Update leap data
	void UpdateLeap(){
		frame = controller.Frame ();							//Get current frame data
		fingers = frame.Fingers;								//Update finger list from frame
		vFinger = fingers.Frontmost.StabilizedTipPosition;		//Get pos of frontmost finger from list

		//Convert Leap.Vector to Unity.Vector3 (floats)
		vFingerUnity = new Vector3(vFinger.ToFloatArray()[0],vFinger.ToFloatArray()[1],vFinger.ToFloatArray()[2]);
		
	}
	
	//Convert Leap input to screen coordinates
	void LeapToScreen(){
		//Transpose x axis for width offset and y axis for Leap window height
		float transX = vFingerUnity.x + leapWidth/2;
		//Remove Leap window height and invert y axis
		float transY = (vFingerUnity.y - leapCentreY + leapHeight/2) * -1 + leapHeight;
		Vector3 vTransposedFingerUnity = new Vector3(transX, transY, vFingerUnity.z);
		//Scale leap data to screen dimensions
		vScreenCoords = Vector3.Scale(vTransposedFingerUnity, vScale);	
	}
	
	//Test for grab action
	void TestGrab(){
		//If two fingers detected (finger and thumb spread)
		if (fingers.Count > 1){
			data.bPointerGrab = false;
		}
		//otherwise true (thumb against hand so not detected)
		else{
			data.bPointerGrab = true;
		}
	}
	
	
	
	
	//TEMP update GUIText output
	void UpdateGuiText(){
		guiTextRaw.text = "Raw data: X: " + Round(vFinger.x, 2) + " Y: " + Round(vFinger.y,2) + " Z: " + Round(vFinger.z, 2);
		guiTextMouse.text = "Mouse pos: X " + Input.mousePosition.x + "   Y " + Input.mousePosition.y;
			float mousePosX = Input.mousePosition.x;
			float mousePosY = Input.mousePosition.y;
			float screenX = mousePosX;
			float screenY = (mousePosY * -1) + UnityEngine.Screen.height;
		guiTextMouseToScreen.text = "Mouse coords: X " + screenX + "   Y " + screenY;	
		guiTextUnityToScreen.text = "Leap coords: X " + Round(vScreenCoords.x, 2) + "   Y " + Round(vScreenCoords.y, 2);
		
		
	}
	
	public static float Round(float value, int digits)
    {
    	float mult = Mathf.Pow(10.0f, (float)digits);
    	return Mathf.Round(value * mult) / mult;
    }
	
	
}
