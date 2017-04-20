﻿using System;
using UnityEngine;
namespace Assets.Script
{
	public class Block
	{
		private BlockType type;
		private Vector3 position;
		private float rotation;
		
		public Block(BlockType type, Vector3 position, float rotation)
		{
			this.type = type;
			this.position = position;
			this.rotation = rotation;
		}
		
		public void Place()
		{
			UnityEngine.Object.Instantiate(
				Resources.Load(this.type+"_col"),
				this.position, 
				Quaternion.Euler(0.0f, rotation, 0.0f));
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
