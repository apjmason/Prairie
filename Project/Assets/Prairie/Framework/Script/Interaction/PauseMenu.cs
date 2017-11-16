using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		gameObject.transform.GetChild(0).gameObject.SetActive (false);
		Debug.Log (gameObject.transform.GetChild (0).gameObject.activeSelf);
	}
	
	void Update(){
		if (Input.GetKeyDown ("escape")) {
			if (gameObject.transform.GetChild(0).gameObject.activeSelf == false) {
				gameObject.transform.GetChild(0).gameObject.SetActive (true);
				Time.timeScale = 0;
				Debug.Log("Pressed esc, paused");
				Cursor.visible = true;
				GameObject.FindGameObjectsWithTag ("Player")[0].GetComponent<FirstPersonInteractor> ().enabled = false;
			} else {
				gameObject.transform.GetChild(0).gameObject.SetActive (false);
				Time.timeScale = 1;
				Cursor.visible = false;
				//Debug.Log("Pressed esc, unpaused");
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


	public void quit(){
		Debug.Log ("is this quitting?");
		Application.Quit();
	}

}
