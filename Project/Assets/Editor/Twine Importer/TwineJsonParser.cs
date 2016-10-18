using UnityEngine;
using System.Collections;
using SimpleJSON;

public class TwineJsonParser {

	// Keys for JSON:
	private readonly static string NodeIdKey = "pid";
	private readonly static string NodeNameKey = "name";

	/// <summary>
	/// Imports the provided file in full to the current project
	/// </summary>
	/// <param name="file">The Twine JSON file to import</param>
	public static void ImportFile (TextAsset file) {
		Debug.Log ("Importing "+file.name+"...");

		ReadJson (file.text);

		Debug.Log ("Done!");
	}
		
	public static void ReadJson (string jsonString) {
		JSONNode parsedJson = JSON.Parse(jsonString);
		JSONArray parsedArray = parsedJson.AsArray;

		// Demo code, printing the ID and name of each node:
		foreach (JSONNode storyNode in parsedArray) {
			Debug.Log ("Node ID: " + storyNode[NodeIdKey]);
			Debug.Log ("Node name: " + storyNode[NodeNameKey]);
		}

	}


}
