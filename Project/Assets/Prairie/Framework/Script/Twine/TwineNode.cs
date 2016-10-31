using UnityEngine;
using System.Collections;

public class TwineNode : MonoBehaviour {

	public string pid;
	public string name;
	public string[] tags;
	public string content;
	public string[] childrenNames;
	public GameObject[] children;
	
	public GameObject[] triggeredObjects;

	// use the unity 'enabled' thing to keep track of whether
	// a twine node is triggerable or not
	// the below doesn't do anything, but was a pseudocode-ish
	// attempt at iterating through the triggeredObjects to trigger them
	/*
	public static void TriggerNode() {
		for (int i = 0; i < triggeredObjects.Length; i++) {
			triggeredObjects[i].GetComponent <Interaction> ();
		}
	} */
}
