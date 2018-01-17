using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Journal))]
public class JournalEditor : Editor
{
	private bool[] showEntrySlots = new bool[Journal.NUMSLOTS];
	private SerializedProperty journalEntriesProperty;
	private const string journalPropContentName = "journal";
	private void OnEnable ()
	{
		journalEntriesProperty = serializedObject.FindProperty (journalPropContentName);
	}
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		for (int i = 0; i < Journal.NUMSLOTS; i++)
		{
			EntrySlotGUI (i);
		}
		serializedObject.ApplyModifiedProperties ();
	}
	private void EntrySlotGUI (int index)
	{
		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;

		showEntrySlots[index] = EditorGUILayout.Foldout (showEntrySlots[index], "Entry slot " + index);
		if (showEntrySlots[index])
		{
			if (journalEntriesProperty == null) {
				Debug.Log ("journalEntriesProperty is null");
			}
			SerializedProperty serialized_ic = journalEntriesProperty.GetArrayElementAtIndex (index);
			ShowRelativeProperty (serialized_ic, "title");
			ShowRelativeProperty (serialized_ic, "content");
			ShowRelativeProperty (serialized_ic, "imagePaths");
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();
	}

	// Show child property of parent serializedProperty for a custom class
	private void ShowRelativeProperty(SerializedProperty serializedProperty, string propertyName)
	{
		SerializedProperty property = serializedProperty.FindPropertyRelative(propertyName);
		if (property != null)
		{
			EditorGUI.indentLevel++;
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property, true);
			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
			EditorGUI.indentLevel--;
		}
	}
}