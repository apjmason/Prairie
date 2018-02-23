using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class Journal : MonoBehaviour

{
	public List<JournalEntry> journal; 

	void Start() {
		journal = new List<JournalEntry>();
	}

	public void AddToJournal (Annotation a)
	{
		JournalEntry e = ScriptableObject.CreateInstance<JournalEntry> ();
		e.title = a.summary;
		e.content = a.content;
		e.images = a.images;

		if (!journal.Contains (e)) {
			journal.Add (e);
		}
	}
}
