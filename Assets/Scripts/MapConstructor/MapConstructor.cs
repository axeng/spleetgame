using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script
{
	public class MapConstructor
	{
		private List<Block> blocks;

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
		}

		//Construct the MapConstructor
		public void Construct()
		{
			foreach(Block b in blocks)
			{
				b.Place();
			}
		}
	}
}
