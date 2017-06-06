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
	public float adrenalineMultiplier = 2.0f;
	public int adrenalineDuration = 10;
	public float minDegreeView = -80.0f;
	public float maxDegreeView = 80.0f;
	

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
				StartCoroutine(this.player.FadeGameOver(0.5f));
				break;
				
			case "adr":
				hit.gameObject.SetActive(false);
				this.player.toActivate.Add(hit.gameObject);
				this.player.ActiveAdrenaline(adrenalineDuration);
				break;
				
			case "dop":
				hit.gameObject.SetActive(false);
				//this.player.toActivate.Add(hit.gameObject);
				this.player.nbHints++;
				string s = "s";
				if (this.player.nbHints < 2)
					s = "";
				Game.game.PopupMessage("You have " + this.player.nbHints + " hint" + s, 1);
				break;
				
			case "tp":
				hit.gameObject.SetActive(false);
				if (Game.level > 0 && hit.gameObject.name.Contains("Stp") && !Game.finishLevels.Contains(Game.map.GetName()))
				{
					Game.level--;
					Game.finishLevels.Add(Game.map.GetName());
				}
				//yolo code
				Game.game.StartCoroutine(Game.game.LoadMapTime(hit.gameObject.name.Replace("tp_", "").Replace("%l", ""+Game.level)));
				break;

			default:
				Game.map.WalkOn(hit.gameObject);
				break;
		}
	}
	
	void FixedUpdate()
	{
		if (Game.game.pause)
			return;

		if (this.player.adr && (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds >= this.player.stopAdr)
		{
			this.player.doubleJump = false;
			this.player.adr = false;
		}
			
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

		if (this.player.adr)
			move = move * adrenalineMultiplier;

		if (this.player.IsGrounded())
		{
			jumpCount = 0;
		}
		
		if ((this.player.IsGrounded() || this.player.doubleJump) && Input.GetKeyDown(settings.jump) && (jumpCount == 0 || this.player.doubleJump && jumpCount <= 2))
		{
			force.y += 5.0f * jumpForce;
			jumpCount++;
			
			player.body.velocity = force;
		}
		
		rotX += Input.GetAxis("Mouse X") * speedCam;
		rotY -= Input.GetAxis("Mouse Y") * speedCam;
		if (rotY > maxDegreeView)
			rotY = maxDegreeView;
		else if (rotY < minDegreeView)
			rotY = minDegreeView;

		transform.rotation = Quaternion.Euler(0, rotX, 0);
		transform.Translate(move);

		//camera.transform.Translate(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
		//transform.rotation.Set(0, camera.transform.rotation.y, 0, 0);
		camera.transform.rotation = Quaternion.Euler(rotY, rotX, 0);
	}
	
}
