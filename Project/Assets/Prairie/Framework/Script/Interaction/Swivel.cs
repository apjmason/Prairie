using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Prairie/Interactions/Door Swivel")]
public class Swivel : PromptInteraction
{
	public bool openFromLeft = false;

	private Vector3 hinge;
	private Vector3 direction;
	private float rotateSpeed = 90.0f;
	private float targetAngle = 0;
	const float rotationAmount = 1.5f;
	private float localx;
	private float localy;
	private float localz;
	private bool closed = true;

	void Start()
	{
		hinge = this.transform.position;
		Vector3 amt;

		// assuming target is a cube transformed to look like a door
		// the largest side should be the height
		// the second largest side is width used to construct the pivot point
		List<float> dimensions = new List<float>();
		localx = this.transform.localScale.x;
		localy = this.transform.localScale.y;
		localz = this.transform.localScale.z;
		dimensions.Add(localx);
		dimensions.Add(localy);
		dimensions.Add(localz);
		dimensions.Sort();
		float width = dimensions[1];

		if (width == localx)
		{
			amt = this.transform.right;
		}
		else if (width == localy)
		{
			amt = -1 * this.transform.up;
		}
		else
		{
			amt = this.transform.forward;
		}
		amt *= 0.5f * width;

		// opening from left requires a pivot point opposite its counterpart
		// and a different direction
		if (openFromLeft)
		{
			hinge += amt;
			direction = Vector3.up;
		}
		else
		{
			hinge -= amt;
			direction = Vector3.down;
		}
	}

	protected override void PerformAction()
	{
		if (closed)
		{
			targetAngle -= rotateSpeed;
		}
		else
		{
			targetAngle += rotateSpeed;	
		}
		closed = !closed;
	}

	void Update()
	{
		if (targetAngle != 0)
		{
			Rotate();
		}
	}

	protected void Rotate()
	{
		if (targetAngle > 0)
		{
			transform.RotateAround (hinge, direction, -rotationAmount);
			targetAngle -= rotationAmount;
		}
		else if (targetAngle < 0)
		{
			transform.RotateAround (hinge, direction, rotationAmount);
			targetAngle += rotationAmount;
		}
	}

	override public string defaultPrompt {
		get {
			return "Open/Close Door";
		}
	}
}
