using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

[System.Serializable]
public class JournalEntry
{
	public string title;
	public List<string> content;
	public List<string> imagePaths;

	public JournalEntry(string t, List<string> c, List<string> imgP)
	{
		title = t;
		content = c;
		imagePaths = imgP;
	}
}
public class Journal : MonoBehaviour

{
	public const int NUMSLOTS = 20;
	public JournalEntry[] journal = new JournalEntry[NUMSLOTS];

	public bool AddToJournal (Annotation a)
	{
		AnnotationContent c = a.content;

		for (int i = 0; i < journal.Length; i++) {
			if (journal [i].title.Length == 0) {
				Debug.Log ("Add " + a.summary + "'s annotation info to journal slot " + i.ToString () + ".");
				journal [i] = new JournalEntry(a.summary, c.parsedText, c.imagePaths);
				return true;
			}
		}
		Debug.Log ("Cannot add " + a.summary + " to Journal. Journal full.");
		return false;
	}
}

