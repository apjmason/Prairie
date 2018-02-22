using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class FullAnnotationGui : MonoBehaviour
{

	private bool active = false;
	private GameObject fullAnnotationGameObject;

	public Transform contentPanel;
	public GameObject textPrefab;
	public GameObject imagePrefab;
	public FirstPersonInteractor FPI;

	// Name of the fullAnnotationGUI object in game scene.
	private const string FULLANNOTATION = "FullAnnotationPanel";

	void Start() {
		fullAnnotationGameObject = gameObject.transform.Find (FULLANNOTATION).gameObject;
		fullAnnotationGameObject.SetActive (active);
	}

	public void ActivateGui(Annotation a) {
		active = true;
		fullAnnotationGameObject.SetActive (active);
		DisplayFullAnnotation(a);
	}

	public void DeactivateGui() {
		active = false;
		RemoveAllContents ();
		fullAnnotationGameObject.SetActive (active);
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
		newTextBlock.transform.SetParent (contentPanel, false);

		// Scale the prefab according to resolution.
		fitPrefabWidthToParent(newTextBlock);

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

	private void fitImageSizeToParent(GameObject entry, RawImage ri) {
		LayoutElement le = entry.GetComponent<LayoutElement> ();
		double actualWidth = gameObject.transform.Find (FULLANNOTATION).GetComponent<RectTransform> ().rect.width * 0.9; // This is the actual width we want to prefab to be after scaling.
		float imageWidth = ri.texture.width; // This is the actual width of the imported image.

		le.preferredWidth = imageWidth;
		le.preferredHeight = ri.texture.height;

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)imageWidth;
		entry.transform.localScale = new Vector3 (1, 1, 1);

		if (scale < 1) {
			le.preferredWidth = (int)(le.preferredWidth * scale);
			le.preferredHeight = (int)(le.preferredHeight * scale);
		}
	}

	private void fitPrefabWidthToParent(GameObject entry) {
		double actualWidth = gameObject.transform.Find (FULLANNOTATION).GetComponent<RectTransform> ().rect.width * 0.9; // This is the actual width we want to prefab to be after scaling.
		float originalWidth = entry.GetComponent<LayoutElement>().preferredWidth; // This is the preset preferred width of the prefab.

		// Constrain the prefab width to its parent panel.
		RectTransform rt = entry.transform.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2((float)actualWidth, rt.rect.height);

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)originalWidth;

		entry.transform.localScale = new Vector3 (scale, scale, scale);
	}

}
