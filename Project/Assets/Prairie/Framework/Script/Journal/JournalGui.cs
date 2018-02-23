using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JournalGui : MonoBehaviour {
	private bool active = false;
	private bool isJournalOpen = false;
	private int startingEntry = 0;

	public Transform contentEntryPanel;
	public GameObject entryPrefab;
	public FirstPersonInteractor FPI;

	// Name of the journal UI object in game scene.
	private const string JOURNAL = "Journal";
	// Name of the entry panel UI object in game scene.
	private const string ENTRYPANEL = "EntryPanel";

	GameObject journalGameObject;
	GameObject entryPanelGameObject;
	GameObject infoPanelGameObject;

	void Start() {
		journalGameObject = gameObject.transform.Find (JOURNAL).gameObject;
		journalGameObject.SetActive (active);
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
		journalGameObject.SetActive (true);
	}

	public void closeJournal(){
		journalGameObject.SetActive (false);
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
			GameObject newButton = (GameObject)GameObject.Instantiate(entryPrefab);

			newButton.transform.SetParent(contentEntryPanel);

			// Scale the button.
			fitPrefabToEntryPanelSize(newButton);

			JournalTitleGuiButton jtgb = newButton.GetComponent<JournalTitleGuiButton>();
			jtgb.Setup(e);

			startingEntry += 1;
		}
	}

	private void fitPrefabToEntryPanelSize(GameObject button) {
		double actualWidth = journalGameObject.transform.Find(ENTRYPANEL).GetComponent<RectTransform> ().rect.width; // This is the actual width we want to prefab to be after scaling.
		float originalWidth = button.GetComponent<LayoutElement>().preferredWidth; // This is the preset preferred width of the prefab.

		// Constrain the prefab width to its parent panel.
		RectTransform rt = button.transform.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2((float)actualWidth, rt.rect.height);

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)originalWidth;
		button.transform.localScale = new Vector3 (scale, scale, scale);
	}
}
