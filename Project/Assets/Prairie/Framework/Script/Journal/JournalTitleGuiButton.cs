using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class JournalTitleGuiButton : MonoBehaviour
{
	public Button buttonComponent;
	public Text title;

	// Sync with the UI element names in game scene.
	private const string NAME = "ItemName";
	private const string INFO = "Info";
	private const string IMAGE = "ItemPicture";

	private JournalEntry entry;

	// Use this for initialization
	void Start () 
	{
		buttonComponent.onClick.AddListener (HandleClick);
	}

	public void Setup(JournalEntry currentEntry) 
	{
		entry = currentEntry;
		title.text = entry.title;
		fitTextToButton ();
	}

	public void HandleClick() 
	{
		DisplayName (entry);
		DisplayInfo (entry);
	}

	private void DisplayName(JournalEntry e) {
		Text name = GameObject.Find(NAME).GetComponentInChildren<Text>();
		name.text = entry.title;
		name.color = Color.white;
	}

	private void DisplayInfo(JournalEntry e) {
		Text info = GameObject.Find(INFO).GetComponentInChildren<Text>();
		info.text = "";
		foreach (string c in e.content) {
			info.text += c;
			info.text += '\n';
		}
		info.color = Color.white;
	}

	private void fitTextToButton() {
		double textWidth = title.preferredWidth; // This is the width the text would LIKE to be.
		double parentWidth = GetComponent<LayoutElement>().preferredWidth * 0.9; // This is the width that we want the text to be contained in.
																				// Anchor set to 0.05~0.95 for text
		double scale = parentWidth / textWidth;

		// Construct new title for overflowed string
		if (scale < 1) {
			int sublength = (int)(title.text.Length * scale - 3);
			if (sublength < 0) {
				sublength = 0;
			}
			title.text = (title.text.Substring (0, sublength) + "...");
		}
	}
}
