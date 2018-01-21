using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JournalTitleGuiButton : MonoBehaviour
{

	public Button buttonComponent;
	public Text title;

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
	}

}
