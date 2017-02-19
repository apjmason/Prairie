using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ComponentToggleInteraction))]
public class ComponentToggleInteractionEditor : Editor {

	ComponentToggleInteraction componentToggle;
    Behaviour[] editorTargetList = new Behaviour[0];

	public override void OnInspectorGUI ()
	{
        this.componentToggle = (ComponentToggleInteraction)target;

        if (editorTargetList.Length == 0)
        {
            editorTargetList = componentToggle.target;
        }

		componentToggle.repeatable = EditorGUILayout.Toggle ("Repeatable?", componentToggle.repeatable);
		editorTargetList = PrairieGUI.drawObjectList<Behaviour> ("Behaviours To Toggle:", editorTargetList);

        componentToggle.target = PrairieGUI.RemoveNulls(editorTargetList);
		this.DrawWarnings();

        if (GUI.changed) {
			EditorUtility.SetDirty(componentToggle);
		}
	}

    public void DrawWarnings()
	{
		foreach (Behaviour behaviour in componentToggle.target) 
		{
			if (behaviour == null) 
			{
				PrairieGUI.warningLabel ("You have one or more empty slots in your list of toggles.  Please fill these slots or remove them.");
				break;
			}
		}
	}
}
