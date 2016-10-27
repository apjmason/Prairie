using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TwineJsonParser {

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
			GameObject twineNode = MakeGameObjectFromStoryNode (parent, storyNode);
			twineNodes [count] = twineNode;
			++count;
		}
		// make dictionary
		Dictionary<string, GameObject> objDict = new Dictionary<string, GameObject>();
		// add things to dictionary
		foreach (GameObject node in twineNodes) {
			objDict.Add (node.name, node);
		}
		MatchChildren (twineNodes, objDict);
		// create the large prefab, and kill the GameObject that is not a prefab
		PrefabUtility.CreatePrefab ("Assets/Ignored/Story.prefab", parent);
		GameObject.DestroyImmediate (parent);
	}
		
	/// <summary>
	/// Finds the children of each of the gameObjects from the other gameObjects
	/// Iterates through the list of nodes, gets an array of children, and assigns
	/// gameObject children to the node that match the names ofthe array of children
	/// </summary>
	/// <param name="nodes">Array of nodes</param>
	/// <param name="objDict">Dictionary of nodes, with key of node name</param>
	public static void MatchChildren (GameObject[] nodes, Dictionary<string, GameObject> objDict) {
		foreach (GameObject node in nodes) {
			string[] children = node.GetComponent<TwineNode> ().childrenNames;
			node.GetComponent <TwineNode> ().children = new GameObject[children.Length];
			int count = 0;
			foreach (string child in children) {
				node.GetComponent <TwineNode> ().children[count] = objDict[child];
				++count;
			}
		}
	}
		
	/// <summary>
	/// Takes the twine/JSON node, and turns it into a game object with all the relevant data
	/// </summary>
	/// <returns>GameObject of single node</returns>
	/// <param name="parent">Parent.</param>
	/// <param name="storyNode">JSON story node.</param>
	public static GameObject MakeGameObjectFromStoryNode (GameObject parent, JSONNode storyNode) {
		#if UNITY_EDITOR

		GameObject tempObj = new GameObject(storyNode["name"]);
		tempObj.AddComponent<TwineNode> ();
		var data = tempObj.GetComponent<TwineNode> ();
		data.pid = storyNode["pid"];
		data.name = storyNode["name"];
		data.tags = Serialize (storyNode["tags"], false);
		data.content = StripChildren (storyNode["content"]);
		data.childrenNames = Serialize (storyNode["childrenNames"], true);
		tempObj.transform.SetParent (parent.transform);
		return tempObj;

		#endif
	}
		
	/// <summary>
	/// Takes a string list of strings from the JSON, and makes it an array of
	/// strings, where each string is distinct from the other instead of one
	/// giant string
	/// Boolean allows children to have more sliced off of each string than tags
	/// The 4 within the if statement is to slice off the brackets around the children: [[children]]
	/// The 2 comes because we want to slice the brackets off of the tag: [tag]
	/// </summary>
	/// <param name="node">the JSONNode, which just holds the tag or childrenNames content</param>
	/// <param name="parseChildren">If set to <c>true</c>, slice more off the ends of child names</param>
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
		
	/// <summary>
	/// Strips the list of children off the content,
	/// because we really only want the content
	/// </summary>
	/// <returns>The content without children atached</returns>
	/// <param name="content">Content with children attached</param>
	public static string StripChildren (string content) {
		string[] substrings = content.Split ('[');
		return substrings[0];
	}
}
