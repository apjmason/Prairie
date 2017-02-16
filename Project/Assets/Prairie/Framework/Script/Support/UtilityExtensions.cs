using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public static class InteractionExtensions {

	public static FirstPersonInteractor GetPlayer(this Interaction i)
	{
		if (i.rootInteractor != null)
		{
			FirstPersonInteractor player = i.rootInteractor.GetComponent<FirstPersonInteractor> ();
			return player;
		}
		return null;
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
		
		float sensitivity = .5f;
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
