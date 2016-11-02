using UnityEngine;
using System.Collections;

public class SwivelInteraction : Interaction {

	public GameObject hinge;
	private bool open = false;
	private int direction = 1;
	private float rotateTime;
	
	// Update is called once per frame
	protected void Update()
	{
		if (open)
		{
			transform.RotateAround(hinge.transform.position, new Vector3 (0,direction,0), 200*Time.deltaTime);
			rotateTime += 1;
			if (rotateTime == 22) {
				rotateTime = 0;
				direction = -direction;
				open = false;
			}
		}
	}

	protected override void PerformAction() {
		open = true;
	}
}
