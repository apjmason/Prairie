using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carry : PromptInteraction {

	private bool beingCarried;

	void Start(){
		beingCarried = false;

	}

	void Update(){
		if (beingCarried == true) {
			FirstPersonInteractor player = this.GetPlayer ();
			transform.position = player.transform.position + new Vector3(3/2, 1/2, 0);
			transform.position = player.transform.GetChild (0).transform.position + player.transform.GetChild (0).transform.forward * 2;
		} 

	}

	protected override void PerformAction() {
		if (beingCarried == false) {
			beingCarried = true;

		} else {
			beingCarried = false;
			if (GetComponent<Rigidbody>() != null) {
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


			}

		}


	}


}
