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
			if (this.isDecisionNode) {
			    this.isOptionsGuiOpen = true;
			} else if (Input.GetKeyDown(KeyCode.Q)) {
                this.isMinimized = !this.isMinimized;
            }
		}
	}

	public void OnGUI()
	{
		if (this.enabled && !this.isMinimized) {
            float horizontalAlign;
            float verticalAlign;
            float frameWidth;
            float frameHeight;
            if (!this.isDecisionNode) {
                horizontalAlign = 10;
                verticalAlign = 10;
                frameWidth = Math.Min(Screen.width / 3, 350);
                frameHeight = Math.Min(Screen.height / 2, 500);
            } else {
                frameWidth = Math.Min(Screen.width / 3, 500);
                frameHeight = Math.Min(Screen.height / 2, 350);
                horizontalAlign = (Screen.width - frameWidth) / 2;
                verticalAlign = Screen.height - frameHeight;
            }
            Rect frame = new Rect(horizontalAlign, verticalAlign, frameWidth, frameHeight);

            GUI.BeginGroup (frame);
			GUIStyle style = new GUIStyle (GUI.skin.box);
            style.normal.textColor = Color.white;
			style.wordWrap = true;
			style.fixedWidth = frameWidth;
			GUILayout.Box (this.content, style);

            FirstPersonInteractor player = (FirstPersonInteractor)FindObjectOfType(typeof(FirstPersonInteractor));
            if (this.isOptionsGuiOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                player.SetCanMove(false);
                player.SetDrawsGUI(false);
                for (int index = 0; index < this.childrenNames.Length; index++)
                {
                    if (GUILayout.Button(this.childrenNames[index]))
                    {
                        this.ActivateChildAtIndex(index);
                    }
                }
            }
            else {
                player.SetCanMove(true);
                player.SetDrawsGUI(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
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
}
