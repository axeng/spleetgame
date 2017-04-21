using System;
using UnityEngine;
namespace Assets.Script
{
	public class Block
	{
		private string type;
		public Vector3 position { get; set; }
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
			{
				try
				{
					obj.gameObject.tag = this.tag;
				}catch(Exception e)
				{
					Debug.LogWarning("the tag "+tag+"is not defined");
				}
				if (tag.StartsWith("door_"))
				{
					for (int i = 0; i < obj.transform.childCount; i++)
					{
						GameObject child = obj.transform.GetChild(i).gameObject;
						if (child.tag == "left")
							child.tag = tag + "_left";
						else if (child.tag == "right")
							child.tag = tag + "_right";
					}
				}
				else if (tag.StartsWith("button_"))
				{
					for (int i = 0; i < obj.transform.childCount; i++)
					{
						GameObject child = obj.transform.GetChild(i).gameObject;
						if (child.tag == "true")
							child.tag = tag + "_true";
						else if (child.tag == "false")
							child.tag = tag + "_false";
					}
				}
			}
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
