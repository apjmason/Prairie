using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[AddComponentMenu("Prairie/Utility/Twine Node")]
public class TwineNode : MonoBehaviour {

	public GameObject[] objectsToTrigger;

	[HideInInspector]
	public string pid;
	public new string name;
	[HideInInspector]
	public string[] tags;
	public string content;
	public GameObject[] children;
	[HideInInspector]
	public string[] childrenNames;
	public List<GameObject> parents = new List<GameObject> ();
	public bool isDecisionNode;

	private bool isMinimized = false;
	private bool isOptionsGuiOpen = false;

	private int selectedOptionIndex = 0;

	void Update ()
	{
		if (this.enabled) {
			if (Input.GetKeyDown (KeyCode.Q)) {
				this.isMinimized = !this.isMinimized;
			}

			if (this.isDecisionNode) {
				if (this.isOptionsGuiOpen && Input.GetKeyDown (KeyCode.Tab)) {
					// Press TAB to scroll through the children nodes
					this.selectedOptionIndex = (this.selectedOptionIndex + 1) % (children.Length);
				} else if (this.isOptionsGuiOpen && (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return))) {
					// Press ENTER or RETURN to select that child
					this.ActivateChildAtIndex (selectedOptionIndex);
				} else if (this.isOptionsGuiOpen && Input.GetKeyDown (KeyCode.E)) {
					// E closes the options list
					this.isOptionsGuiOpen = false;
				}

				// TAB opens the options list if it's not already open
				if (!this.isOptionsGuiOpen && Input.GetKeyDown (KeyCode.Tab)) {
					this.isOptionsGuiOpen = true;
				}
			}
		}
	}

	public void OnGUI()
	{
		if (this.enabled && !this.isMinimized) {

			float frameWidth = Math.Min(Screen.width / 3, 350);
			float frameHeight = Math.Min(Screen.height / 2, 500);
			Rect frame = new Rect (10, 10, frameWidth, frameHeight);

			GUI.BeginGroup (frame);
			GUIStyle style = new GUIStyle (GUI.skin.box);
			style.wordWrap = true;
			style.fixedWidth = frameWidth;
			GUILayout.Box (this.content, style);

			if (isDecisionNode) {
				GUIStyle decisionHintStyle = new GUIStyle (style);
				decisionHintStyle.fontStyle = FontStyle.BoldAndItalic;

				if (!isOptionsGuiOpen) {
					GUILayout.Box ("Press TAB to progress in the story...", decisionHintStyle);
				} else {
					GUILayout.Box ("Press TAB to scroll, E to close, ENTER to choose", decisionHintStyle);
				}
			}

			if (this.isOptionsGuiOpen) {
				// Draw list of buttons for the possible children nodes to visit:
				GUIStyle optionButtonStyle = new GUIStyle (GUI.skin.button);
				optionButtonStyle.fontStyle = FontStyle.Italic;
				optionButtonStyle.wordWrap = true;

				// Set highlighted button to have green text (this state is called `onNormal`):
				optionButtonStyle.onNormal.textColor = Color.white;
				// Set non-highlighted buttons to have grayed out text (state is called `normal`)
				optionButtonStyle.normal.textColor = Color.gray;

				selectedOptionIndex = GUILayout.SelectionGrid(selectedOptionIndex, this.childrenNames, 1, optionButtonStyle);
			}
			
			GUI.EndGroup ();

		} else if (this.enabled && this.isMinimized) {

			// Draw minimized GUI instead
			Rect frame = new Rect (10, 10, 10, 10);

			GUI.Box (frame, "");

		}
	
	}

	/// <summary>
	/// Trigger the interactions associated with this Twine Node.
	/// </summary>
	/// <param name="interactor"> The interactor acting on this Twine Node, typically a player. </param>
	public void StartInteractions(GameObject interactor) 
	{
		if (this.enabled) 
		{
			foreach (GameObject gameObject in objectsToTrigger) 
			{
				gameObject.InteractAll (interactor);
			}
		}
	}

	/// <summary>
	/// Activate this TwineNode (provided it isn't already
	/// 	active/enabled and it has some active parent)
	/// </summary>
	/// <param name="interactor">The interactor.</param>
	public bool Activate(GameObject interactor)
	{
		if (!this.enabled && this.HasActiveParentNode()) 
		{
			this.enabled = true;
			this.isMinimized = false;
			this.isOptionsGuiOpen = false;
			this.DeactivateAllParents ();
			this.StartInteractions (interactor);

			return true;
		}

		return false;
	}

	/// <summary>
	/// Find the FirstPersonInteractor in the world, and use it to activate
	/// 	the TwineNode's child at the given index.
	/// </summary>
	/// <param name="index">Index of the child to activate.</param>
	private void ActivateChildAtIndex(int index) 
	{
		// Find the interactor:
		FirstPersonInteractor interactor = (FirstPersonInteractor) FindObjectOfType(typeof(FirstPersonInteractor));

		if (interactor != null) {
			GameObject interactorObject = interactor.gameObject;
		
			// Now activate the child using this interactor!
			TwineNode child = this.children [index].GetComponent<TwineNode> ();
			child.Activate (interactorObject);
		}
	}

	public void Deactivate() 
	{
		this.enabled = false;
	}

	/// <summary>
	/// Check if this Twine Node has an active parent node.
	/// </summary>
	/// <returns><c>true</c>, if there is an active parent node, <c>false</c> otherwise.</returns>
	public bool HasActiveParentNode() 
	{
		foreach (GameObject parent in parents) 
		{
			if (parent.GetComponent<TwineNode> ().enabled) 
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Deactivate all parents of this Twine Node.
	/// </summary>
	private void DeactivateAllParents()
	{
		foreach (GameObject parent in parents) 
		{
			parent.GetComponent<TwineNode> ().Deactivate ();
		}
	}

	// GIZMOS

	void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		foreach (GameObject child in this.children)
		{
			Gizmos.DrawLine(transform.position, child.transform.position);
		}

		Gizmos.color = Color.gray;

		// TODO: draw text and such...
		// 		 just mark position with a sphere for now...
		Gizmos.DrawSphere(transform.position, 0.1f);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		foreach (GameObject gameObject in objectsToTrigger)
		{
			// Draw cyan line(s) between the current Twine node and the object(s) it triggers
			Gizmos.DrawLine(transform.position, gameObject.transform.position);
		}
	}
}
