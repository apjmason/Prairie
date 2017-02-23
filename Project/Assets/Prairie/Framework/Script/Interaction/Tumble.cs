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
	private Ray hit;

	// When the user interacts with object, they invoke the ability to 
	// tumble the object with the I, J, K and L keys. Interacting
	// with the object again revokes this ability.

	void Start()
	{
		pickedUp = false;
		oldRotation = transform.rotation;
		oldPosition = this.transform.position;
	}

	protected void Update()
	{
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

	override public string defaultPrompt {
		get {
			return "Pick Up Object";
		}
	}
}
