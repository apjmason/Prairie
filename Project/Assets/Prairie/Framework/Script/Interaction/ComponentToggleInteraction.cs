using UnityEngine;
using System.Collections;

[AddComponentMenu("Prairie/Interactions/Toggle Component")]
public class ComponentToggleInteraction : PromptInteraction 
{
	public Behaviour[] target = new Behaviour[0];
    public Behaviour[] editorTargetList = new Behaviour[0]; //used for the editor

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		for (int i = 0; i < target.Length; i++)
		{
			// Draw red line(s) between the object and the objects whose Behaviours it toggles
			Gizmos.DrawLine(transform.position, target[i].transform.position);
		}
	}

	protected override void PerformAction ()
	{
		for (int i = 0; i < target.Length; i++)
		{
			target[i].enabled = !target[i].enabled;
		}
	}

	override public string defaultPrompt {
		get {
			return "Toggle Something";
		}
	}
}
