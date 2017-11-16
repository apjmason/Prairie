using UnityEngine;
using System.Collections;

[AddComponentMenu("Prairie/Interactions/Trigger Interaction")]
public class TriggerInteraction : PromptInteraction
{

	//I think that this script is responsible for triggering interaction scripts
	//that are attached to objects the player wants to interact with.
    public GameObject[] triggeredObjects = new GameObject[0];

	protected override void PerformAction()
    {
        foreach (GameObject target in triggeredObjects)
        {
            target.InteractAll(this.rootInteractor);
        }
    }

	//Overriding the default prompt
	override public string defaultPrompt {
		get {
			return "Trigger Something";
		}
	}

}
