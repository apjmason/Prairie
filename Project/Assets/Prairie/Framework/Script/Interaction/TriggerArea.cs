using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerArea : MonoBehaviour
{

    public GameObject[] additionalTargets;

	void OnTriggerEnter(Collider other)
	{

		// ensure we're being triggered by a player
		FirstPersonInteractor playerInteractor = other.gameObject.GetComponent<FirstPersonInteractor> ();
		if (playerInteractor == null)
		{
			return;
		}

		// Trigger all the Interactions on this object
		this.gameObject.InteractAll (playerInteractor.gameObject);

		// Trigger all the Interactions on our additional targets
		foreach (GameObject target in additionalTargets)
		{
			target.InteractAll (playerInteractor.gameObject);
		}
	}
    
}
