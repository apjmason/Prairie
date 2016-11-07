using UnityEngine;
using System.Collections;

public class SwivelInteraction : Interaction
{

	public GameObject hinge;
	private float rotateSpeed = 90.0f;
	private float targetAngle = 0;
	const float rotationAmount = 1.5f;

	protected override void PerformAction()
	{
		targetAngle -= rotateSpeed;
	}

	void Update()
	{
		// Trigger functions if Rotate is requested
		if (Input.GetKeyDown (KeyCode.G))
		{
			targetAngle += rotateSpeed;
		}
		if (targetAngle != 0)
		{
			Rotate();
		}
	}

	protected void Rotate()
	{
		if (targetAngle > 0)
		{
			transform.RotateAround (hinge.transform.position, Vector3.up, -rotationAmount);
			targetAngle -= rotationAmount;
		}
		else if (targetAngle < 0)
		{
			transform.RotateAround (hinge.transform.position, Vector3.up, rotationAmount);
			targetAngle += rotationAmount;
		}
	}
}
