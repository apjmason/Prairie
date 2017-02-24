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
        ///run each of these as a separate loop, because we don't want the label to show up more than once for the first two, 
        ///but we want both to show if they both are triggered (for instance if the first object is empty, and the second refers to itself
        /// The last loop is also separate so that it can be run for every object that might not have an interaction on it
		foreach (GameObject triggerTarget in trigger.triggeredObjects)
		{
            if (triggerTarget == null)
            {
                PrairieGUI.warningLabel("You have one or more empty slots in your list of toggles.  Please fill these slots or remove them.");
                break;
            }
		}

        foreach (GameObject triggerTarget in trigger.triggeredObjects)
        {
            if (triggerTarget == this.trigger.gameObject)
            {
                // warn users about INFINITE triggering
                PrairieGUI.warningLabel("A trigger interaction should not trigger itself!");
                break;
            }
        }

        foreach (GameObject triggerTarget in trigger.triggeredObjects)
        {
            if (triggerTarget != null && triggerTarget.GetComponent<Interaction>() == null)
            {
                // no interaction attached to this trigger target!
                string targetName = triggerTarget.name;
                PrairieGUI.warningLabel("'" + targetName + "' has no interactions attached to it. Triggering it will do nothing.");
            }
        }

    }

}