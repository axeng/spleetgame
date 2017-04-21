using System;
using UnityEngine;
namespace Assets.Script
{
	public class Block
	{
		private string type;
		private Vector3 position;
		private float rotation;

		private string tag;
		
		public Block(BlockType type, Vector3 position, float rotation, string tag = "") : this (type + "_col", position, rotation, tag){}
		
		public Block(string type, Vector3 position, float rotation, string tag = "")
		{
			this.type = type;
			this.position = position;
			this.rotation = rotation;
			this.tag = tag;
		}
		
		public void Place()
		{
			GameObject obj = UnityEngine.Object.Instantiate(
				Resources.Load(this.type),
				this.position, 
				Quaternion.Euler(0.0f, rotation, 0.0f)) as GameObject;

			if (tag != "")
				obj.tag = tag;
		}
	}
	
	public enum BlockType
	{
		Xcorridor,
		Tcorridor,
		Lcorridor,
		Icorridor,
		FloorNeonAlea,
		Door3D,
        Button,
        Enigme1
	}
}
