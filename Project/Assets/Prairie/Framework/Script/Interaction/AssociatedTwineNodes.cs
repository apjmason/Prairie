using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Prairie/Twine/Associated Twine Nodes")]
public class AssociatedTwineNodes : PromptInteraction 
{
	public List<GameObject> associatedTwineNodeObjects = new List<GameObject>();
	public List<int> selectedTwineNodeIndices;

	public void UpdateTwineNodeObjectsFromIndices(TwineNode[] nodes)
	{

		if (this.associatedTwineNodeObjects != null) {
			// Clear the list of twine nodes and re-add them based on the new indices:
			this.associatedTwineNodeObjects.Clear ();
		} else {
			// Make sure it exists!
			this.associatedTwineNodeObjects = new List<GameObject> ();
		}
		foreach (int index in this.selectedTwineNodeIndices) {
			GameObject twineNodeObject = nodes [index].gameObject;
			this.associatedTwineNodeObjects.Add (twineNodeObject);
		}
	}

	void OnDrawGizmosSelected()
	{
		// Draw a green line between a Twine node and its subsequent Twine nodes
		Gizmos.color = Color.green;
		foreach (GameObject twineNodeObject in associatedTwineNodeObjects) {
			Gizmos.DrawLine (transform.position, twineNodeObject.transform.position);
		}
	}

	protected override void PerformAction () 
	{
		foreach (GameObject twineNodeObject in associatedTwineNodeObjects) {
			TwineNode twineNode = twineNodeObject.GetComponent<TwineNode> ();

			if (twineNode != null) {
				// Activate the node!
				if (twineNode.Activate (this.rootInteractor))
				{
					return;
				}
			}
		}
	}

	override public string defaultPrompt {
		get {
			return "Progress the Story";
		}
	}
}

