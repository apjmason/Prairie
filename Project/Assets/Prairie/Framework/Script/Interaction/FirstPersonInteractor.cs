using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

[AddComponentMenu("Prairie/Player/First Person Interactor")]
public class FirstPersonInteractor : MonoBehaviour
{
	// Raycast-related
	public float interactionRange = 3;
	private Camera viewpoint;

	// Selection-related
	private GameObject highlightedObject;
	public List<Annotation> areaAnnotationsInRange = new List<Annotation>();
	public bool annotationsEnabled = true;

	// Control-related
	[HideInInspector]
	private bool drawsGUI = true;

	/// --- Game Loop ---

	void Start ()
	{
		viewpoint = Camera.main;
	}

	void Update ()
	{
		// update our highlighted object
		this.highlightedObject = this.GetHighlightedObject();

		// process input
		if (Input.GetMouseButtonDown (0))
		{
			// left click
			this.AttemptInteract ();
		}
		if (Input.GetMouseButtonDown (1))
		{
			// right click
			this.AttemptReadAnnotation ();
		}
		if (areaAnnotationsInRange.Count != 0 && this.annotationsEnabled)
		{
			for (int a = 0; a < areaAnnotationsInRange.Count; a++)
			{
				if (Input.GetKeyDown ((a+1).ToString()))
				{
					areaAnnotationsInRange[a].Interact (this.gameObject);
				}
			}
		}
	}

	/// --- GUI ---

	void OnGUI()
	{
		if (!this.drawsGUI)
		{
			// hide all GUI in certain contexts (such as while slideshow is playing, etc.)
			return;
		}


		if (this.highlightedObject != null)
		{
			// draw prompt on highlighted object
			Prompt prompt = this.highlightedObject.GetComponent<Prompt> ();
			if (prompt != null && prompt.GetPrompt().Trim() != "")
			{
				prompt.DrawPrompt();
			} else {
				// draw crosshair when the prompt is left blank
				this.drawCrosshair();
			}

			// draw potential stub on highlighted annotation object
			Annotation annotation = this.highlightedObject.GetComponent<Annotation> ();
			if (annotation != null && this.annotationsEnabled)
			{
				annotation.DrawSummary();
			}
		}
		else
		{
			// draw a crosshair when we have no highlighted object
			this.drawCrosshair();
		}
		
		// draw toolbar with our set of accessable area annotations
		this.drawToolbar(this.areaAnnotationsInRange);
	}

	private void drawCrosshair()
	{
		Rect frame = new Rect (Screen.width / 2, Screen.height / 2, 10, 10);
		GUI.Box (frame, "");
	}

	private void drawToolbar(List<Annotation> annotations)
	{
		if (annotations.Count != 0 && this.annotationsEnabled)
		{
			float xMargin = 10f;
			float yMargin = 10f;
			float rowSize = 35f;

			float toolbarWidth = Mathf.Min (0.25f * Screen.width, 500f);
			float toolbarHeight = (annotations.Count + 1) * rowSize;	// first row is a label

			// GUI coordinate system places 0,0 in top left corner
			float currentX = xMargin;
			float currentY = Screen.height - (yMargin + toolbarHeight);

			// Make a background box
			Rect toolbarFrame = new Rect (currentX, currentY, toolbarWidth, toolbarHeight);
			GUI.Box(toolbarFrame, "Area Annotations");
			
			// Shift down and indent 
			currentX += 10f;
			currentY += rowSize;

			// Make list of buttons, paired with annotation summaries
			int buttonIndex = 1;
			foreach (Annotation a in annotations)
			{
				float rowHeight = 0.6f*rowSize;
				Rect buttonFrame = new Rect (currentX, currentY, 20, 20);
				Rect labelFrame = new Rect (currentX + 30, currentY, toolbarWidth, rowHeight);

				GUI.Button (buttonFrame, buttonIndex.ToString());
				GUI.Label (labelFrame, a.summary);

				currentY += rowSize;
				buttonIndex++;
			}
		}
	}

	/// --- Trigger Areas ---
	public void OnTriggerEnter(Collider other)
	{
		GameObject inside = other.gameObject;
		// automatically trigger area we're now inside of's NON-ANNOTATION interactions
		List<Interaction> toTrigger = new List<Interaction>();
		foreach (Interaction i in inside.GetComponents<Interaction> ())
		{
			if (!(i is Annotation || i is Slideshow))
			{
				i.Interact (this.gameObject);
			}
		}
	}

	/// --- Handling Interaction ---

	private void AttemptInteract ()
	{
		if (highlightedObject == null) {
			return;
		}
		
		foreach (Interaction i in this.highlightedObject.GetComponents<Interaction> ())
		{
			if (i is Annotation || i is Slideshow)
			{
				// special cases, handled by `AttemptReadAnnotation`
				continue;
			}

			if (i.enabled)
			{ 
				i.Interact (this.gameObject);		
			} 
		}

        if (this.highlightedObject.GetComponent<Prompt>() != null)
        {
            this.highlightedObject.GetComponent<Prompt>().CyclePrompt();
        }
    }


	private void AttemptReadAnnotation ()
	{
		if (!this.annotationsEnabled || highlightedObject == null) {
			return;
		}

		foreach (Interaction i in this.highlightedObject.GetComponents<Interaction> ()) 
		{
			if (i.enabled && (i is Annotation || i is Slideshow))
			{
				i.Interact (this.gameObject);
			}
		}
	}

	private GameObject GetHighlightedObject()
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
				return null;
			}

			return hit.transform.gameObject;
		}
		else
		{
			return null;
		}
	}

	// --- Changing Player Abilities ---

	public void SetDrawsGUI(bool shouldDraw)
	{
		this.drawsGUI = shouldDraw;
	}

	public void SetCanMove(bool canMove)
	{
		var playerCompTypeA = this.gameObject.GetComponent<FirstPersonController> ();
		var playerCompTypeB = this.gameObject.GetComponent<RigidbodyFirstPersonController> ();

		if (playerCompTypeA != null)
		{
			playerCompTypeA.enabled = canMove;
		}
		if (playerCompTypeB != null)
		{
			playerCompTypeB.enabled = canMove;
		}
	}

	// --- Gizmos ---

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.gray;

		Vector3 origin = this.transform.position;
		Vector3 forward = this.transform.TransformDirection (Vector3.forward) * this.interactionRange;

		Gizmos.DrawRay(origin, forward);
	}

}