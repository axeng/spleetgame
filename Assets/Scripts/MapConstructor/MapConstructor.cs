using System;
using System.Collections.Generic;
using UnityEngine;
namespace AssemblyCSharp
{
	public class MapConstructor : MonoBehaviour
	{
		Dictionary<string, Vector3> objects;

		public MapConstructor(Dictionary<string, Vector3> objects)
		{
			this.objects = objects;
		}

		public void Construct()
		{
			//foreach (string obj in objects)
			//{
				//Instantiate(Resources.Load(obj), objects[obj], Quaternion)
			//}
		}
	}
}
