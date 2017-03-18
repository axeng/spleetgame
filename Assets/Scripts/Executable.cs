using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    abstract class Executable
    {
        protected List<Button> listButton;

        public Executable(List<Button> listB)
        {
            this.listButton = listB;
        }

        public bool CheckButton()  // Check si tous les boutons sont activés
        {
			return listButton.TrueForAll(p=>p.IsActivate());
        }

        public abstract void Exec(); 


		public void SetButtons(List<Button> butts)
		{
			this.listButton = butts;
		}
		public List<Button> GetButtons()
		{
			return listButton;
		}
    }

	enum ExecType
	{
		DOOR,
		WINDOW
	}
}
