using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    class Window : Executable
    {

		private Animator anim;
		private GameObject obj;

		public Window(List<Button> listB, GameObject obj) : base(listB)
        {
			this.obj = obj;
			this.anim = this.obj.GetComponent<Animator>();
        }


        public override void Exec()
        {
        	this.anim.SetBool("IsOpen", this.CheckButton());
        }
    }
}
