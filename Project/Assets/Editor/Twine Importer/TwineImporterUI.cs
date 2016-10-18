using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// Defines the Import Twine window and a few contextual menu actions to trigger import.
/// </summary>
public class TwineImporterUI : EditorWindow {

	public TextAsset targetFile;

	/// <summary>
	/// Defines the "Import Twine Data" menu item and its action.
	/// If triggered from a context menu on an HTML asset, the asset is automatically selected for import.
	/// </summary>
	[MenuItem("Assets/Import Twine Data...")]
	static void ShowTwineImportWindow () {

		// if triggered while a text asset is selected, populate it as the target file
		TextAsset selectedFile = null;
		if (Selection.activeObject != null) {
			var filePath = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (Path.GetExtension (filePath).Contains ("json")) {
				selectedFile = (TextAsset) Selection.activeObject;
			}
		}

		// create and show window
		var window = EditorWindow.GetWindow<TwineImporterUI> ();
		window.targetFile = selectedFile;
	}

	/// <summary>
	/// Draws the GUI of the import window
	/// </summary>
	void OnGUI () {
		GUILayout.Label ("Import Twine Data", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Twine File:");

		// button which selects a target file
		var prompt = "Select File...";
		if (targetFile != null) {
			prompt = targetFile.name;
		}
		if (GUILayout.Button (prompt)) {
			var fullPath = EditorUtility.OpenFilePanel ("Select File", "Assets", "json");

			// obnoxiously, the OpenFilePanel returns a full file path,
			// and Unity will only play nice with a relative one so we must convert
			var projectDirectory = Directory.GetParent (Application.dataPath).ToString ();
			var relativePath = GetRelativePath (fullPath, projectDirectory);

			// double check we'll have access to this file
			if (relativePath.StartsWith ("Assets/") || relativePath.StartsWith ("Assets\\")) {	// checks both Mac Path and PC Path types
				this.targetFile = AssetDatabase.LoadAssetAtPath<TextAsset> (relativePath);
			} else {
				EditorUtility.DisplayDialog ("Can't Load Asset", "The file must be stored as part of your Unity project's assets.", "OK");
			}
		}

		GUILayout.EndHorizontal ();

		GUILayout.FlexibleSpace ();

		// button to send to importer
		GUI.enabled = (this.targetFile != null);
		if (GUILayout.Button ("Import")) {
			SendToImporter (this.targetFile);
			this.Close ();
		}
	}

	/// <summary>
	/// Converts a absolute path to a relative path from some other folder
	/// </summary>
	/// <returns>A relative path with the `folder` as the base</returns>
	/// <param name="filespec">The full path to a file inside of `folder`</param>
	/// <param name="folder">The folder to act as the root for the new path</param>
	private string GetRelativePath(string filespec, string folder) {
		Uri pathUri = new Uri(filespec);
		// Folders must end in a slash
		if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			folder += Path.DirectorySeparatorChar;
		}
		Uri folderUri = new Uri(folder);
		return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
	}

	/// <summary>
	/// Sends an html file to the Twine importer for input
	/// </summary>
	/// <param name="filePath">The file path of the Twine html to be imported</param>
	void SendToImporter (TextAsset file) {
		TwineJsonParser.ImportFile (file);
	}

}
