/*
 * Chris Blythe, Payal Bandyopadhyay, Farbod Berenjegani, Afaque Hussain, Maninder Singh
 * University of Helsinki
 */

// Bootstap scene, purely to load persistent DataObject to avoid duplication
// removes need for singleton


using UnityEngine;
using System.Collections;

public class Bootstrap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Go straight to Title scene
		Application.LoadLevel("Title");
	}
}
