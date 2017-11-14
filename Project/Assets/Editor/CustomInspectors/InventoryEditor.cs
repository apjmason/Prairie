using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
	private bool[] showItemSlots = new bool[Inventory.numSlots];
	private SerializedProperty inventoryContentsProperty;
	private const string inventoryPropContentsName = "contents";
	private void OnEnable ()
	{
		inventoryContentsProperty = serializedObject.FindProperty (inventoryPropContentsName);
	}
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		for (int i = 0; i < Inventory.numSlots; i++)
		{
			ItemSlotGUI (i);
		}
		serializedObject.ApplyModifiedProperties ();
	}
	private void ItemSlotGUI (int index)
	{
		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;

		showItemSlots[index] = EditorGUILayout.Foldout (showItemSlots[index], "Item slot " + index);
		if (showItemSlots[index])
		{
			if (inventoryContentsProperty == null) {
				Debug.Log ("inventoryContentProperty is null");
			}
			SerializedProperty serialized_ic = inventoryContentsProperty.GetArrayElementAtIndex (index);
			ShowRelativeProperty (serialized_ic, "objName");
			ShowRelativeProperty (serialized_ic, "obj");
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