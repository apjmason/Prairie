using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	private bool open;
	private bool showInventoryUI;

	// Use this for initialization
	void Awake () {
		open = false;
		showInventoryUI = true;
		gameObject.transform.Find("Pause Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Options Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Keybindings").gameObject.SetActive (false);
	}
	
	void Update(){
		if (Input.GetKeyDown ("escape")) {
			//if (gameObject.transform.GetChild(0).gameObject.activeSelf == false) {
			if (!open) {
				gameObject.transform.Find("Pause Menu").gameObject.SetActive (true);
				open = true;
				Time.timeScale = 0;
				Cursor.visible = true;
				GameObject.FindGameObjectsWithTag ("Player")[0].GetComponent<FirstPersonInteractor> ().enabled = false;
			} else {
				resume ();

			}
		}

	}

	public void resume(){
		open = false;
		gameObject.transform.Find("Pause Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Options Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Keybindings").gameObject.SetActive (false);
		Time.timeScale = 1;
		Cursor.visible = false;
		GameObject.FindGameObjectsWithTag ("Player")[0].GetComponent<FirstPersonInteractor> ().enabled = true;

	}

	public void showKeybindings(){
		gameObject.transform.Find("Pause Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Keybindings").gameObject.SetActive (true);
	}

	public void hideShowInventoryUI(){
		if (showInventoryUI) {
			showInventoryUI = false;
			gameObject.transform.Find("InventoryPanel").gameObject.SetActive (false);
		} else {
			showInventoryUI = true;
			gameObject.transform.Find("InventoryPanel").gameObject.SetActive (true);
		}

	}

	public void options(){
		gameObject.transform.Find("Pause Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Options Menu").gameObject.SetActive (true);
	}

	public void quit(){
		Application.Quit();
	}

	public void backToMainMenu(){
		gameObject.transform.Find("Pause Menu").gameObject.SetActive (true);
		gameObject.transform.Find("Options Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Keybindings").gameObject.SetActive (false);
	}



}
