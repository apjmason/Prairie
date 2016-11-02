using UnityEngine;
using System.Collections.Generic;

public class FirstPersonInteractor : MonoBehaviour
{
	public float interactionRange = 3;

	private Camera viewpoint;

	private List<Interaction> avaliableInteractions;
	private bool interactionAvaliable
	{
		get { return avaliableInteractions.Count > 0; }
	}


	void Start ()
	{
		viewpoint = Camera.main;
	}
		
	void Update ()
	{
		this.avaliableInteractions = GetAvaliableInteractions ();

		if (Input.GetKeyDown (KeyCode.F))
		{
			this.AttemptInteract();
		}
	}

	void OnGUI()
	{
		if (this.interactionAvaliable)
		{
			// Draw a GUI with the interaction
			Rect frame = new Rect (Screen.width / 2, Screen.height / 2, Screen.width / 4, Screen.height / 4);
			string firstInteractionPrompt = avaliableInteractions [0].prompt;
            GUI.BeginGroup(frame);
			GUILayout.Box ("Press F to " + firstInteractionPrompt);
            GUI.EndGroup();
		}
		else 
		{
			// hacky way to draw a crosshair 
			Rect frame = new Rect (Screen.width / 2, Screen.height / 2, 10, 10);
			GUI.Box (frame, "");
		}

	}

	private void AttemptInteract ()
	{
		if (interactionAvaliable)
		{
			foreach (Interaction target in avaliableInteractions)
			{
				target.Interact (this.gameObject);
			}
		}
	}

	private List<Interaction> GetAvaliableInteractions ()
	{
		// perform a raycast from the main camera to an object in front of it
		// the object must have a collider to be hit, and an `Interaction` to be added
		// to this interactor's interaction list

		Vector3 origin = viewpoint.transform.position;
		Vector3 fwd = viewpoint.transform.TransformDirection (Vector3.forward);

		RaycastHit hit;		// we pass this into the raycast function and it populates it with a result

		if (Physics.Raycast (origin, fwd, out hit, interactionRange))
		{
			if (hit.collider.isTrigger)
			{
				// ignore non-physical colliders, such as trigger areas
				return new List<Interaction> ();
			}

			GameObject obj = hit.transform.gameObject;
			List<Interaction> enabledInteractions = new List<Interaction> ();
			foreach (Interaction i in obj.GetComponents<Interaction> ())
			{
				if (i.enabled)
				{
					enabledInteractions.Add (i);
				}
			}
			return enabledInteractions;
		}
		else
		{
			return new List<Interaction> ();	// no interactions, empty list
		}
	}

}