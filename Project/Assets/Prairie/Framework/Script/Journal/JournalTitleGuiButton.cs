using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class JournalTitleGuiButton : MonoBehaviour
{
	public Button buttonComponent;
	public Text title;

	public GameObject infoTextPrefab;
	public GameObject infoImagePrefab;

	// Sync with the UI element names in game scene.
	private const string NAME = "ItemName";
	private const string INFO = "InfoPanel";
	private const string CONTENTTAG = "JournalInfoContent";

	private JournalEntry entry;
	private GameObject contentInfoPanel;

	// Use this for initialization
	void Start () 
	{
		buttonComponent.onClick.AddListener (HandleClick);
		contentInfoPanel = GameObject.FindGameObjectWithTag(CONTENTTAG);
	}

	public void Setup(JournalEntry currentEntry) 
	{
		entry = currentEntry;
		title.text = entry.title;
		fitTextToButton ();
	}

	public void HandleClick() 
	{
		RemoveAllContentsInfoPanel ();
		DisplayName (entry);
		DisplayInfo (entry);
	}

	private void fitTextToButton() {
		double textWidth = title.preferredWidth; // This is the width the text would LIKE to be.
		double buttonWidth = GetComponent<LayoutElement>().preferredWidth * 0.9; // This is the width that we want the text to be contained in.
																				// Anchor set to 0.05~0.95 for text
		double scale = buttonWidth / textWidth;

		// Construct new title for overflowed string
		if (scale < 1) {
			int sublength = (int)(title.text.Length * scale - 3);
			if (sublength < 0) {
				sublength = 0;
			}
			title.text = (title.text.Substring (0, sublength) + "...");
		}
	}

	// Display the title of the journal entry.
	private void DisplayName(JournalEntry e) {
		Text name = GameObject.Find(NAME).GetComponentInChildren<Text>();
		name.text = entry.title;
		name.color = Color.white;
	}

	// Display the detailed content of the journal entry.
	// <param name="e">Journal entry to display</param>
	private void DisplayInfo(JournalEntry e) {
		AnnotationContent content = e.content;
		List<Texture2D> images = e.images;

		for (int i = 0; i < Math.Max (images.Count, content.parsedText.Count); i++) {
			if (i < content.parsedText.Count && content.parsedText [i] != "") {
				AddText (content.parsedText [i]);
			}

			if (i < images.Count) {
				AddImage (images [i]);
			}
		}
	}

	// Add an image block in journal info panel UI.
	// Formats and displays a texture.
	// <param name="tex">Texture from journal entry to display</param>
	private void AddImage(Texture tex)
	{
		if (tex != null)
		{
			GameObject newImageBlock;

			// Instantiate a new image prefab.
			newImageBlock = (GameObject)GameObject.Instantiate (infoImagePrefab);
			newImageBlock.transform.SetParent (contentInfoPanel.transform);

			RawImage ri = newImageBlock.GetComponentInChildren<RawImage> ();
			ri.texture = tex;

			fitImageSizeToInfoPanel (newImageBlock, ri);
		}
	}

	// Add a text block in journal info panel UI.
	// Formats and displays a string.
	// <param name="parstedText">Parsed text string from journal entry to display</param>
	private void AddText(string parsedText) {
		GameObject newTextBlock;

		// Instantiate a new text prefab.
		newTextBlock = (GameObject)GameObject.Instantiate (infoTextPrefab);
		newTextBlock.transform.SetParent (contentInfoPanel.transform, false);

		// Scale the prefab according to resolution.
		fitTextPrefabWidthToInfoPanel(newTextBlock);

		// Update the text in the prefab.
		Text t = newTextBlock.GetComponentInChildren<Text> ();
		t.text = parsedText;
		t.resizeTextForBestFit = true;
		t.supportRichText = true;
	}

	// Remove all contents from info panel UI when another entry is selected.
	private void RemoveAllContentsInfoPanel() {
		for (int i = 0; i < contentInfoPanel.transform.childCount; i++) {
			Destroy (contentInfoPanel.transform.GetChild (i).gameObject);
		}
	}


	private void fitImageSizeToInfoPanel(GameObject entry, RawImage ri) {
		LayoutElement le = entry.GetComponent<LayoutElement> ();
		double actualWidth = GameObject.Find(INFO).GetComponent<RectTransform> ().rect.width * 0.9; // This is the actual width we want to prefab to be after scaling.
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

	private void fitTextPrefabWidthToInfoPanel(GameObject entry) {
		double actualWidth = GameObject.Find(INFO).GetComponent<RectTransform> ().rect.width * 0.9; // This is the actual width we want to prefab to be after scaling.
		float originalWidth = entry.GetComponent<LayoutElement>().preferredWidth; // This is the preset preferred width of the prefab.

		// Constrain the prefab width to its parent panel.
		RectTransform rt = entry.transform.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2((float)actualWidth, rt.rect.height);

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)originalWidth;

		entry.transform.localScale = new Vector3 (scale, scale, scale);
	}
}
