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
	private int distance;
	private Ray hit;

	// When the user interacts with object, they invoke the ability to 
	// tumble the object with the I, J, K and L keys. Interacting
	// with the object again revokes this ability.

	void Start()
	{
		pickedUp = false;
		oldRotation = Quaternion.identity;
		oldPosition = this.transform.position;
		distance = 1;
	}

	protected void Update()
	{
		if (pickedUp)
		{
			if (Input.GetKey (KeyCode.L)) // right
			{
				transform.RotateRelativeToCamera (-10, 0);
			}
			else if (Input.GetKey (KeyCode.J)) // left
			{
				transform.RotateRelativeToCamera (10, 0);
			}
			else if (Input.GetKey (KeyCode.K)) // down
			{
				transform.RotateRelativeToCamera (0, 10);
			}
			else if (Input.GetKey (KeyCode.I)) // up
			{
				transform.RotateRelativeToCamera (0, -10);
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
				hit = new Ray(transform.position, Camera.main.transform.position - transform.position);
				this.transform.position = hit.GetPoint(distance);
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
