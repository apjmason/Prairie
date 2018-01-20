using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalGui : MonoBehaviour {
	private bool active = false;
	private bool isJournalOpen = false;

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
				Time.timeScale = 0;
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			} else {
				isJournalOpen = false;
				closeJournal ();
				Time.timeScale = 1;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
				
		}
	}


	public void openJournal(){
		gameObject.transform.Find(JOURNAL).gameObject.SetActive (true);
	}

	public void closeJournal(){
		gameObject.transform.Find(JOURNAL).gameObject.SetActive (false);
	}

	public bool isOpen() {
		return isJournalOpen;
	}
}
