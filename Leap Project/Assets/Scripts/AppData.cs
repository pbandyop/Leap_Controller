/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

//***** MUST BE ATTACHED TO OBJECT LOADED BY BOOTSTRAP SCENE *****//
//To prevent multiple instances - avoids need for singleton class

//Class stores all data that is used across multiple scenes

using UnityEngine;
using System.Collections;

//Global enum for pointing mode
public enum Mode
	{Mouse, Relative, Absolute, Calibration};

public class AppData : MonoBehaviour {
	
	//APP DATA
	public bool bSpaceGrabMode;				//true for SPACE bar for grab command, false for thumb pinch action
	public bool bPointerGrab;				//Records if pointer grabbed
	public Mode pointingMode;				//Current pointing mode	
	public Vector2 vCursorPos;				//Position of cursor in Unity window
	public Vector2 vMousePos;				//Position of mouse on screen (irispective of Unity window size)
	public float fSensitivity;				//Sensitivity of pointing motion - range 0.50 to 1.50 Default 1 
	public int leapWidth;					//width of leap zone (for relative mode)
	public int leapCentreY;					//Height of CENTRE of leap zone (for relative mode)
	public float leapZoneScale;				//Scaling used to fine tune size of leap zone (for relative mode)
	
	//Make Object persistent across all levels
	void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

	// Use this for initialization
	void Start () {
		bSpaceGrabMode = true;
		bPointerGrab = false;
		pointingMode = Mode.Mouse;			//Default to mouse mode
		fSensitivity = 1.0f;				//Default sensitivity
		leapWidth = 200;					//Default leap zone width
		leapCentreY = 200;					//Default height of CENTRE of leap zone
		leapZoneScale = 1.0f;				//Default scaling of leap zone
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
