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
	private Camera camera;

	private float rotX;
	private float rotY;

	private bool canDoubleJump;

	void Start()
	{
		this.id = Game.AddPlayer(gameObject);
		this.player = Game.players[id];
		this.camera = Camera.main;

		//this.rotX = camera.transform.rotation.eulerAngles.x;
		//this.rotY = camera.transform.rotation.eulerAngles.y;
		this.rotX = 0;
		this.rotY = 0;

		canDoubleJump = this.player.doubleJump;
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
				Player.nbHints++;
				string s = "s";
				if (Player.nbHints < 2)
					s = "";
				Game.game.PopupMessage("You have " + Player.nbHints + " hint" + s, 1);
				break;
				
			case "tp":
				hit.gameObject.SetActive(false);
				if (Game.level > 0 && hit.gameObject.name.Contains("Stp") && !Game.finishLevels.Contains(Game.map.GetName()))
				{
					Game.level--;
					Game.finishLevels.Add(Game.map.GetName());
					Game.game.Save();
				}
				//yolo code
				Game.game.StartCoroutine(Game.game.LoadMapTime(hit.gameObject.name.Replace("tp_", "").Replace("%l", ""+Game.level)));
				break;

			default:
				Game.map.WalkOn(hit.gameObject);
				break;
		}
	}
	
	void Update()
	{
		if (Game.game.pause)
			return;
			
		if (this.player.adr && (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds >= this.player.stopAdr)
		{
			this.player.doubleJump = false;
			this.player.adr = false;
		}
		
		Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
		
		if (Input.GetKeyDown(Player.settings.jump))
		{
			if (Physics.Raycast(transform.position, -Vector3.up, GetComponent<Collider>().bounds.extents.y + 0.1f))
			{
				force.y += 5.0f * jumpForce;
			
				player.body.velocity = force;
				canDoubleJump = true;
			}
			else if (this.player.doubleJump && canDoubleJump)
			{
				player.body.velocity.Set(player.body.velocity.x, 0, player.body.velocity.y);
				force.y += 5.0f * jumpForce;
			
				player.body.velocity = force;
				canDoubleJump = false;
			}
		}
		
		if (Input.GetKeyDown(Player.settings.hint))
		{
			if (Player.nbHints > 0 && Game.map.PopOneHint())
				Player.nbHints--;
		}
	}	
	void FixedUpdate()
	{
		if (Game.game.pause)
			return;
			
		Vector3 move = new Vector3(0.0f, 0.0f, 0.0f);
		
		if (Input.GetKey(Player.settings.forward))
		{
			move.z += 0.1f * speedMov;
		}
		if (Input.GetKey(Player.settings.left))
		{
			move.x -= 0.1f * speedMov;
		}
		if (Input.GetKey(Player.settings.backward))
		{
			move.z -= 0.1f * speedMov;
		}
		if (Input.GetKey(Player.settings.right))
		{
			move.x += 0.1f * speedMov;
		}

		if (Input.GetKey(Player.settings.sprint))
			move = move * sprintMultiplier;

		if (this.player.adr)
			move = move * adrenalineMultiplier;

		//Debug.Log(canDoubleJump);

		/*if (this.player.IsGrounded())
		{
			jumpCount = 0;
		}

		if ((this.player.IsGrounded() || this.player.doubleJump) && Input.GetKeyDown(Player.settings.jump) && (jumpCount == 0 || this.player.doubleJump && jumpCount < 2))
		{
		//if (Input.GetKey(Player.settings.jump))
		//{
			force.y += 5.0f * jumpForce;
			jumpCount++;
			
			player.body.velocity = force;
		}*/
		
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
