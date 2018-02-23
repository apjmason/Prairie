using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JournalEntry : ScriptableObject
{
	public string title;
	public AnnotationContent content;
	public List<Texture2D> images;

	public override bool Equals(System.Object obj)
	{
		if (obj == null)
			return false;
		JournalEntry e = obj as JournalEntry ;
		if ((System.Object) e == null)
			return false;
		return title.Equals(e.title);
	}

	public bool Equals(JournalEntry e)
	{
		if ((object) e == null)
			return false;
		return title.Equals(e.title);
	}

	public override int GetHashCode()
	{
		return title.GetHashCode ();
	}
}

