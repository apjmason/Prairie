using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	private bool open;
	private bool showInventoryUI;
	private FirstPersonInteractor playerScript;
	private bool isTextHide = true;

	// Use this for initialization
	void Awake () {
		open = false;
		showInventoryUI = true;
		gameObject.transform.Find("Pause Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Options Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Keybindings").gameObject.SetActive (false);
		playerScript = (FirstPersonInteractor) GameObject.FindWithTag("Player").GetComponent(typeof(FirstPersonInteractor));
	}
	
	void Update(){
		if (Input.GetKeyDown ("escape")) {
			//if (gameObject.transform.GetChild(0).gameObject.activeSelf == false) {
			if (!open) {
				gameObject.transform.Find("Pause Menu").gameObject.SetActive (true);
				open = true;
				//Time.timeScale = 0;
				playerScript.setWorldActive("Pause Menu"); //WORKING HERE******************************************************************
				//Cursor.visible = true;
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
		//Time.timeScale = 1;
		//Cursor.visible = false;
		playerScript.setWorldActive("Pause Menu");
		GameObject.FindGameObjectsWithTag ("Player")[0].GetComponent<FirstPersonInteractor> ().enabled = true;

	}

	public void showKeybindings(){
		gameObject.transform.Find("Pause Menu").gameObject.SetActive (false);
		gameObject.transform.Find("Keybindings").gameObject.SetActive (true);
	}
		
	//Turns the inventory opacity to 0 if button is pressed once, and to 106 if pressed agai (alpha value is a percentage)
	public void hideShowInventoryUI(){
		GameObject inventory = GameObject.Find("InventoryPanel");
		if (showInventoryUI) {
			showInventoryUI = false;
			var noColor = inventory.GetComponent<Image>().color;
			noColor.a = 0; 
			inventory.GetComponent<Image>().color= noColor;
		} else {
			showInventoryUI = true;
			var originalColor = inventory.GetComponent<Image>().color;
			originalColor.a = .4156f;
			inventory.GetComponent<Image>().color = originalColor;
		}

	}

	//Changes the text of the Show/hide inventory UI buttom
	public void changeShowHideText(){
		Text text = GetComponentInChildren<Text> ();
		if (isTextHide) {
			text.text = "Show Inventory UI";
			isTextHide = false;
		} else {
			isTextHide = true;
			text.text = "Hide Inventory UI";
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
