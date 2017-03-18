using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    class Button
    {
		//two game object to switch
		private GameObject active;
		private GameObject nonActive;

        private Executable executable;// Contient une liste d'objet pouvant être activé avec les boutons
        private bool activate;

		public Button(Executable e, GameObject active, GameObject nonActive)
        {
			this.active = active;
			this.nonActive = nonActive;

            this.executable = e;
            this.activate = true;
			this.Push();
        }

        //check if the button is on or off
        public bool IsActivate()
        {
            return activate;
        }

        // Active le bouton ou le desactive
        public void Push()           
        {
            this.activate = !this.activate;

			//change buttons colors
			active.SetActive(this.activate);
			nonActive.SetActive(!this.activate);
        }

        public void Exec()
        {
			executable.Exec();
        }

		public GameObject GetActive()
		{
			return active;
		}
		public GameObject GetNonActive()
		{
			return nonActive;
		}
		//return true if obj is active or nonActive
		public bool IsThisGameObject(GameObject obj)
		{
			return obj.tag == active.tag || obj.tag == nonActive.tag;
		}

		public void SetExecutable(Executable executable)
		{
			this.executable = executable;
		}

        
    }
}
