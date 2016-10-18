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
	protected override void PerformAction() {
		if (!pickedUp)
		{
			pickedUp = true;
		} else
		{
			pickedUp = false;
		}
	}
		 
	protected void Update()
	{
		if (pickedUp)
		{
			float rotateSpeed = 2f;

			// FirstPersonInteractor.avaliableInteractions [0].prompt = "Put Down";

			if (Input.GetKey (KeyCode.L))
			{
				transform.Rotate(Vector3.down * rotateSpeed, Space.World);
			}
			else if (Input.GetKey (KeyCode.J))
			{
				transform.Rotate(Vector3.up * rotateSpeed, Space.World);
			}
			else if (Input.GetKey (KeyCode.K))
			{
				transform.Rotate(Vector3.left * rotateSpeed, Space.World);
			}
			else if (Input.GetKey (KeyCode.I))
			{
				transform.Rotate(Vector3.right * rotateSpeed, Space.World);
			}
		}
	}
}
