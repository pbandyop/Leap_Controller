/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

//ADD TO CAMERA
//Used by Circles to implement onscreen timer
//Create GUIText object in scene and link to guiTextTimer in Inspector 


using UnityEngine;
using System.Collections;

public class TaskTimer : MonoBehaviour {
	
	public GUIText guiTextTimer;
	float time = 0f;
	public bool running = false;
//	float myTimePoint = 0f;
	

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update (){
		if (running){
			time += Time.deltaTime;
			//time = Time.time - myTimePoint;
		}
		
		guiTextTimer.text = "time: " + Round(time, 2);
		
	}
	
	public void StartTimer(){
		running = true;
//		myTimePoint = Time.time;
	}
	
	public void StopTimer(){
		running = false;
	}
	
	public void Reset(){
		running = false;
		time = 0f;
		
	}

	public static float Round(float value, int digits)
    {
    	float mult = Mathf.Pow(10.0f, (float)digits);
    	return Mathf.Round(value * mult) / mult;
    }
	
	
	
	
	
}
