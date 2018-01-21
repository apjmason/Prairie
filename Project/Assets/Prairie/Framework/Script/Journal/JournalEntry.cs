using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Journal/JournalEntry", order = 1)]
public class JournalEntry : ScriptableObject
{
	public string title;
	public List<string> content;
	public List<string> imagePaths;
}

