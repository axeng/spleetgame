using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	public float speedMov = 2.0f;
	public float speedCam = 2.0f;
	public float jumpForce = 2.0f;
	public float sprintMultiplier = 1.5f;

	private int id;
	private Player player;
	private Settings settings;
	private Camera camera;

	private float rotX;
	private float rotY;

	private int jumpCount;

	void Start()
	{
		this.id = Game.AddPlayer(gameObject);
		this.player = Game.players[id];
		this.settings = new Settings();
		this.camera = Camera.main;

		//this.rotX = camera.transform.rotation.eulerAngles.x;
		//this.rotY = camera.transform.rotation.eulerAngles.y;
		this.rotX = 0;
		this.rotY = 0;

		this.jumpCount = 0;
	}
	
	void OnCollisionEnter(Collision hit)
	{
		switch(hit.gameObject.tag)
		{
			case "die":
				this.player.Die();
				break;
				
			case "adr":
				hit.gameObject.SetActive(false);
				this.player.ActiveAdrenaline(10);
				break;
		}
	}
	
	void FixedUpdate()
	{
		if (Game.game.pause)
			return;

		bool boostspeed = false;

		if ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds >= this.player.stopAdr)
			this.player.doubleJump = false;
		else
			boostspeed = true;
			


		Vector3 move = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
		
		if (Input.GetKey(settings.forward))
		{
			move.z += 0.1f * speedMov;
		}
		if (Input.GetKey(settings.left))
		{
			move.x -= 0.1f * speedMov;
		}
		if (Input.GetKey(settings.backward))
		{
			move.z -= 0.1f * speedMov;
		}
		if (Input.GetKey(settings.right))
		{
			move.x += 0.1f * speedMov;
		}

		if (Input.GetKey(settings.sprint))
			move = move * sprintMultiplier;

		if (boostspeed)
			move = move * 2;

		if (this.player.IsGrounded())
		{
			jumpCount = 0;
		}

		if (Input.GetKeyDown(settings.jump) && (jumpCount == 0 || this.player.doubleJump && jumpCount <= 2))
		{
			force.y += 5.0f * jumpForce;
			jumpCount++;
			
			player.body.velocity = force;
		}

		rotX += Input.GetAxis("Mouse X") * speedCam;
		rotY -= Input.GetAxis("Mouse Y") * speedCam;
		if (rotY > 80)
			rotY = 80;
		else if (rotY < -80)
			rotY = -80;

		transform.rotation = Quaternion.Euler(0, rotX, 0);
		transform.Translate(move);

		//camera.transform.Translate(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
		//transform.rotation.Set(0, camera.transform.rotation.y, 0, 0);
		camera.transform.rotation = Quaternion.Euler(rotY, rotX, 0);
	}
	
}
