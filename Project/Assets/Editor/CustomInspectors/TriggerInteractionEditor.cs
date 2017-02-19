using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriggerInteraction))]
public class TriggerInteractionEditor : Editor {

	TriggerInteraction trigger;
    GameObject[] editorTriggeredObjects = new GameObject[0];

	public override void OnInspectorGUI ()
	{
		this.trigger = (TriggerInteraction)target;

        if (editorTriggeredObjects.Length == 0)
        {
            editorTriggeredObjects = trigger.triggeredObjects;
        }

		// Principle Configuration:
		trigger.repeatable = EditorGUILayout.Toggle ("Repeatable?", trigger.repeatable);
		trigger.triggeredObjects = PrairieGUI.drawObjectList<GameObject> ("Trigger Objects:", trigger.triggeredObjects);

        trigger.triggeredObjects = PrairieGUI.RemoveNulls(editorTriggeredObjects);

        // Warnings:
        this.DrawWarnings();
		if (GUI.changed) {
			EditorUtility.SetDirty(trigger);
		}
	}

	public void DrawWarnings()
	{
		foreach (GameObject triggerTarget in trigger.triggeredObjects)
		{
			if (triggerTarget.GetComponent<Interaction>() == null)
			{
				// no interaction attached to this trigger target!
				string targetName = triggerTarget.name;
				PrairieGUI.warningLabel("'" + targetName + "' has no interactions attached to it. Triggering it will do nothing.");
			}

			if (triggerTarget == this.trigger.gameObject)
			{
				// warn users about INFINITE triggering
				PrairieGUI.warningLabel("A trigger interaction should not trigger itself!");
			}
		}
	}

}