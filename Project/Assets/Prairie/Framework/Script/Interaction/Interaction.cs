using UnityEngine;
using System.Collections;

public abstract class Interaction : MonoBehaviour
{
	public string prompt;
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
