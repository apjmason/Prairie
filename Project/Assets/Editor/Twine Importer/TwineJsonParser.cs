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

        // remove the surrounding quotes (leftover from JSONNode toString() method)
        startNodePid = startNodePid.Replace('"', ' ').Trim(); 

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
                // Tell the start node that it's a start node
                twineNodeObject.GetComponent<TwineNode>().isStartNode = true;
            }
        }

        // link nodes to their children
        MatchChildren(twineNodesJsonByName, twineGameObjectsByName);

        // "If the directory already exists, this method does not create a new directory..."
        // - From the C# docs
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
        // Matches an alphanumeric string preceded by a dollar sign
        // E.g. 
        //   "$var"
        //   "$1Apple3" 
        //   not "$.var"
        //   not "$app le"
        Regex variableRegex = new Regex("\\$\\w*");

        // Finds an instance of a variable being assigned, starting with the 
        // ":" and including the assigned value in a sub-group.
        // E.g. 
        //   ":red" and "red"
        //   ":      -3" and "-3"
        Regex assignmentRegex = new Regex(":\\s*(-?\\w*)");

        // Looks for addition and retrieves the variable, the operator, and 
        // the value.
        // E.g.
        //   "$var + 1" and "var", "+", "1"
        //   "$var+-1" and "var", "+", "-1"
        //   "$var -apple" and "var", "-", "apple"
        Regex additionRegex = new Regex("\\$(\\w*)\\s*([+-])\\s*(-?\\w*)");

        // Like the previous, but detects a value being compared to a value 
        // with an equals sign rather than assignments.  Also gets comparison
        // type.
        // E.g.
        //   "= red" and "=", "red"
        //   "!= 3" and "!=", "3"
        Regex matchValueRegex = new Regex("(!?=|<=?|>=?)\\s*(-?\\w*)");

        // Finds a twine link - i.e. a double-bracketed line of text - with the
        // link content in a sub-group
        // E.g. 
        //   "[[Next Node]]" and "Next Node"
        Regex linkRegex = new Regex("\\[\\[([^\\]]*)\\]\\]");

        // Matches all variable lines that start with "if", ignoring case
        Regex ifRegex = new Regex("^\\s*if", RegexOptions.IgnoreCase);

        Debug.Log("Going through variable expressions...");
        foreach (string expression in expressions)
        {
            Debug.Log("Analyzing var expression...");
            if (assignmentRegex.IsMatch(expression))
            {
                string variable = variableRegex.Match(expression).Value;
                string value = assignmentRegex.Match(expression).Groups[1].Value;
                node.AddAssignment(variable, value);
                Debug.Log("Adding assignment...");
                Debug.Log("Assignment value = " + value);
            }
            else if (additionRegex.IsMatch(expression))
            {
                GroupCollection matches = additionRegex.Match(expression).Groups;
                string variable = matches[1].Value;
                string operation = matches[2].Value;
                string value = matches[3].Value;
                // If we're subtracting, flip the sign of the value
                if (operation[0] == '-')
                {
                    if (value[0] != '-')
                    {
                        value = "-" + value;
                    }
                    else
                    {
                        value = value.Substring(1);
                    }
                }
                // Now add a "+" to the value to mark this as addition, not 
                // assignment
                value = "+" + value;
                node.AddAssignment(variable, value);
                Debug.Log("Adding addition...");
                Debug.Log("Addition value = " + value);
            }
            else if (ifRegex.IsMatch(expression))
            {
                // This condition covers both types of "if" statement.  
                // We should probably later extend it to cover <, >, <=, and >=,
                // if we decide to implement them.
                Debug.Log("Adding conditional...");
                string variable = variableRegex.Match(expression).Value;
                string operation = matchValueRegex.Match(expression).Groups[1].Value;
                Debug.Log("Operation: " + operation);
                string matchValue = matchValueRegex.Match(expression).Groups[2].Value;
                string link = linkRegex.Match(expression).Groups[1].Value;
                node.AddConditional(variable, matchValue, link, operation);
            }
            else
            {
                Debug.Log("Unknown variable expression!");
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
        // Pattern for text surrounded by double parentheses or double brackets
        //  e.g. "[[text blah blay asdlh]]" or "((asddflhjwherkkh}}"
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
