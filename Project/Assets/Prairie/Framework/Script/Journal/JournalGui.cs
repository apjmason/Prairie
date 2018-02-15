using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class JournalGui : MonoBehaviour {
	private bool active = false;
	private bool isJournalOpen = false;
	private int startingEntry = 0;

	public Transform contentPanel;
	public GameObject prefab;
	public FirstPersonInteractor FPI;

	// Name of the journal UI object in game scene.
	private const string JOURNAL = "Journal";
	// Name of the entry panel UI object in game scene.
	private const string ENTRYPANEL = "EntryPanel";


	void Start() {
		gameObject.transform.Find(JOURNAL).gameObject.SetActive (active);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.J)) {
			if (!isJournalOpen) {
				isJournalOpen = true;
				openJournal ();
				FPI.setWorldActive ("Journal");
			} else {
				isJournalOpen = false;
				closeJournal ();
				FPI.setWorldActive ("Journal");
			}
		}
	}

	public void openJournal(){
		AddButtons ();
		gameObject.transform.Find(JOURNAL).gameObject.SetActive (true);
	}

	public void closeJournal(){
		gameObject.transform.Find(JOURNAL).gameObject.SetActive (false);
	}

	public bool isOpen() {
		return isJournalOpen;
	}

	private void AddButtons()
	{
		List<JournalEntry> entries = gameObject.GetComponentInParent<Journal> ().journal;
		for (int i = startingEntry; i < entries.Count; i++) 
		{
			JournalEntry e = entries[i];
			GameObject newButton = (GameObject)GameObject.Instantiate(prefab);

			newButton.transform.SetParent(contentPanel);

			// Scale the button.
			fitPrefabToParentSize(newButton);

			JournalTitleGuiButton jtgb = newButton.GetComponent<JournalTitleGuiButton>();
			jtgb.Setup(e);

			startingEntry += 1;
		}
	}

	private void fitPrefabToParentSize(GameObject button) {
		double actualWidth = gameObject.transform.Find(JOURNAL).gameObject.transform.Find(ENTRYPANEL).GetComponent<RectTransform> ().rect.width; // This is the actual width we want to prefab to be after scaling.
		float originalWidth = button.GetComponent<LayoutElement>().preferredWidth; // This is the preset preferred width of the prefab.

		// Constrain the prefab width to its parent panel.
		RectTransform rt = button.transform.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2((float)actualWidth, rt.rect.height);

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)originalWidth;
		button.transform.localScale = new Vector3 (scale, scale, scale);
	}
}
