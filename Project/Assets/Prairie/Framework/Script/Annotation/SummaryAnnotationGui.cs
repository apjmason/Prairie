using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SummaryAnnotationGui : MonoBehaviour
{

	private bool active = false;
	private const int DEFAULTFONTSIZE = 16;

	public FirstPersonInteractor FPI;

	// Name of the fullAnnotationGUI object in game scene.
	private const string SUMMARYANNOTATION = "SummaryAnnotationPanel";

	void Start() {
		gameObject.transform.Find(SUMMARYANNOTATION).gameObject.SetActive (active);
	}

	public void ActivateGui(Annotation a) {
		GameObject g = gameObject.transform.Find (SUMMARYANNOTATION).gameObject;
		Text t = g.GetComponentInChildren<Text> ();
		DisplaySummaryAnnotation(a,t);

		if (!isUIActive ()) {
			active = true;
			g.SetActive (active);
		}
	}

	public void DeactivateGui() {
		active = false;
		gameObject.transform.Find(SUMMARYANNOTATION).gameObject.SetActive (active);
	}

	public bool isUIActive() {
		return active;
	}

	// Add the newest entry in the in-range area annotations to the UI.
	// <param name="a">Annotation to display</param>
	private void DisplaySummaryAnnotation(Annotation a, Text t) {
		t.text = a.summary;
		fitFontSizeToParent (t);

		if (a.importType != (int)ImportTypes.NONE)
		{
			string clickForMore = "\n\n <size="+((int)(t.fontSize*0.8)).ToString()+"><i>Right click for more...</i></size>";
			t.text += clickForMore;
		}
	}

	private void fitFontSizeToParent(Text t) {
		double actualWidth = gameObject.transform.Find (SUMMARYANNOTATION).GetComponent<RectTransform> ().rect.width; // This is the actual width we want to prefab to be after scaling.
		float originalWidth = t.GetComponent<LayoutElement>().preferredWidth; // This is the preset preferred width of the prefab.

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)originalWidth;
		t.fontSize = (int)(DEFAULTFONTSIZE * scale);
	}
}
