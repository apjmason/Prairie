using UnityEngine;
using System.Collections;

public class TriggerInteraction : Interaction
{

    public GameObject[] triggeredObjects;

	protected override void PerformAction()
    {
        foreach (GameObject target in triggeredObjects)
        {
            target.InteractAll(this.rootInteractor);
        }
    }
}
