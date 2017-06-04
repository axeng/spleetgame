using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    class Button : Executor
    {
		public Button(Executable e, GameObject active, GameObject nonActive, bool tag = true) : 
			base (e, active, nonActive, ExecutorType.BUTTON, tag)
        {
        }
    }
}
