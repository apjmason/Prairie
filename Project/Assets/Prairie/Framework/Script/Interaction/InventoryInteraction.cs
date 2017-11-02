using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryInteraction : Interaction
{
	public bool pickable = true;
	private GameObject player;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindWithTag("Player");
	}

	protected override void PerformAction () 
	{
		if (pickable) {
			this.gameObject.SetActive (false);
			player.GetComponent<Inventory> ().AddToInventory (this.gameObject);
		}
	}
}

