using UnityEngine;
using System.Collections;

// Interface for Various types of interactions, eg: Annotation, PromptInteraction, etc.
public abstract class Interaction : MonoBehaviour
{
	public bool repeatable = true;

	[HideInInspector]
    public GameObject rootInteractor;

	/// <summary>
	/// Trigger behavoir of this interaction.
	/// </summary>
	/// <param name="interactor">The game object which is the root invoker of this action. Typically a player character.</param>
	public void Interact (GameObject interactor)
	{
        this.rootInteractor = interactor;
		if (this.enabled) {
			PerformAction ();					// run the interaction
			if (!repeatable) {
				this.enabled = false;	// prevent future interactions
			}
		}
	}

	protected abstract void PerformAction ();
}
