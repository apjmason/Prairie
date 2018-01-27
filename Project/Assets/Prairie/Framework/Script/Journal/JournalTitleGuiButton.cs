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
	}

	public void HandleClick() 
	{
		DisplayName (entry);
		DisplayInfo (entry);
//		DisplayImage (entry);
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
}
