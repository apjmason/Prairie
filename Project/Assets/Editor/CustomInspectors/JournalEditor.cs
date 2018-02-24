using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Journal))]
public class JournalEditor : Editor
{
//	private bool[] showEntrySlots = new bool[Journal.NUMSLOTS];
	private SerializedProperty journalEntriesProperty;
	private const string JOURNAL = "journal";
	private const string TITLE = "title";
	private const string CONTENT = "content";
	private const string IMG = "imagePaths";

	private void OnEnable ()
	{
		journalEntriesProperty = serializedObject.FindProperty (JOURNAL);
	}
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		for (int i = 0; i < journalEntriesProperty.arraySize; i++)
		{
			EntrySlotGUI (i);
		}
		serializedObject.ApplyModifiedProperties ();
	}
	private void EntrySlotGUI (int index)
	{
		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;

		EditorGUILayout.Foldout (true, "Entry " + index);

		if (journalEntriesProperty == null) {
			Debug.Log (JOURNAL+" is null.");
		}

		SerializedProperty serialized_ic = journalEntriesProperty.GetArrayElementAtIndex(index);

		ShowRelativeProperty (serialized_ic, TITLE);
		ShowRelativeProperty (serialized_ic, CONTENT);
		ShowRelativeProperty (serialized_ic, IMG);

		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();
	}

	// Show child property of parent serializedProperty for a custom class
	private void ShowRelativeProperty(SerializedProperty serializedProperty, string propertyName)
	{
		// Need to create a serializedObject then find children propert.
		// Directly FindRelativeProperty from the parent serializedProperty returns null.
		SerializedObject propObj = new SerializedObject(serializedProperty.objectReferenceValue);
		SerializedProperty property = propObj.FindProperty(propertyName);

		if (property != null) {
			EditorGUI.indentLevel++;
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (property, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			EditorGUI.indentLevel--;
		} else {
			Debug.Log (propertyName+" property is null.");
		}
	}
}
