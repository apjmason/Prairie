using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class FullAnnotationGui : MonoBehaviour
{

	private bool active = false;

	public Transform contentPanel;
	public GameObject textPrefab;
	public GameObject imagePrefab;
	public FirstPersonInteractor FPI;

	// Name of the fullAnnotationGUI object in game scene.
	private const string FULLANNOTATION = "FullAnnotationPanel";

	void Start() {
		gameObject.transform.Find(FULLANNOTATION).gameObject.SetActive (active);
	}

	public void ActivateGui(Annotation a) {
		active = true;
		gameObject.transform.Find(FULLANNOTATION).gameObject.SetActive (active);
		DisplayFullAnnotation(a);
	}

	public void DeactivateGui() {
		active = false;
		RemoveAllContents ();
		gameObject.transform.Find(FULLANNOTATION).gameObject.SetActive (active);
	}

	public bool isUIActive() {
		return active;
	}

	// Add the newest entry in the in-range area annotations to the UI.
	public void DisplayFullAnnotation(Annotation a) {
		AnnotationContent content = a.content;
		List<Texture2D> images = a.images;

		for (int i = 0; i < Math.Max (images.Count, content.parsedText.Count); i++) {
			if (i < content.parsedText.Count && content.parsedText [i] != "") {
				AddText (content.parsedText [i]);
			}

//			if (i < images.Count) {
//				AddImage (images [i]);
//			}
		}
	}

	/// <summary>
	/// Formats and displays a texture
//	/// </summary>
//	/// <param name="tex">Texture to display</param>
//	void AddImage(Texture tex)
//	{
//		if (tex != null)
//		{
//			GUILayout.BeginHorizontal();
//			GUILayout.FlexibleSpace();
////			if (tex.width > BOX_WIDTH - 40)
////			{
////				//resize image if it is wider than the scrollbox
////				float newHeight = ((BOX_WIDTH - 40) / tex.width) * ((float)tex.height);
////				GUILayout.Label(new GUIContent(tex), GUILayout.Width(BOX_WIDTH - 40), GUILayout.Height(newHeight));
////			}
////			else
////			{
////				GUILayout.Label(new GUIContent(tex));
////			}
//			GUILayout.FlexibleSpace();
//			GUILayout.EndHorizontal();
//		}
//	}


	// Add the text block in full annotation panel UI.
	public void AddText(string parsedText) {
		GameObject newTextBlock;

		Debug.Log ("Add parsed text: " + parsedText);
		// Instantiate a new prefab if not enough entries have been created before.
		newTextBlock = (GameObject)GameObject.Instantiate (textPrefab);
		newTextBlock.transform.SetParent (contentPanel);

		// Scale the prefab according to resolution.
		fitFontSizeToParent (newTextBlock);

		// Update the text in the prefab.
		Text t = newTextBlock.GetComponentInChildren<Text> ();
		t.text = parsedText;
	}

	// Remove the content when full annotation is deactivated.
	public void RemoveAllContents() {
		for (int i = 0; i < contentPanel.childCount; i++) {
			Destroy (contentPanel.GetChild (i).gameObject);
		}
	}

	private void fitFontSizeToParent(GameObject entry) {
		double actualWidth = gameObject.transform.Find (FULLANNOTATION).GetComponent<RectTransform> ().rect.width; // This is the actual width we want to prefab to be after scaling.
		float originalWidth = entry.GetComponent<LayoutElement>().preferredWidth; // This is the preset preferred width of the prefab.

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)originalWidth;
		Text txt = entry.GetComponent<Text> ();
		txt.fontSize = (int)(txt.fontSize * scale);
	}
}
