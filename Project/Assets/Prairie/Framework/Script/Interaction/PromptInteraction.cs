using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Prompt))]
public abstract class PromptInteraction : Interaction
{
    /// <summary>
    /// Return a default prompt suitable for this interaction type, for pre-populating a Prompt object.
    /// Note that this value will only affect the editing user experience.
    /// Changing this during gameplay will have NO EFFECT on the prompt.
    /// </summary>
    /// <value>A sensible default, or null if no such sensible default is appropriate.</value>
    public virtual string defaultPrompt
    {
        get { return null; }
    }

    // Typically, when a component is added to the inspector for the first time,
    // it automatically appends a 'Prompt' component as well.
    //
    // Having this call here ensures that, no matter the order of instantiation,
    // any newly added Prompt component recieves a default value.
    public void Reset()
    {
        Prompt prompt = this.gameObject.GetComponent<Prompt>();
        if (prompt.firstPrompt == null || prompt.firstPrompt == "")
        {
            prompt.SetDefaultPrompt();
        }
    }

}
