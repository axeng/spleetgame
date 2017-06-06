using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    public abstract class Executable
    {
        protected List<Executor> listExec;

        public Executable(List<Executor> listE)
        {
			this.listExec = listE;
        }

        public bool CheckExecutors()  // Check si tous les boutons sont activés
        {
			return listExec.TrueForAll(p=>p.IsActivate());
        }

        public abstract void Exec(); 


		public void SetExecutors(List<Executor> execs)
		{
			this.listExec = execs;
		}
		public List<Executor> GetExecutors()
		{
			return listExec;
		}
    }

	public enum ExecType
	{
		DOOR,
		WINDOW
	}
}
