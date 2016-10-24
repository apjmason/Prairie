using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TwineJsonParser {

	// Keys for JSON:
	//private readonly static string NodeIdKey = "pid";
	//private readonly static string NodeNameKey = "name";

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
		GameObject[] twineNodes = new GameObject[parsedArray.Count];
		
		int count = 0;
		// create parent game object
		GameObject parent = new GameObject("Story");
		// make GameObject nodes out of every twine/json node
		foreach (JSONNode storyNode in parsedArray) {
			GameObject twineNode = MakeGameObjectFromStory (parent, storyNode);
			twineNodes [count] = twineNode;
			++count;
		}
		// make dictionary
		Dictionary<string, GameObject> objDict = new Dictionary<string, GameObject>();
		// add things to dictionary
		foreach (GameObject node in twineNodes) {
			objDict.Add (node.name, node);
		}
		FindChildren (twineNodes, objDict);
		// create the large prefab, and kill the GameObject that is not a prefab
		PrefabUtility.CreatePrefab ("Assets/Ignored/Story.prefab", parent);
		GameObject.DestroyImmediate (parent);
	}

	// Finds the children of each of the gameObjects from the other gameObjects
	// Iterates through the list of nodes, gets an array of children, and assigns
	// gameObject children to the node that match the names of the array of children
	public static void FindChildren (GameObject[] nodes, Dictionary<string, GameObject> objDict) {
		foreach (GameObject node in nodes) {
			string[] children = node.GetComponent<NodeInfo> ().childrenNames;
			node.GetComponent <NodeInfo> ().children = new GameObject[children.Length];
			int count = 0;
			foreach (string child in children) {
				node.GetComponent <NodeInfo> ().children[count] = objDict[child];
				++count;
			}
		}
	}

	// takes the twine/json node, and turns it into a game object with all the relevant data
	public static GameObject MakeGameObjectFromStory (GameObject parent, JSONNode storyNode) {
		#if UNITY_EDITOR

		GameObject tempObj = new GameObject(storyNode["name"]);
		tempObj.AddComponent<NodeInfo> ();
		var data = tempObj.GetComponent<NodeInfo> ();
		data.pid = storyNode["pid"];
		data.name = storyNode["name"];
		data.tags = Serialize (storyNode["tags"], false);
		data.content = StripChildren (storyNode["content"]);
		data.childrenNames = Serialize (storyNode["childrenNames"], true);
		tempObj.transform.SetParent (parent.transform);
		return tempObj;

		#endif
	}

	// takes a string list of strings from the json, and makes it an array of
	// strings, where each string is distinct from the other instead of all
	// one giant thing
	// boolean is because the children need more of their string sliced off
	// than the tags
	public static string[] Serialize (JSONNode node, bool parseChildren) {
		string[] nodeList = new string[node.Count];
		for (int i = 0; i < node.Count; i++) {
			string nodeString = node [i].ToString();
			if (parseChildren) {
				int stringLength = nodeString.Length - 4;
				nodeString = nodeString.Substring (2, stringLength);
			}
			nodeString = nodeString.Substring (1, nodeString.Length - 2);
			nodeList[i] = (nodeString);
		}
		return nodeList;
	}

	// strips the list of children off of the content,
	// because we really only want the content
	public static string StripChildren (string content) {
		string[] substrings = content.Split ('[');
		return substrings[0];
	}
}
