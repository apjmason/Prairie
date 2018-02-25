using UnityEngine;
using System.Collections;

[AddComponentMenu("Prairie/Interactions/Tumble")]
public class Tumble : PromptInteraction
{
	/// <summary>
	/// Allows user to rotate object.
	/// </summary>
	private bool pickedUp;
	private Quaternion oldRotation;
	private Vector3 oldPosition;
	public float distance = 1.5f;
	public float speed = 10;

	// When the user interacts with object, they invoke the ability to 
	// tumble the object with the I, J, K and L keys. Interacting
	// with the object again revokes this ability.

	//Function called when object is interacted with, used to initiate global variables
	void Start()
	{
		pickedUp = false;
		oldRotation = this.transform.rotation;
		oldPosition = this.transform.position;
	}

	//Function called every frame, updates position of object based on input and axis
	protected void Update()
	{
		//Only rotate if object is 'interacted' with
		if (pickedUp)
		{
			if (Input.GetKey (KeyCode.L)) // right
			{
				transform.RotateRelativeToCamera (-speed, 0);
			}
			else if (Input.GetKey (KeyCode.J)) // left
			{
				transform.RotateRelativeToCamera (speed, 0);
			}
			else if (Input.GetKey (KeyCode.K)) // down
			{
				transform.RotateRelativeToCamera (0, speed);
			}
			else if (Input.GetKey (KeyCode.I)) // up
			{
				transform.RotateRelativeToCamera (0, -speed);
			}
			else if (Input.GetKey (KeyCode.Escape))
			{
				this.PerformAction();
			}
		}
	}

	//Function that interacts with the object
	//If the player picks up the object, then the camera zooms in on the object,
	//Then removes movement and I think removes the UI
	//If the object is stopped being interacted with, it goes back to normal, and so does the player
	protected override void PerformAction() {
		pickedUp = !pickedUp;
		FirstPersonInteractor player = this.GetPlayer ();
		if (player != null) {
			if (pickedUp)
			{
				this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
				player.SetCanMove (false);
				player.SetDrawsGUI (false);
			}
			else
			{
				this.transform.rotation = oldRotation;
				this.transform.position = oldPosition;
				player.SetCanMove (true);
				player.SetDrawsGUI (true);
			}
		}
	}

	//Override the default prompt
	override public string defaultPrompt {
		get {
			return "Pick Up Object";
		}
	}
}
