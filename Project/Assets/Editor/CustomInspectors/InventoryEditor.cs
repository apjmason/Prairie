using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
	private bool[] showItemSlots = new bool[Inventory.NUMSLOTS];
	private SerializedProperty inventoryContentsProperty;

	// String value needs to be the same as the instance variable name in the Inventory Script.
	private const string INVENTORY_CONTENTS = "contents";
	private const string OBJECT_NAME = "objName";
	private const string OBJECT = "obj";

	private void OnEnable ()
	{
		inventoryContentsProperty = serializedObject.FindProperty (INVENTORY_CONTENTS);
	}
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		for (int i = 0; i < Inventory.NUMSLOTS; i++)
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
				Debug.Log (INVENTORY_CONTENTS + " is null");
			}
			SerializedProperty serialized_ic = inventoryContentsProperty.GetArrayElementAtIndex (index);
			ShowRelativeProperty (serialized_ic, OBJECT_NAME);
			ShowRelativeProperty (serialized_ic, OBJECT);
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