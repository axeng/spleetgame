using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script
{
	public class MapConstructor
	{
		private List<Block> blocks;
		private List<string> objects;

		private int xDec;
		private int yDec;
		private int zDec;

		public MapConstructor(List<Block> blocks) : this(blocks, 0, 0, 0){}
		public MapConstructor(List<Block> blocks, int xDec, int yDec, int zDec)
		{
			this.blocks = blocks;
			this.xDec = xDec;
			this.yDec = yDec;
			this.zDec = zDec;

			objects = new List<string>();
		}

		public void AddObject(string obj)
		{
			objects.Add(obj);
		}

		//Construct the MapConstructor
		public void Construct()
		{
			foreach(Block b in blocks)
			{
				b.position.Set(b.position.x + xDec, b.position.y + yDec, b.position.z + zDec);
				b.Place();
			}

			foreach (string obj in objects)
				UnityEngine.Object.Instantiate(Resources.Load(obj));
		}
	}
}
