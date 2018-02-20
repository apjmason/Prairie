using UnityEngine;
using System.Collections.Generic;
//using TwineVariables;
using System;
using System.Linq;

[AddComponentMenu("Prairie/Utility/Twine Node")]
public class TwineNode : MonoBehaviour
{

    public GameObject[] objectsToTrigger;
    public GameObject[] objectsToEnable;
 	public GameObject[] objectsToRotate;
 	public int rotX = 0;
 	public int rotY = 0;
 	public int rotZ = 0;
 	public GameObject[] objectsToTransform;
 	public int trX = 0;
 	public int trY = 0;
 	public int trZ = 0;

    [HideInInspector]
    public string pid;
    public new string name;
    [HideInInspector]
    public string[] tags;
    public string content;
    public string show = "";
    public GameObject[] children;
    [HideInInspector]
    public string[] linkNames;
    public GameObject[] validChildren;
    public string[] validLinkNames;
    public List<GameObject> parents = new List<GameObject>();
    public bool isDecisionNode;
    public bool isConditionNode;

    public List<string> assignmentVars;
    public List<string> assignmentVals;

    public List<string> conditionalVars;
    public List<string> conditionalVals;
    public List<string> conditionalLinks;
    public List<string> conditionalOp;

    public TwineVariables globalVariables;

    private bool isMinimized = false;
    private bool isOptionsGuiOpen = false;

    private int selectedOptionIndex = 0;

    public static List<TwineNode> TwineNodeList = new List<TwineNode>();
    public static int visibleNodeIndex = 0;
    public static int insertIndex = -1;
    private static bool fanfold = true;
    public static string storyTitle = "";


    void Update()
    {
        UpdateConditionalLinks();
        if (this.enabled)
        {
            if (Input.GetKeyDown(KeyCode.C) && TwineNodeList.IndexOf(this) == 0)
            {
                fanfold = !fanfold;
            }
            //            if (visibleNodeIndex == null) {
            //                visibleNodeIndex = 0;
            //            }
            //            print(visibleNodeIndex);
            //            print(TwineNodeList.IndexOf(this));
            if (!TwineNodeList.Contains(this))
            {
                TwineNodeList.Add(this);
                //                storyTitle = this.name;
                //                print(this.name);
                //                insertIndex = TwineNodeList.IndexOf(this);
                //                print("printing");
                //                print(TwineNodeList.Count);
                //                    foreach (TwineNode item in TwineNodeList) {
                //                        print(item.name);
                //                    }
                //                    if (TwineNodeList.Count > 1){
                //                        this.isMinimized = true;
                //                    }
            }
            if (Input.GetKeyDown(KeyCode.Tab) && TwineNodeList.IndexOf(this) == 0)
            {
                if (visibleNodeIndex == TwineNodeList.Count - 1)
                {
                    visibleNodeIndex = 0;
                }
                else
                {
                    visibleNodeIndex++;
                }
            }
            if (TwineNodeList.IndexOf(this) == visibleNodeIndex)
            {
                this.isMinimized = false;
            }
            else
            {
                this.isMinimized = true;
            }
            //            if (Input.GetKeyDown (KeyCode.Alpha0)) {
            //				visibleNodeIndex = 0;
            //			}
            //            if (Input.GetKeyDown (KeyCode.Alpha1)) {
            //				visibleNodeIndex = 1;
            //			}
            //			if (Input.GetKeyDown (KeyCode.Q) {
            //				this.isMinimized = ;
            //			}

            if (this.isDecisionNode)
            {
                this.isOptionsGuiOpen = true;
            }
            if (this.isConditionNode)
            {
                // get the $color value from global list
                // check the platform name by check $color:platform pair stored in condition node
                // check child node name to match platform name
                // activate childnode
                this.ActivateChildAtIndex(0);
                Debug.Log("This is a Condition Node: " + this.name);
            }
        }
    }

    public void OnGUI()
    {
        if (isDecisionNode)
        {
            int frameWidth = Math.Min(Screen.width / 3, 500);
            int frameHeight = Math.Min(Screen.height / 2, 350);
            int horizontalAlign = (Screen.width - frameWidth) / 2;
            int verticalAlign = Screen.height - frameHeight;

            Rect frame = new Rect(horizontalAlign, verticalAlign, frameWidth, frameHeight);

            GUI.BeginGroup(frame);
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.normal.textColor = Color.white;
            style.wordWrap = true;
            style.fixedWidth = frameWidth;
            GUILayout.Box(this.content, style);
            for (int index = 0; index < this.validLinkNames.Length; index++)
            {
                if (GUILayout.Button(this.validLinkNames[index]))
                {
                    this.ActivateChildAtIndex(index);
                }
            }

            GUI.EndGroup();
        }
        else if (fanfold) //Draw a bunch of boxes
        {
            float frameWidth = Math.Min(Screen.width / 3, 150);
            float frameHeight = Math.Min(Screen.height / 2, 500);
            int index = TwineNodeList.IndexOf(this);
            Rect frame = new Rect(10 + index * 150, 10, frameWidth, frameHeight);
            GUI.BeginGroup(frame);
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.wordWrap = true;
            style.fixedWidth = frameWidth;
            GUILayout.Box(this.content, style);

            GUI.EndGroup();

        }
        else if (this.enabled && !this.isMinimized) //Draw just this box
        {
            float frameWidth = Math.Min(Screen.width / 3, 350);
            float frameHeight = Math.Min(Screen.height / 2, 500);
            Rect frame = new Rect(10, 10, frameWidth, frameHeight);
            GUI.BeginGroup(frame);
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.wordWrap = true;
            style.fixedWidth = frameWidth;
            GUILayout.Box(this.content, style);

            GUI.EndGroup();

        }
        else if (this.enabled && this.isMinimized) // Don't draw any boxes, just a tiny icon
        {

            // Draw minimized GUI instead
            Rect frame = new Rect(10, 10, 10, 10);

            GUI.Box(frame, "");

        }

    }

    /// <summary>
    /// Trigger the interactions associated with this Twine Node.
    /// </summary>
    /// <param name="interactor"> The interactor acting on this Twine Node, typically a player. </param>
    public void StartInteractions(GameObject interactor)
    {
        if (this.enabled)
        {
            foreach (GameObject gameObject in objectsToTrigger)
            {
                gameObject.InteractAll(interactor);
            }
            foreach (GameObject gameObject in objectsToEnable)
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
            foreach (GameObject gameObject in objectsToRotate)
            {
                gameObject.transform.Rotate(rotX, rotY, rotZ);
            }
            foreach (GameObject gameObject in objectsToTransform)
            {
                gameObject.transform.Translate(trX, trY, trZ);
            }
        }
    }

    /// <summary>
    /// Activate this TwineNode (provided it isn't already
    /// 	active/enabled and it has some active parent)
    /// </summary>
    /// <param name="interactor">The interactor.</param>
    public bool Activate(GameObject interactor)
    {
        //        print(TwineNodeList);
        print("Activate called on " + name);
        print(this.enabled);
        if (!this.enabled && this.HasActiveParentNode())
        {
            print("Activating " + name);
            if (this.isDecisionNode)
            {
                FirstPersonInteractor player = (FirstPersonInteractor)FindObjectOfType(typeof(FirstPersonInteractor));
                player.setWorldActive("DialogueOpen");
            }
            this.enabled = true;
            this.isMinimized = false;
            this.RunVariableAssignments();
            this.UpdateConditionalLinks();
            //            this.isOptionsGuiOpen = false;

            this.DeactivateAllParents();
            TwineNodeList.Insert(insertIndex, this);
            //            TwineNodeList.Add(this);
            visibleNodeIndex = TwineNodeList.IndexOf(this);
            this.StartInteractions(interactor);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Find the FirstPersonInteractor in the world, and use it to activate
    /// 	the TwineNode's child at the given index.
    /// </summary>
    /// <param name="index">Index of the child to activate.</param>
    private void ActivateChildAtIndex(int index)
    {
        // Find the interactor:
        FirstPersonInteractor interactor = (FirstPersonInteractor)FindObjectOfType(typeof(FirstPersonInteractor));

        if (interactor != null)
        {
            GameObject interactorObject = interactor.gameObject;

            // Now activate the child using this interactor!
            TwineNode child = this.validChildren[index].GetComponent<TwineNode>();
            child.Activate(interactorObject);
        }
    }

    public void Deactivate()
    {
        if (this.isDecisionNode)
        {
            FirstPersonInteractor player = (FirstPersonInteractor)FindObjectOfType(typeof(FirstPersonInteractor));
            player.setWorldActive("DialogueClose");
        }
        this.enabled = false;
        insertIndex = TwineNodeList.IndexOf(this);
        TwineNodeList.Remove(this);
    }

    //    public void AddToList()
    //    {
    //        TwineNodeList.Add(this);
    //    }

    /// <summary>
    /// Check if this Twine Node has an active parent node.  Also only returns "True" if this node is a valid child of the parent.
    /// </summary>
    /// <returns><c>true</c>, if there is an active parent node, <c>false</c> otherwise.</returns>
    public bool HasActiveParentNode()
    {
        foreach (GameObject parent in parents)
        {
            if (parent.GetComponent<TwineNode>().enabled)
            {
                GameObject[] validSiblings = parent.GetComponent<TwineNode>().validChildren;
                foreach (GameObject sibling in validSiblings)
                {
                    if (name == sibling.name)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Deactivate all parents of this Twine Node.
    /// </summary>
    private void DeactivateAllParents()
    {
        print("Deactivating parents of " + name);
        foreach (GameObject parent in parents)
        {
            if(parent.GetComponent<TwineNode>().enabled)
            {
                print("Deactivating " + parent.GetComponent<TwineNode>().name);
                parent.GetComponent<TwineNode>().Deactivate();
            }
        }
    }

    /// <summary>
    /// Change any twine variable values as appropriate.  This includes 
    /// addition as well as assignment.
    /// </summary>
    private void RunVariableAssignments()
    {
        if (assignmentVars != null)
        {
            if (globalVariables == null)
            {
                globalVariables = TwineVariables.GetVariableObject();
            }
            // This character is a marker that says to use addition rather than
            // assignment.
            string plusSign = "+";

            for (int i = 0; i < assignmentVars.Count; i++)
            {
                string varName = assignmentVars[i];
                string varValue = assignmentVals[i];
                if (!varValue.Contains(plusSign))
                {
                    globalVariables.AssignValue(varName, varValue);
                }
                else
                {
                    // Remove the "+" marker and increment value
                    varValue = varValue.Substring(1);
                    globalVariables.IncrementValue(varName, varValue);
                }
            }
        }
    }

    /// <summary>
    /// Sets validChildren and validChildNames to include only the links that are allowed by the conditionals assigned to them.
    /// </summary>
    private void UpdateConditionalLinks()
    {
        if (conditionalVars.Count > 0)
        {
            if (globalVariables == null)
            {
                globalVariables = TwineVariables.GetVariableObject();
            }

            List<GameObject> checkedChildren = new List<GameObject>();
            List<string> checkedChildNames = new List<string>();
            for (int index = 0; index < linkNames.Length; index++)
            {
                if (!conditionalLinks.Contains(linkNames[index]))
                {
                    checkedChildNames.Add(linkNames[index]);
                    checkedChildren.Add(children[index]);
                }
                else
                {
                    string linkName = linkNames[index];
                    int condIndex = conditionalLinks.IndexOf(linkName);
                    string varName = conditionalVars[condIndex];
                    // Operation of the condition - e.g. "=", "!="
                    string operation = conditionalOp[condIndex];
                    // Truth value of the conditional
                    bool conditionMet;
                    if (operation == "=") 
                    {
                        conditionMet = globalVariables.GetValue(varName) == conditionalVals[condIndex];
                    }
                    else if (operation == "!=")
                    {
                        conditionMet = !(globalVariables.GetValue(varName) == conditionalVals[condIndex]);
                    }
                    else
                    {
                        conditionMet = false;
                        Exception e = new Exception("Twine conditional has unknown operation");
                        throw e;
                    }
                    //Debug.Log("Operation is " + operation);
                    //Debug.Log(globalVariables.GetValue(varName));
                    //Debug.Log(conditionalVals[condIndex]);
                    if (conditionMet)
                    {
                        checkedChildNames.Add(linkNames[index]);
                        checkedChildren.Add(children[index]);
                    }
                }
            }
            validLinkNames = checkedChildNames.ToArray();
            validChildren = checkedChildren.ToArray();
        } else
        {
            validLinkNames = linkNames;
            validChildren = children;
        }
    }

    public void AddAssignment(string var, string value)
    {
        if (assignmentVars == null)
        {
            assignmentVars = new List<string>();
            assignmentVals = new List<string>();
        }
        assignmentVars.Add(var);
        assignmentVals.Add(value);
    }

    public void AddConditional(string var, string value, string link, string match)
    {
        Debug.Log("Add conditional");
        if (conditionalVars == null)
        {
            conditionalVars = new List<string>();
            conditionalVals = new List<string>();
            conditionalLinks = new List<string>();
            conditionalOp = new List<string>();
        }
        conditionalVars.Add(var);
        conditionalVals.Add(value);
        conditionalLinks.Add(link);
        conditionalOp.Add(match);
    }
}
