using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TwineJsonParser
{

    public const string PRAIRIE_DECISION_TAG = "prairie_decision";
    public const string PRAIRIE_CONDITION_TAG = "prairie_condition";

    public static void ImportFromString(string jsonString, string prefabDestinationDirectory)
    {
        Debug.Log("Importing JSON...");

        ReadJson(jsonString, prefabDestinationDirectory);

        Debug.Log("Done!");
    }

    public static void ReadJson(string jsonString, string prefabDestinationDirectory)
    {
        // parse using `SimpleJSON`
        JSONNode parsedJson = JSON.Parse(jsonString);
        JSONArray parsedArray = parsedJson["passages"].AsArray;

        // parent game object which will be the story prefab
        string nameOfStory = parsedJson["name"];
        Debug.Log(nameOfStory);
        GameObject parent = new GameObject(nameOfStory);

        // Now, let's make GameObject nodes out of every twine/json node.
        //	Also, for easy access when setting up our parent-child relationships,
        //	we'll keep two dictionaries, linking names --> JSONNodes and names --> GameObjects
        Dictionary<string, JSONNode> twineNodesJsonByName = new Dictionary<string, JSONNode>();
        Dictionary<string, GameObject> twineGameObjectsByName = new Dictionary<string, GameObject>();

        string startNodePid = parsedJson["startnode"].ToString();
        startNodePid = startNodePid.Replace('"', ' ').Trim(); // remove the surrounding quotes (leftover from JSONNode toString() method)

        foreach (JSONNode storyNode in parsedArray)
        {
            GameObject twineNodeObject = MakeGameObjectFromStoryNode(storyNode);

            // Bind this node to the parent "Story" object
            twineNodeObject.transform.SetParent(parent.transform);

            // Store this node and its game object in our dictionaries:
            twineNodesJsonByName[twineNodeObject.name] = storyNode;
            twineGameObjectsByName[twineNodeObject.name] = twineNodeObject;

            TwineNode twineNode = twineNodeObject.GetComponent<TwineNode>();

            if (startNodePid.Equals(twineNode.pid))
            {
                // Enable/activate the start node in the story:
                twineNodeObject.GetComponent<TwineNode>().enabled = true;
            }
        }

        // link nodes to their children
        MatchChildren(twineNodesJsonByName, twineGameObjectsByName);

        // "If the directory already exists, this method does not create a new directory..."
        // From the C# docs
        System.IO.Directory.CreateDirectory(prefabDestinationDirectory);

        // save a prefab to disk, and then remove the GameObject from the scene
        string prefabDestination = prefabDestinationDirectory + "/" + parent.name + " - Twine.prefab";
        PrefabUtility.CreatePrefab(prefabDestination, parent);
        GameObject.DestroyImmediate(parent);
    }

    /// <summary>
    /// Turns a JSON-formatted Twine node into a GameObject with all the relevant data in a TwineNode component.
    /// </summary>
    /// <returns>GameObject of single node.</returns>
    /// <param name="nodeJSON">A Twine Node, in JSON format</param>
    public static GameObject MakeGameObjectFromStoryNode(JSONNode nodeJSON)
    {
#if UNITY_EDITOR

		GameObject nodeGameObject = new GameObject(nodeJSON["name"]);
		nodeGameObject.AddComponent<TwineNode> ();

		// Save additional Twine data on a Twine component
		TwineNode twineNode = nodeGameObject.GetComponent<TwineNode> ();
		twineNode.pid = nodeJSON["pid"];
		twineNode.name = nodeJSON["name"];

		twineNode.tags = GetDequotedStringArrayFromJsonArray(nodeJSON["tags"]);

        twineNode.content = GetVisibleText (nodeJSON["text"]);
        
        string[] variableExpressions = GetVariableExpressions(nodeJSON["text"]);
        ActivateVariableExpressions(variableExpressions, twineNode);

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

        Debug.Log("Going through variable expressions...");
        foreach (string expression in expressions)
        {
            Debug.Log("Analyzing var exp...");
            if (assignmentRegex.IsMatch(expression))
            {
                string variable = variableRegex.Match(expression).Value;
                string newValue = newValueRegex.Match(expression).Groups[1].Value;
                node.AddAssignment(variable, newValue);
                Debug.Log("Adding assignment...");
            }
            else if (ifRegex.IsMatch(expression))
            {
                string variable = variableRegex.Match(expression).Value;
                string matchValue = matchValueRegex.Match(expression).Groups[1].Value;
                string link = linkRegex.Match(expression).Groups[1].Value;
                node.AddConditional(variable, matchValue, link);
            }
            else
            {
                node.content += "\n Unknown variable expression!";
            }
        }
        return false;
    }

    public static void MatchChildren(Dictionary<string, JSONNode> twineNodesJsonByName, Dictionary<string, GameObject> gameObjectsByName)
    {
        foreach (KeyValuePair<string, GameObject> entry in gameObjectsByName)
        {
            string nodeName = entry.Key;
            GameObject nodeObject = entry.Value;

            TwineNode twineNode = nodeObject.GetComponent<TwineNode>();
            JSONNode jsonNode = twineNodesJsonByName[nodeName];

            // Iterate through the links and establish object relationships:
            JSONNode nodeLinks = jsonNode["links"];

            twineNode.children = new GameObject[nodeLinks.Count];
            twineNode.linkNames = new string[nodeLinks.Count];

            for (int i = 0; i < nodeLinks.Count; i++)
            {
                JSONNode link = nodeLinks[i];
                string linkName = link["name"];
                GameObject linkDestination = gameObjectsByName[link["link"]];

                // Remember parent:
                linkDestination.GetComponent<TwineNode>().parents.Add(nodeObject);

                // Set link as a child, and remember the name.
                twineNode.children[i] = linkDestination;
                twineNode.linkNames[i] = linkName;
            }
        }
    }

    /// <summary>
    /// Returns the text of a node without links or variable expressions
    /// </summary>
    /// <returns>The content without children atached.</returns>
    /// <param name="content">Content with children attached.</param>
    public static string GetVisibleText(string content)
    {
        Regex invisibleTextRegex = new Regex("(\\[\\[([^\\]]*)\\]\\])|(\\(\\([^)]*\\)\\))");
        return invisibleTextRegex.Replace(content, "");
    }

    static string[] GetDequotedStringArrayFromJsonArray(JSONNode jsonNode)
    {
        if (jsonNode == null)
        {
            return null;
        }

        string[] stringArray = new string[jsonNode.Count];
        for (int i = 0; i < jsonNode.Count; i++)
        {
            string quotedString = jsonNode[i].ToString();
            string dequotedString = quotedString.Replace('"', ' ').Trim();
            stringArray[i] = dequotedString;
        }

        return stringArray;
    }
}
