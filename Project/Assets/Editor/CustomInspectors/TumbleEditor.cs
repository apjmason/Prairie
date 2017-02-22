using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tumble))]
public class TumbleEditor : Editor {

	Tumble tumble;

	public override void OnInspectorGUI()
	{
		tumble = (Tumble)target;
		EditorGUILayout.LabelField ("Use I, J, K and L keys to rotate object, and Escape to put it down.");
		EditorGUILayout.LabelField ("Distance from player:");
		tumble.distance = EditorGUILayout.Slider(tumble.distance, 0, 10);
		EditorGUILayout.LabelField ("Rotation speed:");
		tumble.speed = EditorGUILayout.Slider(tumble.speed, 1, 20);
		if (GUI.changed) {
			EditorUtility.SetDirty(tumble);
		}
	}
}
