using System;
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
        private Vector3 currentCheckpoint;
       	
        private Map map;

		public Player(int id, GameObject obj, Map map)
        {
			this.id = id;
			
			this.obj = obj;
			this.body = obj.GetComponent<Rigidbody>();

			this.map = map;

			//set the spawn point as the current checkpoint
			this.currentCheckpoint = new Vector3(body.position.x, body.position.y, body.position.z);
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
			
			if (body.position.y <= -4)
				Die();

        }

		public void Die()
		{
			this.Tp(currentCheckpoint);
		}
		
		public void Tp(Vector3 loc)
		{
			obj.transform.position = loc;
		}

        public void SetMap(Map m)
        {
            this.map = m;
        }

		public Vector3 GetCurrentCheckpoint()
		{
			return this.currentCheckpoint;
		}

		public void SetCurrentCheckpoint(Vector3 vector)
		{
			this.currentCheckpoint = vector;
		}
    }
}
