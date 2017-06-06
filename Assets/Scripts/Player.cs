using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    class Player
    {
		//pseudo
		private int id;

		public GameObject obj { get; set; }
		public Rigidbody body { get; set; }
		public CharacterController controller { get; set; }
        private Vector3 currentCheckpoint;
       	
       	private Settings settings;
       	
        private Map map;

		private float distToGround;
		public bool doubleJump;
		public double stopAdr;
		public bool adr;

		public int nbHints;

		public List<GameObject> toActivate;
		
		public Player(int id, GameObject obj, Map map)
        {
			this.id = id;
			
			this.obj = obj;
			this.body = obj.GetComponent<Rigidbody>();
			this.controller = obj.GetComponent<CharacterController>();

			this.map = map;

			settings = new Settings();

			//set the spawn point as the current checkpoint
			this.currentCheckpoint = new Vector3(body.position.x, body.position.y, body.position.z);

			this.distToGround = this.obj.GetComponent<Collider>().bounds.extents.y;

			this.doubleJump = false;
			this.stopAdr = 0;
			this.adr = false;

			this.nbHints = 2;

			this.toActivate = new List<GameObject>();
        }


        public void ChangeCheck() //Change le checkpoint du joueur si il est dans la zone d'un checkpoint
        {
			foreach (Checkpoint check in map.GetCheckpoints())
			{
				if (check.CheckInZone(body.position))
				{
					this.currentCheckpoint = check.GetRespawn();
					break;
				}
			}
        }


        public void Move() //Sera appelé après chaque déplacement du joueur
        {
            ChangeCheck();
			
			if (body.position.y <= -6)
				Die();

        }

		public void Die()
		{
			this.Tp(currentCheckpoint);
			
			foreach (GameObject gameObject in toActivate)
				gameObject.SetActive(true);

			adr = false;
			nbHints = 0;
			doubleJump = false;

			//GameObject.FindWithTag("GameOverGUI").transform.GetChild(0).gameObject.SetActive(true);
		}
		
		public void ActiveAdrenaline(int seconds)
		{
			double mili = seconds * 1000;
			this.stopAdr = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds + seconds * 1000d;
			doubleJump = true;
			adr = true;
			Game.game.PopupMessage("Adrenaline active for "+seconds+"s", 1);
		}
		
		public void Tp(Vector3 loc)
		{
			obj.transform.position = loc;
		}

        public void SetMap(Map m)
        {
            this.map = m;
        }
        
        public Settings GetSettings()
        {
			return this.settings;
		}

		public Vector3 GetCurrentCheckpoint()
		{
			return this.currentCheckpoint;
		}

		public void SetCurrentCheckpoint(Vector3 vector)
		{
			this.currentCheckpoint = vector;
		}
		
		public bool IsGrounded()
		{
			return Physics.Raycast(this.obj.transform.position, -Vector3.up, distToGround + 0.1f);
		}
		
		public IEnumerator FadeGameOver(float delay)
		{
			GameObject canvas = GameObject.FindWithTag("GameOverGUI").transform.GetChild(0).gameObject;

			float step = 1.0f / (delay * 100.0f);
		
			canvas.SetActive(true);
			while (delay > 0)
			{
				yield return new WaitForSeconds(0.01f);
				canvas.transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(delay * 100.0f * step);
				delay -= 0.01f;
			}
			canvas.SetActive(false);
			canvas.transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(1.0f);
		}
    }
}
