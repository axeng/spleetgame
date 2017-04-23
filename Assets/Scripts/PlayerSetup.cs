using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	public Behaviour[] componentsToDisable;
	private int id;

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
		{
			DisableComponents();
		}
		else
		{
			id = Game.AddPlayer(gameObject);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void DisableComponents ()
	{
		for (int i = 0; i < componentsToDisable.Length; i++)
		{
			componentsToDisable[i].enabled = false;
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.tag == "die")
		{
			Game.players[id].Die();
		}
	}
}
