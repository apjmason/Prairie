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

	// Name of the journal object in game scene.
	private const string JOURNAL = "Journal";

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

			JournalTitleGuiButton jtgb = newButton.GetComponent<JournalTitleGuiButton>();
			jtgb.Setup(e);

			startingEntry += 1;
		}
	}
}
