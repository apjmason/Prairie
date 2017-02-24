using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(TwineNode))]
public class TwineNodeEditor : Editor {

	TwineNode node;
    GameObject[] editorTriggeredObjects = new GameObject[0];

    public override void OnInspectorGUI ()
	{
		node = (TwineNode)target;

        if (editorTriggeredObjects.Length == 0)
        {
            editorTriggeredObjects = node.objectsToTrigger;
        }

        node.isDecisionNode = EditorGUILayout.Toggle ("Decision node?", node.isDecisionNode);
        editorTriggeredObjects = PrairieGUI.drawObjectList ("Objects To Trigger", editorTriggeredObjects);

        node.objectsToTrigger = PrairieGUI.RemoveNulls(editorTriggeredObjects);

        EditorGUILayout.LabelField ("Name", node.name);
		EditorGUILayout.LabelField ("Content");
		EditorGUI.indentLevel += 1;
		node.content = EditorGUILayout.TextArea (node.content);
		EditorGUI.indentLevel -= 1;
		node.children = PrairieGUI.drawObjectListReadOnly ("Children", node.children);
		GameObject[] parentArray = node.parents.ToArray ();
		parentArray = PrairieGUI.drawObjectListReadOnly ("Parents", parentArray);
		node.parents = new List<GameObject> (parentArray);

		// Save changes to the TwineNode if the user edits something in the GUI:
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
