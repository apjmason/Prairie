using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Prairie/Inventory/InventoryInteraction")]
public class InventoryInteraction : Interaction
{
	private GameObject player;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindWithTag("Player");
	}

	protected override void PerformAction () 
	{
		this.gameObject.SetActive (false);
		// Hacky way of getting the inventory script in canvass. 
		// Only work if only one inventory is attached to one player.
		player.GetComponentsInChildren<Inventory> ()[0].AddToInventory (this.gameObject);
	}
}

