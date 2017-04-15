using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AssemblyCSharp
{
	public class MapConstructor : MonoBehaviour
	{
		//Vector 4 : (x, y, z, w), w = rotation in degree
		Dictionary<string, Vector4> objects;

		public MapConstructor(Dictionary<string, Vector4> objects)
		{
			this.objects = objects;
		}
		
		public MapConstructor(Dictionary<BlockType, Vector4> objects)
		{
			foreach(BlockType s in objects.Keys)
			{
				this.objects.Add(s.ToString()+"_col", objects[s]);
			}
		}

		//Construct the MapConstructor
		public void Construct()
		{
			foreach (string obj in objects.Keys)
			{
				Vector3 pos = new Vector3(objects[obj].x, objects[obj].y, objects[obj].z);
				Instantiate(Resources.Load(obj), pos, Quaternion.Euler(0.0f, objects[obj].w, 0.0f));
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
		ButtonActif,
		ButtonNonActif
	}
}
