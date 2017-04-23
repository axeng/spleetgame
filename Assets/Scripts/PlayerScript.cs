using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	private int id;

	void Start()
	{
		id = Game.AddPlayer(gameObject);
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.tag == "die")
		{
			Game.players[id].Die();
		}
	}
}
