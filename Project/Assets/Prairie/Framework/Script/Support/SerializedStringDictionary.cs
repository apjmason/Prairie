using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SerializedStringDictionary : System.Object
{
	
	[SerializeField]
	public List<string> keys;
	[SerializeField]
	public List<string> values;

	public SerializedStringDictionary()
	{
		this.keys = new List<string> ();
		this.values = new List<string> ();
	}

	public string ValueForKey(string key)
	{
		int index = this.keys.IndexOf (key);
		if (index == -1) {
			return null;
		}

		return this.values [index];
	}

	public void Set(string key, string value)
	{
		if (this.keys.Contains (key)) {
			// update it within collections
			int index = this.keys.IndexOf(key);
			this.values [index] = value;
		} else {
			// add it to collections
			this.keys.Add (key);
			this.values.Add (key);
		}
	}

}

