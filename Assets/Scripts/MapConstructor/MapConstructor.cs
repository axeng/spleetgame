using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script.MapConstructor
{
	public class MapConstructor : MonoBehaviour
	{
		private List<Block> blocks;

		public MapConstructor(List<Block> blocks)
		{
			this.blocks = blocks;
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
