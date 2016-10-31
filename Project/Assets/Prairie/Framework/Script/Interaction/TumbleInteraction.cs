using UnityEngine;
using System.Collections;

public class TumbleInteraction : Interaction
{
	/// <summary>
	/// Allows user to rotate object.
	/// </summary>
	public bool pickedUp = false;

	// When the user interacts with object, they invoke the ability to 
	// tumble the object with the I, J, K and L keys. Interacting
	// with the object again revokes this ability.

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
			else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Escape))
			{
				this.SetPlayerIsFrozen(false);
				pickedUp = false;
			}
		}
	}

	protected override void PerformAction() {
		pickedUp = true;
		this.SetPlayerIsFrozen(true);
	}
}
