using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
	class Door : Executable
	{
		//the two elemement of the door
		private GameObject door;
		private GameObject leftPane;
		private GameObject rightPane;

		//the obj is for the middle element
		public Door(List<Button> listB, GameObject door, GameObject leftPane, GameObject rightPane) : base(listB)
		{
			this.leftPane = leftPane;
			this.rightPane = rightPane;
		}

		public override void Exec()
		{
			this.leftPane.SetActive(!this.CheckButton());
			this.rightPane.SetActive(!this.CheckButton());
		}
	}
}
