using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	private bool open;

	// Use this for initialization
	void Awake () {
		open = false;
		gameObject.transform.GetChild(0).gameObject.SetActive (false);
		gameObject.transform.GetChild(1).gameObject.SetActive (false);
		gameObject.transform.GetChild(2).gameObject.SetActive (false);
		Debug.Log (gameObject.transform.GetChild (0).gameObject.activeSelf);
		Debug.Log (open);
	}
	
	void Update(){
		if (Input.GetKeyDown ("escape")) {
			//if (gameObject.transform.GetChild(0).gameObject.activeSelf == false) {
			if (!open) {
				gameObject.transform.GetChild(0).gameObject.SetActive (true);
				open = true;
				Time.timeScale = 0;
				Cursor.visible = true;
				Debug.Log (open);
				GameObject.FindGameObjectsWithTag ("Player")[0].GetComponent<FirstPersonInteractor> ().enabled = false;
			} else {
				open = false;
				gameObject.transform.GetChild(0).gameObject.SetActive (false);
				gameObject.transform.GetChild(1).gameObject.SetActive (false);
				gameObject.transform.GetChild(2).gameObject.SetActive (false);
				Time.timeScale = 1;
				Cursor.visible = false;
				GameObject.FindGameObjectsWithTag ("Player")[0].GetComponent<FirstPersonInteractor> ().enabled = true;

			}
		}

	}

	public void resume(){
		gameObject.transform.GetChild(0).gameObject.SetActive (false);
		Time.timeScale = 1;
		Cursor.visible = false;
		Debug.Log("Pressed esc, unpaused");
		GameObject.FindGameObjectsWithTag ("Player")[0].GetComponent<FirstPersonInteractor> ().enabled = true;

	}

	public void showKeybindings(){
		gameObject.transform.GetChild(0).gameObject.SetActive (false);
		gameObject.transform.GetChild(2).gameObject.SetActive (true);
	}

	public void options(){
		gameObject.transform.GetChild(0).gameObject.SetActive (false);
		gameObject.transform.GetChild(1).gameObject.SetActive (true);
	}

	public void quit(){
		Debug.Log ("is this quitting?");
		Application.Quit();
	}

	public void backToMainMenu(){
		gameObject.transform.GetChild(0).gameObject.SetActive (true);
		gameObject.transform.GetChild(1).gameObject.SetActive (false);
		gameObject.transform.GetChild(2).gameObject.SetActive (false);
	}

}
