using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    class Window : Executable
    {

		private GameObject obj;

		public Window(List<Button> listB, GameObject obj) : base(listB)
        {
			this.obj = obj;
        }


        public override void Exec()
        {
			this.obj.SetActive(!this.CheckButton());
        }
    }
}
