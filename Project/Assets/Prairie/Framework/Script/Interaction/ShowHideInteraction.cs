using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideInteraction : PromptInteraction {

	// Use this for initialization
	void Start () {
		this.gameObject.GetComponent<Renderer> ().enabled = false;
	}

	public bool isTwineNode
	{
		get { return this.gameObject.GetComponent<AssociatedTwineNodes>() != null; }
	}

	// check if the current object has an active twine node
	public bool isAssociatedTwineNodeActive()
	{
		AssociatedTwineNodes nodes = this.gameObject.GetComponent<AssociatedTwineNodes>();
		if (nodes == null) { return false; }  // sanity check

		foreach (GameObject twineNodeObject in nodes.associatedTwineNodeObjects)
		{
			TwineNode twineNode = twineNodeObject.GetComponent<TwineNode> ();
			if (twineNode.enabled)
			{
				return true;
			}
		}

		return false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) // Checks to see if left mouse button was clicked.
		{
			Debug.Log ("Clicked on " + this.gameObject.name);
			PerformAction();
		}
	}

	protected override void PerformAction () 
	{
		if (this.isTwineNode)
		{
			if (this.isAssociatedTwineNodeActive ()) {
				Debug.Log ("Associated twine node of " + this.gameObject.name + " is currently active.");	
				this.gameObject.GetComponent<Renderer>().enabled = !this.gameObject.GetComponent<Renderer>().enabled;
			} else {
				Debug.Log ("Associated twine node of " + this.gameObject.name + "is currently inactive.");
			}
		}
	}

}
