/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

//ADD TO CAMERA
//REQUIRES Pointer and Timer
//Draws circles and targets
//Handles hit tests between graphical objects
//Records states of circles
//Controls Timer

using UnityEngine;
using System.Collections;

public class Circles : MonoBehaviour {
	
	//Graphics
	public int radiusCircle;
	public int radiusTarget;
	public float circleScreenOffset = 0.4f;
	public float targetScreenOffset = 0.2f;
	public Texture	texRedCircle, texYellowCircle, texBlueCircle, texGreenCircle;
	public Texture	texRedTarget, texYellowTarget, texBlueTarget, texGreenTarget;
	public Texture	texRedTargetHit, texYellowTargetHit, texBlueTargetHit, texGreenTargetHit;
	Texture	texRedTargetCurrent, texYellowTargetCurrent, texBlueTargetCurrent, texGreenTargetCurrent;
	Vector2 vRedBasePos, vYellowBasePos, vBlueBasePos, vGreenBasePos;			//Base circle positions
	Vector2 vRedTarPos, vYellowTarPos, vBlueTarPos, vGreenTarPos;				//Target positions
	Vector2 vRedPos, vYellowPos, vBluePos, vGreenPos;							//Current positions of circles - updated by UpdateGUI
	
	//Timer script
	TaskTimer timer;
	
	//AppData links
	GameObject dataObject;				//Persistent object holding AppData script
	AppData data;						//Script storing global app data required across scenes
	
	//Logic
	Vector2 vPointer;														//Position of pointer
	Vector2 vHitOffset;														//offset of pointer 'grab' pos to centre of circle
	bool pointerGrabbed = false;											//True if pointer in 'grabbed' mode
	bool pointerGrabbedPrev = false;										//Records previous state of pointerGrabbed
	bool redActive, yellowActive, blueActive, greenActive;					//Circles active until successfully dropped on targets
	int circleGrabbed = 0;	// 0=NONE; 1=red; 2=yellow; 3=blue; 4=green		//Records which circle currently grabbed
	int completedCount = 0;													//Tracks number of completed circle drops
	int hitRadius;														//Distance from centre of target for circle position to be a hit
	
	// Use this for initialization
	void Start () {
		//Link AppData
		dataObject = GameObject.Find("GlobalDataObject");
		data = dataObject.GetComponent<AppData>();
		
		//Setup circle positions
		float xOffset = UnityEngine.Screen.width * circleScreenOffset;
		float yOffset = UnityEngine.Screen.height * circleScreenOffset;
		vRedBasePos = new Vector2(xOffset, yOffset);
		vYellowBasePos = new Vector2(UnityEngine.Screen.width - xOffset, yOffset);
		vBlueBasePos = new Vector2(xOffset, UnityEngine.Screen.height - yOffset);
		vGreenBasePos = new Vector2(UnityEngine.Screen.width - xOffset, UnityEngine.Screen.height - yOffset);
		//Setup target positions
		xOffset = UnityEngine.Screen.width * targetScreenOffset;
		yOffset = UnityEngine.Screen.height * targetScreenOffset;
		vRedTarPos = new Vector2(xOffset, yOffset);
		vYellowTarPos = new Vector2(UnityEngine.Screen.width - xOffset, yOffset);
		vBlueTarPos = new Vector2(xOffset, UnityEngine.Screen.height - yOffset);
		vGreenTarPos = new Vector2(UnityEngine.Screen.width - xOffset, UnityEngine.Screen.height - yOffset);
		//setup target textures
		texRedTargetCurrent = texRedTarget;
		texYellowTargetCurrent = texYellowTarget;
		texBlueTargetCurrent = texBlueTarget;
		texGreenTargetCurrent = texGreenTarget;
		
		
		//Get AppData config
		radiusTarget = data.targetRadius;
		radiusCircle = data.circleRadius;
		//Calculate hit radius - the distance centre of circle needs to be within centre of target to register a hit
		//Based on target radius less circle radius
		hitRadius = radiusTarget - (radiusCircle + 3);
		
		//Link timer script
		timer = GetComponent<TaskTimer>();
		
		
		
		//Use Reset for initialisation of logic
		Reset();
	
	}
	
	// Update is called once per frame
	void Update () {
		//Update circle positions and states
		UpdateGUI();
		
	}
	
	// MAIN SCRIPT LOGIC //
	//Get pointer coordinates and initiates circle state update
	public void UpdateGUI(){
		//convert pointer coords from V3 to V2 and update position + state
		vPointer = new Vector2(data.vCursorPos.x, data.vCursorPos.y);
//		vPointer = new Vector2(pointer.vPosition.x, pointer.vPosition.y);
		pointerGrabbed = data.bPointerGrab;

		//Execute logic based on which circle grabbed
		//0=NONE; 1=red; 2=yellow; 3=blue; 4=green
		switch (circleGrabbed){
			case 0:							//No circle grabbed
			//IF pointer is grabbed for 1st time initiate hittests
			if (pointerGrabbed && !pointerGrabbedPrev)
			{
				if(HitTest(vPointer, vRedPos, radiusCircle - 5) && redActive)
				{
					circleGrabbed = 1;	
					HitOffset(vPointer, vRedPos);
				}
				else if(HitTest(vPointer, vYellowPos, radiusCircle - 5) && yellowActive)
				{
					circleGrabbed = 2;
					HitOffset(vPointer, vYellowPos);
				}
				else if(HitTest(vPointer, vBluePos, radiusCircle - 5) && blueActive)
				{
					circleGrabbed = 3;	
					HitOffset(vPointer, vBluePos);
				}
				else if(HitTest(vPointer, vGreenPos, radiusCircle - 5) && greenActive)
				{
					circleGrabbed = 4;
					HitOffset(vPointer, vGreenPos);
				}
				
				//If timer not running = 1st grab of any circle, so start timer
				if ((circleGrabbed > 0) && (!timer.running))
				{
					timer.StartTimer();	
				}
			
			}	
			   break;
			
			case 1:							//RED circle grabbed
			//Update circle position to pointer plus offset for drawing circle linked to pointer
			vRedPos = vPointer + vHitOffset;
			   break;
			
			case 2:							//YELLOW circle grabbed
			//Update circle position to pointer plus offset for drawing circle linked to pointer
			vYellowPos = vPointer + vHitOffset;
			   break;
			
			case 3:							//BLUE circle grabbed
			//Update circle position to pointer plus offset for drawing circle linked to pointer
			vBluePos = vPointer + vHitOffset;
			   break;
			
			case 4:							//GREEN circle grabbed
			//Update circle position to pointer plus offset for drawing circle linked to pointer
			vGreenPos = vPointer + vHitOffset;
			   break;
			
			default:
			  //CODE
			   break;
			}
		
		//DROP CIRCLE
		//If pointer not grabbed, and a circle is grabbed, drop current circle
		//then test for hit on target and reset to base position if miss
		if (!pointerGrabbed && circleGrabbed > 0){
			//Test for hit on target if last pointerGrabbed was true
			if(pointerGrabbedPrev){
				TargetHit();
			}
			//Reset active circle (if missed target)
			ResetGrabbedCircle();
			circleGrabbed = 0;
			
			//If completedCount = 4 last task is complete, so stop timer
			if (completedCount == 4)
			{
				timer.StopTimer();	
			}
		}
		
		//Update pointerGrabbed record
		pointerGrabbedPrev = pointerGrabbed;
	}

	// Draw to screen
	void OnGUI() {
		//Draw all circles and targets
		DrawGUI ();
		
	}
	
	//Draw all circles and targets at updated positions - texture based on state (if necessary for user feedback?)
	void DrawGUI ()
	{
		

		//Draw targets - change target texture if circle hovering on target
		switch (circleGrabbed){
			case 1:							//RED circle grabbed
			if(HitTest(vRedPos, vRedTarPos, hitRadius))
				{
					texRedTargetCurrent = texRedTargetHit;
				}else{
					texRedTargetCurrent = texRedTarget;
				}
			   break;
			
			case 2:							//YELLOW circle grabbed
			if(HitTest(vYellowPos, vYellowTarPos, hitRadius))
				{
					texYellowTargetCurrent = texYellowTargetHit;
				}else{
					texYellowTargetCurrent = texYellowTarget;
				}
			   break;
			
			case 3:							//BLUE circle grabbed
			if(HitTest(vBluePos, vBlueTarPos, hitRadius))
				{
					texBlueTargetCurrent = texBlueTargetHit;
				}else{
					texBlueTargetCurrent = texBlueTarget;
				}
			   break;
			
			case 4:							//GREEN circle grabbed
			if(HitTest(vGreenPos, vGreenTarPos, hitRadius))
				{
					texGreenTargetCurrent = texGreenTargetHit;
				}else{
					texGreenTargetCurrent = texGreenTarget;
				}
			   break;
			
			default:
			  //CODE
			   break;
			}		
		Draw(vRedTarPos, radiusTarget, texRedTargetCurrent);
		Draw(vYellowTarPos, radiusTarget, texYellowTargetCurrent);
		Draw(vBlueTarPos, radiusTarget, texBlueTargetCurrent);
		Draw(vGreenTarPos, radiusTarget, texGreenTargetCurrent);
		
		//Draw circles
		Draw(vRedPos, radiusCircle, texRedCircle);
		Draw(vYellowPos, radiusCircle, texYellowCircle);
		Draw(vBluePos, radiusCircle, texBlueCircle);
		Draw(vGreenPos, radiusCircle, texGreenCircle);
	}
	
	//Draw texture @ position
	void Draw(Vector2 pos, int radius, Texture texture){
		GUI.DrawTexture(new Rect(pos.x - radius, pos.y - radius, radius*2, radius*2), texture);
	}
	
	//hittest - Returns True if distance betweeen two Vector2's is less than test distance
	bool HitTest(Vector2 v1, Vector2 v2, int dist){
		float vDist = Vector2.Distance(v1, v2);
		return (vDist < dist);
	}
	
	//Calc and update hit offset vector - used for dragging
	void HitOffset(Vector2 pointer, Vector2 circle){
		vHitOffset = new Vector2(circle.x - pointer.x, circle.y - pointer.y);
	}
	
	//Test for circle dropped on target
	//If circle hit on target, deactivate circle and set position to target position
	void TargetHit(){
		switch (circleGrabbed){
			case 1:							//RED circle grabbed
			if(HitTest(vRedPos, vRedTarPos, hitRadius)){
				redActive = false;
				circleGrabbed = 0;
				vRedPos = vRedTarPos;
				texRedTargetCurrent = texRedTarget;
				completedCount++;
			}
			   break;
			
			case 2:							//YELLOW circle grabbed
			if(HitTest(vYellowPos, vYellowTarPos, hitRadius)){
				yellowActive = false;
				circleGrabbed = 0;
				vYellowPos = vYellowTarPos;
				texYellowTargetCurrent = texYellowTarget;
				completedCount++;
			}
			   break;
			
			case 3:							//BLUE circle grabbed
			if(HitTest(vBluePos, vBlueTarPos, hitRadius)){
				blueActive = false;
				circleGrabbed = 0;
				vBluePos = vBlueTarPos;
				texBlueTargetCurrent = texBlueTarget;
				completedCount++;
			}
			   break;
			
			case 4:							//GREEN circle grabbed
			if(HitTest(vGreenPos, vGreenTarPos, hitRadius)){
				greenActive = false;
				circleGrabbed = 0;
				vGreenPos = vGreenTarPos;
				texGreenTargetCurrent = texGreenTarget;
				completedCount++;
			}
			   break;
			
			default:
			  //CODE
			   break;
			}
	}
	
	//Reset active circle to base position and ensure target texture correct
	void ResetGrabbedCircle(){
		switch (circleGrabbed){
			case 1:							//RED circle grabbed
			vRedPos = vRedBasePos;
			texRedTargetCurrent = texRedTarget;
			   break;
			
			case 2:							//YELLOW circle grabbed
			vYellowPos = vYellowBasePos;
			   break;
			
			case 3:							//BLUE circle grabbed
			vBluePos = vBlueBasePos;
			   break;
			
			case 4:							//GREEN circle grabbed
			vGreenPos = vGreenBasePos;
			   break;
			
			default:
			  //CODE
			   break;
			}
	}
	
	//Reset logic and graphics to start state
	void Reset(){
		//reset circles to starting positions
		vRedPos = vRedBasePos;
		vYellowPos = vYellowBasePos;
		vBluePos = vBlueBasePos;
		vGreenPos = vGreenBasePos;
		//reset bools
		pointerGrabbed = false;
		redActive = true;
		yellowActive = true;
		blueActive = true;
		greenActive = true;
		completedCount = 0;
		//no circle grabbed
		circleGrabbed = 0;
		//reset timer	
		timer.Reset();		
		
	}
		
	

}
