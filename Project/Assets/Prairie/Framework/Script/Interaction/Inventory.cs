using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory : MonoBehaviour
{	
	public const int numSlots = 4;
	public Image[] objImages = new Image[numSlots];
	public GameObject[] objs = new GameObject[numSlots];

//	int rows = 2;
//	int columns = 5;
//
//	private GameObject player;

//	// Use this for initialization
//	void Start ()
//	{
////		// Need to attach this script to 
////		player = this.gameObject;
////		Debug.Log ("Inventory attached to " + player.name);
//	}

	public void AddToInventory (GameObject objToAdd)
	{
		for (int i = 0; i < objs.Length; i++) {
			if (objs [i] == null) {
				objs [i] = objToAdd;
				objImages [i] = objToAdd.GetComponents<Image>()[0];
				objImages [i].enabled = true;
				return;
			}
		}
	}

	public void RemoveFromInventory (GameObject objToRemove)
	{
		for (int i = 0; i < objs.Length; i++) {
			if (objs [i] == objToRemove) {
				objs [i] = null;
				objImages [i] = null;
				objImages [i].enabled = false;
				return;
			}
		}
	}
}

