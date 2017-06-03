using System;
using UnityEngine;

namespace Assets.Script
{
	
	public class Settings
	{
		public KeyCode forward { get; set; }
		public KeyCode left { get; set; }
		public KeyCode backward { get; set; }
		public KeyCode right { get; set; }
		
		public KeyCode jump { get; set; }
		
		public Settings()
		{
			forward = KeyCode.Z;
			left = KeyCode.Q;
			backward = KeyCode.S;
			right = KeyCode.D;

			jump = KeyCode.Space;
		}
	}
}
