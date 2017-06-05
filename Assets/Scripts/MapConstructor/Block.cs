using System;
using UnityEngine;
namespace Assets.Script
{
	public class Block
	{
		private string type;
		public Vector3 position { get; set; }
		private Vector3 rotation;

		private string name;
		
		public Block(BlockType type, Vector3 position, float rotation, string name = "") : this (type + "_col", position, rotation, name){}
		
		public Block(string type, Vector3 position, float rotation, string name = "") : this(type, position, new Vector3(0,rotation,0), name) {}
		
		public Block(string type, Vector3 position, Vector3 rotation, string name = "")
		{
			this.type = type;
			this.position = position;
			this.rotation = rotation;
			this.name = name;
		}
		
		public GameObject Place()
		{
			GameObject obj = UnityEngine.Object.Instantiate(
				Resources.Load(this.type),
				this.position, 
				Quaternion.Euler(rotation)) as GameObject;

			if (name != "")
			{
				obj.gameObject.name = this.name;
				if (name.StartsWith("door_"))
				{
					for (int i = 0; i < obj.transform.childCount; i++)
					{
						GameObject child = obj.transform.GetChild(i).gameObject;
						if (child.tag == "left")
							child.name = name + "_left";
						else if (child.tag == "right")
							child.name = name + "_right";
					}
				}
				else if (name.StartsWith("button_") || name.StartsWith("pressionplate_"))
				{
					for (int i = 0; i < obj.transform.childCount; i++)
					{
						GameObject child = obj.transform.GetChild(i).gameObject;
						if (child.tag == "true")
							child.name = name + "_true";
						else if (child.tag == "false")
							child.name = name + "_false";
					}
				}
			}

			return obj;
		}

        public override string ToString()
        {
            return base.ToString();
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
