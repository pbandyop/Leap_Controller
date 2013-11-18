/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

//ADD TO CAMERA
//Draws pointer to coordinates
//Updated by main pointing script

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
	
	//Override Update
	//Update coordinates - called by main ......Pointing script
//	public void Update(Vector3 coords, bool grabbed){
//		v3Position = coords;
//		bPointerGrabbed = grabbed;
//	}
	
	//Draw pointer at most recent coordinates
	void OnGUI() {
		//Change texture based on if pointer in 'grabbed' mode and draw to pointer position
		if (bPointerGrabbed){
			GUI.DrawTexture(new Rect(vPosition.x - radius, vPosition.y - radius, radius*2, radius*2), pointerGrabbedTexture);
		}else{
			GUI.DrawTexture(new Rect(vPosition.x - radius, vPosition.y - radius, radius*2, radius*2), pointerTexture);
		}
		
		
		
		//DrawPointerCircle(v3Position.x,vPosition.y);
	}
	
		
	void DrawPointerCircle(float xCoord, float yCoord){
		
		//Change texture based on if pointer in 'grabbed' mode and draw to coords
		if (bPointerGrabbed){
			GUI.DrawTexture(new Rect(xCoord - radius, yCoord - radius, radius*2, radius*2), pointerGrabbedTexture);
		}else{
			GUI.DrawTexture(new Rect(xCoord - radius, yCoord - radius, radius*2, radius*2), pointerTexture);
		}
	}
}
 
