using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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
    public bool isStartNode;
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
    
    public int insertIndex = -1;
    private float vScrollBarValue;
    public Vector2 scrollPosition = new Vector2(0, 0);
    private string innerText;

    public static List<TwineNode> TwineNodeList = new List<TwineNode>();
    public static int visibleNodeIndex = 0;
    private static bool allMinimized = false;
    private static bool fanfold = true;
    public static string storyTitle = "";

    private void Awake()
    {
        if (isStartNode)
        {
            enabled = true;
        }
    }

    private void Start()
    {
        if (isStartNode)
        {
            FirstPersonInteractor interactor = (FirstPersonInteractor)FindObjectOfType(typeof(FirstPersonInteractor));
            GameObject interactorObject = interactor.gameObject;
            this._Activate(interactorObject);
        }
        StartCoroutine(Example());
    }

    /// <summary>
    /// Check if TwineNode has no children. If it
    ///     doesn't, wait 5 seconds and deactivate.
    /// </summary>
    IEnumerator Example()
    {
        if (this.children.Length == 0) {
            yield return new WaitForSeconds(5);
            this.Deactivate();
        }
    }

    void Update()
    {
        UpdateConditionalLinks();
        if (this.enabled)
        {
            if (Input.GetKeyDown(KeyCode.C) && TwineNodeList.IndexOf(this) == 0  && allMinimized == false)
            {
                fanfold = !fanfold;
            }
            if (Input.GetKeyDown (KeyCode.M) && TwineNodeList.IndexOf(this) == 0){
                allMinimized = !allMinimized;
            }
            if (!TwineNodeList.Contains(this))
            {
                TwineNodeList.Add(this);
            }
            if (Input.GetKeyDown(KeyCode.Tab) && TwineNodeList.IndexOf(this) == 0 && allMinimized == false && fanfold == false)
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
        else if ((fanfold) && (allMinimized == false)) {
                float frameWidth = Math.Min(Screen.width / 5, 200);
                float frameHeight = 80;
                if (TwineNodeList.Count() > 0){
                    frameHeight = Math.Min(Screen.height / (TwineNodeList.Count()), 80);
                }
                int index = TwineNodeList.IndexOf(this);
                Rect frame = new Rect (10, 10+index*80, frameWidth, frameHeight);
                GUIStyle style = new GUIStyle (GUI.skin.box);
                style.wordWrap = true;
                style.fixedWidth = frameWidth-10;
                GUI.BeginGroup(new Rect(20, 10+index*frameHeight, frameWidth + 10, frameHeight));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(frameWidth + 10), GUILayout.Height(frameHeight));
                content = content.TrimEnd();
                GUILayout.Label(new GUIContent(this.content), style, GUILayout.Width(frameWidth - 20), GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
                GUI.EndGroup ();

            }
            else if (this.enabled && !this.isMinimized && !allMinimized) {
                float frameWidth = Math.Min(Screen.width / 3, 350);
                float frameHeight = Math.Min(Screen.height / 2, 500);
                Rect frame = new Rect (10, 10, frameWidth, frameHeight);
                GUI.BeginGroup (frame);
                GUIStyle style = new GUIStyle (GUI.skin.box);
                style.wordWrap = true;
                style.fixedWidth = frameWidth;
                GUILayout.Box (this.content, style);
                GUI.EndGroup ();

            } else if (this.enabled && this.isMinimized) {

                // Draw minimized GUI instead
                Rect frame = new Rect (10, 10, 10, 10);

                GUI.Box (frame, "");

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
        if (!this.enabled && this.HasActiveParentNode())
        {
            foreach (GameObject parent in parents)
            {
                if (parent.GetComponent<TwineNode>().enabled)
                {
                    insertIndex = TwineNodeList.IndexOf(parent.GetComponent<TwineNode>());
                }
            }
            TwineNodeList.Insert(insertIndex, this);
            visibleNodeIndex = TwineNodeList.IndexOf(this);
            this._Activate(interactor);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Activate this TwineNode immediately, without any checks beforehand.
    /// </summary>
    /// <param name="interactor"></param>
    private void _Activate(GameObject interactor)
    {
        if (this.isDecisionNode)
        {
            FirstPersonInteractor player = (FirstPersonInteractor)FindObjectOfType(typeof(FirstPersonInteractor));
            player.setWorldActive("DialogueOpen");
        }
        this.enabled = true;
        this.isMinimized = false;
        this.RunVariableAssignments();
        this.UpdateConditionalLinks();
        this.DeactivateAllParents();
        this.StartInteractions(interactor);
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
        TwineNodeList.Remove(this);
    }

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
        foreach (GameObject parent in parents)
        {
            if(parent.GetComponent<TwineNode>().enabled)
            {
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
                    bool conditionMet = false;
                    switch (operation)
                    {
                        // For equals and not equals, we compare the values
                        // without caring if they can be converted to ints.
                        case "=":
                            conditionMet = globalVariables.GetValue(varName) == conditionalVals[condIndex];
                            break;
                        case "!=":
                            conditionMet = !(globalVariables.GetValue(varName) == conditionalVals[condIndex]);
                            break;
                        default:
                            // For greater-than/less-than-style comparisons, we
                            // break and leave conditionMet as false unless both
                            // values can be converted to ints.
                            int val1;
                            if (!Int32.TryParse(globalVariables.GetValue(varName), out val1)) break;
                            int val2;
                            if (!Int32.TryParse(conditionalVals[condIndex], out val2)) break;
                            switch (operation)
                            {
                                case "<":
                                    if (val1 < val2)
                                    {
                                        conditionMet = true;
                                    }
                                    break;
                                case "<=":
                                    if (val1 <= val2)
                                    {
                                        conditionMet = true;
                                    }
                                    break;
                                case ">":
                                    if (val1 > val2)
                                    {
                                        conditionMet = true;
                                    }
                                    break;
                                case ">=":
                                    if (val1 >= val2)
                                    {
                                        conditionMet = true;
                                    }
                                    break;
                                default:
                                    conditionMet = false;
                                    Exception e = new Exception("Twine conditional has unknown operation");
                                    throw e;
                            }
                            break;
                    }
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