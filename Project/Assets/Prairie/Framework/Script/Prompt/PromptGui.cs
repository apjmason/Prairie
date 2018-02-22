using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PromptGui : MonoBehaviour
{
	// Name of the promptGui object in game scene.
	private const string PROMPT = "PromptPanel";

	private bool active = false;
	private GameObject promptGameObject;
	private string currentText;

	public FirstPersonInteractor FPI;

	// Use this for initialization
	void Start ()
	{
		promptGameObject = gameObject.transform.Find (PROMPT).gameObject;
		currentText = "";
		promptGameObject.SetActive (active);
	}

	public void ActivateGui(Prompt p) {
		Text t = promptGameObject.GetComponentInChildren<Text> ();
		active = DisplayPrompt(p,t);
		promptGameObject.SetActive (active);
	}

	public void DeactivateGui() {
		active = false;
		promptGameObject.SetActive (active);
	}

	public bool isUIActive() {
		return active;
	}

	public string GetCurrentText() {
		return currentText;
	}

	// Add the newest entry in the in-range area annotations to the UI.
	// <param name="a">Annotation to display</param>
	private bool DisplayPrompt(Prompt p, Text t) {
		string content = p.GetPrompt ();

		if (!string.IsNullOrEmpty (content.Trim ())) {
			t.text = content;
			currentText = content;
			return true;
		}
		return false;
	}
}
