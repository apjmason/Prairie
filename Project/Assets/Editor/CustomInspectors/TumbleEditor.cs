using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tumble))]
public class TumbleEditor : Editor {

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField ("Use I, J, K and L keys to rotate object, and Escape to put it down.");
	}
}
