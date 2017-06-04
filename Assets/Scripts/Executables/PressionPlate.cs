using System;
using UnityEngine;

namespace Assets.Script
{
	public class PressionPlate : Executor
	{
		public PressionPlate(Executable e, GameObject active, GameObject nonActive, bool tag = true)
		: base(e, active, nonActive, ExecutorType.PRESSIONPLATE, tag)
		{
		}
	}
}
