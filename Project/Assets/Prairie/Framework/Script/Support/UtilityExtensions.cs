using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public static class PlayerExtensions
{

	/// <summary>
	/// Sets the player state to be locked if true, free to move if untrue.
	/// </summary>
	/// <param name="isFrozen">If <c>true</c>, the player can not move.</param>
	public static void SetIsFrozen(this FirstPersonInteractor player, bool isFrozen)
	{
		var playerCompTypeA = player.GetComponent<FirstPersonController> ();

		var playerCompTypeB = player.GetComponent<RigidbodyFirstPersonController> ();

		if (playerCompTypeA != null)
		{
			playerCompTypeA.enabled = !isFrozen;
		}
		if (playerCompTypeB != null)
		{
			playerCompTypeB.enabled = !isFrozen;
		}

		player.enabled = !isFrozen;
	}

}

public static class InteractionExtensions {

	/// <summary>
	/// Sets the player who triggered this Interaction's state to be locked if true, free to move if untrue.
	/// </summary>
	/// <param name="isFrozen">If <c>true</c>, the player can not move.</param>
	public static void SetPlayerIsFrozen(this Interaction i, bool isFrozen)
	{
		if (i.rootInteractor != null)
		{
			FirstPersonInteractor player = i.rootInteractor.GetComponent<FirstPersonInteractor> ();
			if (player != null)
			{
				player.SetIsFrozen(isFrozen);
			}
		}
	}

}

public static class GameObjectExtensions {

	/// <summary>
	/// Interact with all Interactions attached to this GameObject.
	/// </summary>
	/// <param name="interactor">The invoker of the interaction, typically a player.</param>
	public static void InteractAll(this GameObject go, GameObject interactor)
	{
		foreach (Interaction i in go.GetComponents<Interaction> ())
		{
			i.Interact (interactor);
		}
	}

}

public static class TransformExtensions {

	/// <summary>
	/// Rotates this transform relative to the main camera.
	/// </summary>
	/// <param name="leftRightRotate">The speed at which to rotate against the horizontal axis.</param>
	/// <param name="upDownRotate">The speed at which to rotate against the vertical axis.</param>
	public static void RotateRelativeToCamera(this Transform t, float leftRightRotate, float upDownRotate)
	{
		// Code adapted from:
		// http://answers.unity3d.com/questions/299126/how-to-rotate-relative-to-camera-angleposition.html
		
		float sensitivity = .25f;
		//Get Main camera in Use.
		Camera cam = Camera.main;
		//Gets the world vector space for cameras up vector 
		Vector3 relativeUp = cam.transform.TransformDirection(Vector3.up);
		//Gets world vector for space cameras right vector
		Vector3 relativeRight = cam.transform.TransformDirection(Vector3.right);

		//Turns relativeUp vector from world to objects local space
		Vector3 objectRelativeUp = t.InverseTransformDirection(relativeUp);
		//Turns relativeRight vector from world to object local space
		Vector3 objectRelaviveRight = t.InverseTransformDirection(relativeRight);

		Quaternion rotateBy = Quaternion.AngleAxis(leftRightRotate / t.localScale.x * sensitivity, objectRelativeUp)
			* Quaternion.AngleAxis(-upDownRotate / t.localScale.x  * sensitivity, objectRelaviveRight);

		t.localRotation *= rotateBy;
	}

}
