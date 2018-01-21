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
		AnnotationContent c = a.content;

		JournalEntry e = ScriptableObject.CreateInstance<JournalEntry> ();
		e.title = a.summary;
		e.content = c.parsedText;
		e.imagePaths = c.imagePaths;

		if (!journal.Contains (e)) {
			journal.Add (e);
			Debug.Log ("Add " + e.title + "'s annotation info to journal.");
		}
	}
}
