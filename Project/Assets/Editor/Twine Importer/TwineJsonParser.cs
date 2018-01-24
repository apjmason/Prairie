using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TwineJsonParser {

	public const string PRAIRIE_DECISION_TAG = "prairie_decision";

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
	/// <param name="nodeJSON">A Twine Node, in JSON format</param>
	public static GameObject MakeGameObjectFromStoryNode (JSONNode nodeJSON)
	{
#if UNITY_EDITOR

		GameObject nodeGameObject = new GameObject(nodeJSON["name"]);
		nodeGameObject.AddComponent<TwineNode> ();

		// Save additional Twine data on a Twine component
		TwineNode twineNode = nodeGameObject.GetComponent<TwineNode> ();
		twineNode.pid = nodeJSON["pid"];
		twineNode.name = nodeJSON["name"];

		twineNode.tags = GetDequotedStringArrayFromJsonArray(nodeJSON["tags"]);

		twineNode.content = RemoveTwineLinks (nodeJSON["text"]);

        //TODO:  Come up with a better name, and make this actually do something useful
        string[] variableExpressions = GetVariableExpressions(nodeJSON["text"]);
        ActivateVariableExpressions(variableExpressions, twineNode);

		// Upon creation of this node, ensure that it is a decision node if it has
		//	the decision tag:
		twineNode.isDecisionNode = (twineNode.tags != null && twineNode.tags.Contains (PRAIRIE_DECISION_TAG));

		// Start all twine nodes as deactivated at first:
		twineNode.Deactivate();

		return nodeGameObject;

#endif
    }

    /// <summary>
    /// Parses the text of a twine node to find the variable expressions 
    /// (marked with double parentheses) within
    /// </summary>
    /// <param name="text">Node text with all links and variable expressions</param>
    /// <returns>Variable expressions minus parentheses</returns>
    public static string[] GetVariableExpressions(string text)
    {
        Regex expressionRegex = new Regex("\\(\\([^)]*\\)\\)");
        MatchCollection matches = expressionRegex.Matches(text);
        string[] strings = new string[matches.Count];
        int resultNum = 0;
        foreach (Match match in matches)
        {
            string result = match.Value;
            strings[resultNum] = result.Substring(2, result.Length - 4);
            resultNum++;
        }
        return strings;
    }

    //TODO: Finish
    /// <summary>
    /// Sets the 
    /// </summary>
    /// <param name="expressions">Node's variable expressions</param>
    /// <param name="node">Twine node</param>
    /// <returns>True, unless something goes wrong</returns>
    public static bool ActivateVariableExpressions(string[] expressions, TwineNode node)
    {
        Regex variableRegex = new Regex("\\$\\w*");
        Regex newValueRegex = new Regex(":\\s*(\\w*)");
        Regex matchValueRegex = new Regex("=\\s*(\\w*)");
        Regex assignmentRegex = new Regex("\\$\\w*:");
        Regex linkRegex = new Regex("\\[\\[([^\\]]*)\\]\\]");
        Regex ifRegex = new Regex("^\\s*if", RegexOptions.IgnoreCase);
        foreach (string expression in expressions)
        {
            node.content += "\n" + expression;
            if (assignmentRegex.IsMatch(expression))
            {
                node.content += "\n (assignment)";
                string variable = variableRegex.Match(expression).Value;
                string newValue = newValueRegex.Match(expression).Groups[1].Value;
                node.content += "\n Set '" + variable + "' to '" + newValue + "'";
                node.assignments[variable] = newValue;
            }
            else if (ifRegex.IsMatch(expression))
            {
                node.content += "\n (conditional)";
                string variable = variableRegex.Match(expression).Value;
                string matchValue = matchValueRegex.Match(expression).Groups[1].Value;
                string link = linkRegex.Match(expression).Groups[1].Value;
                node.content += "\n Proceed to '" + link + "' if '" + variable + "' = '" + matchValue + "'";
                string[] variableMatch = { variable, matchValue };
                node.conditionals[link] = variableMatch;
            }
            else
            {
                node.content += "\n Unknown!";
            }
        }
        return false;
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

    //TODO:  Update this.  At present, it just cuts everything off after the first single open bracket.  Bad.
    /// <summary>
    /// Returns the text of a node without links or variable expressions
    /// </summary>
    /// <returns>The content without children atached.</returns>
    /// <param name="content">Content with children attached.</param>
    public static string GetVisibleText (string content)
	{
        Regex invisibleTextRegex = new Regex("(\\[\\[([^\\]]*)\\]\\])|(\\(\\([^)]*\\)\\))");
        return invisibleTextRegex.Replace(content, "");
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
