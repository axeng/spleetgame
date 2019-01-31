using System;
using UnityEngine;

namespace Assets.Script
{
	public abstract class Executor
	{
		//two game object to switch
		protected GameObject active;
		protected GameObject nonActive;

        protected Executable executable;// Contient une liste d'objet pouvant être activé avec les boutons
        protected bool activate;

		protected bool tag;

		public ExecutorType type { get; private set; }

		public Executor(Executable e, GameObject active, GameObject nonActive, ExecutorType type, bool tag = true)
        {
			this.active = active;
			this.nonActive = nonActive;

            this.executable = e;
			this.activate = false;
			this.type = type;
			this.tag = tag;
			//this.Push();
			UpdateColors();
        }

        //check if the button is on or off
        public bool IsActivate()
        {
            return activate;
        }
        
        // Active le bouton ou le desactive
        public virtual void Push()           
        {
            this.activate = !this.activate;

			UpdateColors();
        }

		public void UpdateColors()
		{
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
			if (tag)
				return obj.tag == active.tag || obj.tag == nonActive.tag;
			else
				return obj.name == active.name || obj.name == nonActive.name;
		}

		public void SetExecutable(Executable executable)
		{
			this.executable = executable;
		}
	}
	
	public enum ExecutorType
	{
		BUTTON,
		PRESSIONPLATE
	}
}
