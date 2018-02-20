using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(TwineNode))]
public class TwineNodeEditor : Editor {


	TwineNode node;

	public void Awake()
	{
		this.node = (TwineNode) target;
	}

	public override void OnInspectorGUI ()
	{
		// Configuration:
		EditorGUILayout.LabelField ("Name", node.name);
		EditorGUILayout.LabelField ("Content");
		EditorGUI.indentLevel += 1;
		string _content = EditorGUILayout.TextArea (node.content);
		EditorGUI.indentLevel -= 1;

        bool _isDecisionNode = EditorGUILayout.Toggle("Decision node?", node.isDecisionNode);
        GameObject[] _objectsToTrigger = PrairieGUI.drawObjectList("Objects To Trigger", node.objectsToTrigger);
        GameObject[] _objectsToEnable = PrairieGUI.drawObjectList<GameObject>("Objects To Enable:", node.objectsToEnable);
        GameObject[] _objectsToRotate = PrairieGUI.drawObjectList<GameObject>("Objects To Rotate:", node.objectsToRotate);
        int _rotX = EditorGUILayout.IntField("X-axis rotation amount:", node.rotX);
        int _rotY = EditorGUILayout.IntField("Y-axis rotation amount:", node.rotY);
        int _rotZ = EditorGUILayout.IntField("Z-axis rotation amount:", node.rotZ);
        GameObject[] _objectsToTransform = PrairieGUI.drawObjectList<GameObject>("Objects To Transform:", node.objectsToTransform);
        int _trX = EditorGUILayout.IntField("X-axis transform amount:", node.trX);
        int _trY = EditorGUILayout.IntField("Y-axis transform amount:", node.trY);
        int _trZ = EditorGUILayout.IntField("Z-axis transform amount:", node.trZ);

        // Read-Only Display:
        PrairieGUI.drawObjectListReadOnly ("Children", node.children);
		PrairieGUI.drawObjectListReadOnly ("Parents", node.parents.ToArray ());

		// Save changes to the TwineNode if the user edits something in the GUI:
		if (GUI.changed) {
			Undo.RecordObject(node, "Modify Twine Node");
			node.isDecisionNode = _isDecisionNode;
			node.objectsToTrigger = _objectsToTrigger;
			node.content = _content;
		}
	}
}
