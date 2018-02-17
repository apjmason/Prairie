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
	// <param name="a">Annotation to display</param>
	private void DisplayFullAnnotation(Annotation a) {
		AnnotationContent content = a.content;
		List<Texture2D> images = a.images;

		for (int i = 0; i < Math.Max (images.Count, content.parsedText.Count); i++) {
			if (i < content.parsedText.Count && content.parsedText [i] != "") {
				AddText (content.parsedText [i]);
			}

			if (i < images.Count) {
				AddImage (images [i]);
			}
		}
	}

	// Add an image block in full annotation panel UI.
	// Formats and displays a texture.
	// <param name="tex">Texture from annotation to display</param>
	private void AddImage(Texture tex)
	{
		if (tex != null)
		{
			GameObject newImageBlock;

			// Instantiate a new image prefab.
			newImageBlock = (GameObject)GameObject.Instantiate (imagePrefab);
			newImageBlock.transform.SetParent (contentPanel);

			RawImage ri = newImageBlock.GetComponentInChildren<RawImage> ();
			ri.texture = tex;

			fitImageSizeToParent (newImageBlock, ri);
		}
	}

	// Add a text block in full annotation panel UI.
	// Formats and displays a string.
	// <param name="parstedText">Parsed text string from annotation to display</param>
	private void AddText(string parsedText) {
		GameObject newTextBlock;

		// Instantiate a new text prefab.
		newTextBlock = (GameObject)GameObject.Instantiate (textPrefab);
		newTextBlock.transform.SetParent (contentPanel);

		// Scale the prefab according to resolution.
		fitFontSizeToParent (newTextBlock);

		// Update the text in the prefab.
		Text t = newTextBlock.GetComponentInChildren<Text> ();
		t.text = parsedText;
		t.supportRichText = true;
	}

	// Remove the content when full annotation is deactivated.
	private void RemoveAllContents() {
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

	private void fitImageSizeToParent(GameObject entry, RawImage ri) {
		LayoutElement le = entry.GetComponent<LayoutElement> ();
		float preferredWidth = le.preferredWidth; // This is the preset preferred width of the prefab.
		float imageWidth = ri.texture.width; // This is the actual width of the imported image.

		// Scale the content within the prefab.
		float scale = (float)imageWidth / (float)preferredWidth;

		if (imageWidth < preferredWidth) {
			le.preferredWidth = imageWidth;
			le.preferredHeight = ri.texture.height;

		} else {
			le.preferredHeight = ri.texture.height / scale;
		}
	}
}
