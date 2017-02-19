using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Prairie/Utility/Prompt")]
public class Prompt : MonoBehaviour 
{
    private static readonly int FIRST_PROMPT = 1;
    private static readonly int SECOND_PROMPT = 2;

    public bool isCyclic = false;

    public int curPrompt = FIRST_PROMPT;
    public string firstPrompt = "";
    public string secondPrompt = "";

    // if an AssociatedTwineNode interaction is present on the gameobject, then this
    // dictionary overrides all other prompt information
	public SerializedStringDictionary twinePrompts = new SerializedStringDictionary();
	public bool isTwinePrompt
    {
        get { return this.gameObject.GetComponent<AssociatedTwineNodes>() != null; }
    }

    public string GetPrompt()
    {
        if (this.isTwinePrompt)
        {
            TwineNode activeNode = this.GetActiveAssociatedTwineNode();
            if (activeNode == null)
            {
                // if there is no active node, return an empty (hidden) prompt
                return "";
            }
            // return the prompt associated with this node
			string twinePrompt = this.twinePrompts.ValueForKey(activeNode.name);
            if (twinePrompt == null) { return ""; }   // use empty prompt if not specified
            return twinePrompt;
        }

        // return single or cyclic prompt
        return curPrompt == FIRST_PROMPT ? this.firstPrompt : this.secondPrompt;
    }

	public void DrawPrompt()
	{
		// Draw a GUI with the interaction 
        if (!string.IsNullOrEmpty(this.GetPrompt().Trim()))
        {
            Rect frame = new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 4, Screen.height / 4);
            GUI.BeginGroup(frame);
            GUILayout.Box(this.GetPrompt());
            GUI.EndGroup();
        }
	}

    // ---- DEFAULT PROMPTS ----

	// `Reset()` is called when this component is being added to a
	// GameObject in the Inpsector for the first time, or if the user
	// explicitly clicks the "reset" button on the component.
	//
	// Here, we use it to set a sensible default for the component,
	// based on the presense of interaction components.
	public void Reset()
	{
		this.SetDefaultPrompt ();
	}

	public void SetDefaultPrompt()
	{
		string prompt = "";
		GameObject source = this.gameObject;
		foreach (PromptInteraction i in source.GetComponents<PromptInteraction> ())
		{
			if (i.defaultPrompt != null)
			{
				if (prompt != "") {
					prompt += ", ";		// seperate multiple actions
				}
				prompt += i.defaultPrompt;
			}
		}

        this.firstPrompt = prompt;
	}

    // ---- TWINE PROMPT ----

    // returns the twine node this prompt which is currently active
    public TwineNode GetActiveAssociatedTwineNode()
    {
        AssociatedTwineNodes nodes = this.gameObject.GetComponent<AssociatedTwineNodes>();
        if (nodes == null) { return null; }  // sanity check

        foreach (GameObject twineNodeObject in nodes.associatedTwineNodeObjects)
        {
            TwineNode twineNode = twineNodeObject.GetComponent<TwineNode> ();
			if (twineNode.HasActiveParentNode())
            {
				return twineNode;
            }
        }

        // no active twine node was found
        return null;
    }

    // ---- CYCLE PROMPT ----

    public void CyclePrompt()
    {
        
        if (isCyclic && !string.IsNullOrEmpty(this.secondPrompt.Trim()) && !string.IsNullOrEmpty(this.firstPrompt.Trim()))
        {
            if (curPrompt == FIRST_PROMPT)
            {
                curPrompt = SECOND_PROMPT;
            }
            else
            {
                curPrompt = FIRST_PROMPT;
            }
        }
    }

}

