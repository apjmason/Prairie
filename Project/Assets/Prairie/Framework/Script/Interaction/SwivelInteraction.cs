using UnityEngine;
using System.Collections;

public class SwivelInteraction : Interaction {

	public GameObject hinge;
	private bool open = false;
	private float rotateTime;
	
	// Update is called once per frame
	protected void Update()
	{
		if (open)
		{
			transform.RotateAround(hinge.transform.position, new Vector3 (0,1,0), 200*Time.deltaTime);
			rotateTime += 1;
			if (rotateTime == 22) {
				rotateTime = 0;
				open = false;
			}
		}
	}

	protected override void PerformAction() {
		open = !open;
	}
}
