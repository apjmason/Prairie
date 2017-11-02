using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
	private bool[] showItemSlots = new bool[Inventory.numSlots];
	private SerializedProperty objNameProperty;
	private SerializedProperty objsProperty;
	private const string inventoryPropobjNamesName = "objImages";
	private const string inventoryPropobjsName = "objs";
	private void OnEnable ()
	{
		objNameProperty = serializedObject.FindProperty (inventoryPropobjNamesName);
		objsProperty = serializedObject.FindProperty (inventoryPropobjsName);
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
			if (objNameProperty == null) {
				Debug.Log ("objNameProperty is null");
			}
			EditorGUILayout.PropertyField (objNameProperty.GetArrayElementAtIndex (index));
			EditorGUILayout.PropertyField (objsProperty.GetArrayElementAtIndex (index));
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();
	}
}