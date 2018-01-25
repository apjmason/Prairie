using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TwineJsonParser {

	public const string PRAIRIE_DECISION_TAG = "prairie_decision";
	public const string PRAIRIE_CONDITION_TAG = "prairie_condition";

	public static void ImportFromString(string jsonString, string prefabDestinationDirectory)
	{
		Debug.Log ("Importing JSON...");

		ReadJson (jsonString, prefabDestinationDirectory);

		Debug.Log ("Done!");
	}
		
	public static void ReadJson (string jsonString, string prefabDestinationDirectory)
	{
		// parse using `SimpleJSON`
		JSONNode parsedJson = JSON.Parse(jsonString);
		JSONArray parsedArray = parsedJson["passages"].AsArray;

		// parent game object which will be the story prefab
		string nameOfStory = parsedJson["name"];
		Debug.Log (nameOfStory);
		GameObject parent = new GameObject(nameOfStory);

		// Now, let's make GameObject nodes out of every twine/json node.
		//	Also, for easy access when setting up our parent-child relationships,
		//	we'll keep two dictionaries, linking names --> JSONNodes and names --> GameObjects
		Dictionary<string, JSONNode> twineNodesJsonByName = new Dictionary<string, JSONNode>();
		Dictionary<string, GameObject> twineGameObjectsByName = new Dictionary<string, GameObject>();

		string startNodePid = parsedJson ["startnode"].ToString ();
		startNodePid = startNodePid.Replace ('"', ' ').Trim(); // remove the surrounding quotes (leftover from JSONNode toString() method)

		foreach (JSONNode storyNode in parsedArray)
		{
			GameObject twineNodeObject = MakeGameObjectFromStoryNode (storyNode);

			// Bind this node to the parent "Story" object
			twineNodeObject.transform.SetParent (parent.transform);

			// Store this node and its game object in our dictionaries:
			twineNodesJsonByName [twineNodeObject.name] = storyNode;
			twineGameObjectsByName [twineNodeObject.name] = twineNodeObject;

			TwineNode twineNode = twineNodeObject.GetComponent<TwineNode> ();

			if (startNodePid.Equals(twineNode.pid)) {
				// Enable/activate the start node in the story:
				twineNodeObject.GetComponent<TwineNode> ().enabled = true;
			}
		}

		// link nodes to their children
		MatchChildren (twineNodesJsonByName, twineGameObjectsByName);

		// "If the directory already exists, this method does not create a new directory..."
		// From the C# docs
		System.IO.Directory.CreateDirectory (prefabDestinationDirectory);

		// save a prefab to disk, and then remove the GameObject from the scene
		string prefabDestination = prefabDestinationDirectory + "/" + parent.name + " - Twine.prefab";
		PrefabUtility.CreatePrefab (prefabDestination, parent);
		GameObject.DestroyImmediate (parent);
	}

	/// <summary>
	/// Turns a JSON-formatted Twine node into a GameObject with all the relevant data in a TwineNode component.
	/// </summary>
	/// <returns>GameObject of single node.</returns>
	/// <param name="storyNode">A Twine Node, in JSON format</param>
	public static GameObject MakeGameObjectFromStoryNode (JSONNode storyNode)
	{
		#if UNITY_EDITOR

		GameObject nodeGameObject = new GameObject(storyNode["name"]);
		nodeGameObject.AddComponent<TwineNode> ();

		// Save additional Twine data on a Twine component
		TwineNode twineNode = nodeGameObject.GetComponent<TwineNode> ();
		twineNode.pid = storyNode["pid"];
		twineNode.name = storyNode["name"];

		twineNode.tags = GetDequotedStringArrayFromJsonArray(storyNode["tags"]);

		twineNode.content = RemoveTwineLinks (storyNode["text"]);

		// If Start Node, Parse the content and store variables
		if (twineNode.name.Equals("start")) {
			Debug.Log("ADD VAR TO START");
			string[] raw = twineNode.content.Split(new string[] { "\n" }, StringSplitOptions.None).Reverse().Skip(1).Reverse().ToArray();
			Debug.Log("ready to init");
			twineNode.iniVariables(raw);


			// foreach (string r in raw) {
			// 	string ar = r.Replace ("((", string.Empty).Replace ("))", string.Empty);
			// 	List<string> pair = ar.Split (":").ToList ();
			// 	twineNode.iniVariables (pair[0], pair[1]);
			// }
		}

		// Upon creation of this node, ensure that it is a decision node if it has
		//	the decision tag:
		// Vice versa for condition node
		twineNode.isDecisionNode = (twineNode.tags != null && twineNode.tags.Contains (PRAIRIE_DECISION_TAG));
		twineNode.isConditionNode = (twineNode.tags != null && twineNode.tags.Contains (PRAIRIE_CONDITION_TAG));


		// Start all twine nodes as deactivated at first:
		twineNode.Deactivate();

		return nodeGameObject;

		#endif
	}

	public static void MatchChildren(Dictionary<string, JSONNode> twineNodesJsonByName, Dictionary<string, GameObject> gameObjectsByName)
	{
		foreach(KeyValuePair<string, GameObject> entry in gameObjectsByName)
		{
			string nodeName = entry.Key;
			GameObject nodeObject = entry.Value;

			TwineNode twineNode = nodeObject.GetComponent<TwineNode> ();
			JSONNode jsonNode = twineNodesJsonByName [nodeName];
		
			// Iterate through the links and establish object relationships:
			JSONNode nodeLinks = jsonNode ["links"];

			twineNode.children = new GameObject[nodeLinks.Count];
			twineNode.childrenNames = new string[nodeLinks.Count];

			for (int i = 0; i < nodeLinks.Count; i++) {
				JSONNode link = nodeLinks [i];
				string linkName = link ["name"];
				GameObject linkDestination = gameObjectsByName [link ["link"]];

				// Remember parent:
				linkDestination.GetComponent<TwineNode> ().parents.Add (nodeObject);

				// Set link as a child, and remember the name.
				twineNode.children[i] = linkDestination;
				twineNode.childrenNames [i] = linkName;
			}
		}
	}
		
	/// <summary>
	/// Strips the list of children off the content,
	/// because we really only want the content.
	/// </summary>
	/// <returns>The content without children atached.</returns>
	/// <param name="content">Content with children attached.</param>
	public static string RemoveTwineLinks (string content)
	{
		string[] substrings = content.Split ('[');
		return substrings[0];
	}

	static string[] GetDequotedStringArrayFromJsonArray (JSONNode jsonNode)
	{
		if (jsonNode == null) {
			return null;
		}

		string[] stringArray = new string[jsonNode.Count];
		for (int i = 0; i < jsonNode.Count; i++)
		{
			string quotedString = jsonNode [i].ToString();
			string dequotedString = quotedString.Replace ('"', ' ').Trim ();
			stringArray [i] = dequotedString;
		}

		return stringArray;
	}
}
