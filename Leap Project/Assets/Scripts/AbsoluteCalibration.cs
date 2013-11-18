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

public class AbsoluteCalibration : MonoBehaviour {
	
	//Leap
	Controller controller;							//Main Leap controller
	Frame frame;									//Frame of Leap data - updated every frame
	FingerList fingers;								//List of fingers from frame		
	Leap.Vector vFinger;							//Leap data for coords of foremost finger
	GestureList gestures;							//Gestures from frame
	
	//Screen dimensions
	int screenWidthUnity, screenHeightUnity, screenWidthWindows, screenHeightWindows;
	
	//Internal
	int stage;
	Vector2 vLeapCoords;
	
	
	//AppData links
	GameObject dataObject;							//Persistent object holding AppData script
	AppData data;									//Script storing global app data required across scenes
	

	// Use this for initialization
	void Start () {
		//Link AppData
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();
		
				//screen dimensions
		screenWidthUnity = UnityEngine.Screen.width;							//Dimensions of Unity window
		screenHeightUnity = UnityEngine.Screen.height;
		screenWidthWindows = UnityEngine.Screen.currentResolution.width;		//Dimensions of whole screen (regardless of app window size)
		screenHeightWindows = UnityEngine.Screen.currentResolution.height;
		
		//Initialise Leap controller and enable require gestures
		controller = new Controller();
		
		stage = 1;
	
	}
	
	// Update is called once per frame
	void Update () {
		//Update leap data
		UpdateLeap();
		
		
		//Update interface based on stage
		if(stage == 1){
			Stage1();
		}else if (stage == 2){
			Stage2();
		}else {
			Stage3();
		}
	
	}
	
	//Create static labels on screen
	void OnGUI(){
		Rect labelBox = new Rect(UnityEngine.Screen.width/2-100, UnityEngine.Screen.height-50,200,20);
		//Update interface based on stage
		if(stage == 1){
			GUI.Label(labelBox,"STAGE 1");
		}else if (stage == 2){
			GUI.Label(labelBox,"STAGE 2");
		}else {
			GUI.Label(labelBox,"STAGE 3");
		}
	
    }
	
	void Stage1()
	{
		//If space bar down record top left coord
		if (Input.GetKeyDown("space"))
		{
			data.vAbsTopReference.x = vLeapCoords.x;
			data.vAbsTopReference.y = vLeapCoords.y;
			
			stage = 2;
		}
	}
	
	void Stage2()
	{
		//If space bar down record top left coord
		if (Input.GetKeyDown("space"))
		{
			data.vAbsBottomReference.x = vLeapCoords.x;
			data.vAbsBottomReference.y = vLeapCoords.y;
			
			data.pointingMode = Mode.Absolute;
			Application.LoadLevel("AbsolutePointing");
			//stage = 3;
		}
	}
	
	void Stage3()
	{
		
	}
	
	
	///// ABSOLUTE POINTING MODE /////
	void UpdateLeap()
	{
		frame = controller.Frame ();							//Get current frame data
		fingers = frame.Fingers;								//Update finger list from frame
		
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
		
		vLeapCoords = new Vector2(vScreenIntersect.x * screenWidthUnity, (vScreenIntersect.y * screenHeightUnity) * -1 + screenHeightUnity);
		
		print ("vLeapCoords X: " + vLeapCoords.x + " Y: " + vLeapCoords.y);
		

	}
	
	

}
