using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script
{
	public class PressionPlate : Executor
	{
		private int activeDuration = 10;
	
		public PressionPlate(Executable e, GameObject active, GameObject nonActive, bool tag = true)
		: base(e, active, nonActive, ExecutorType.PRESSIONPLATE, tag)
		{
		}
		
		public override void Push()
		{
			this.activate = true;
			
			active.SetActive(this.activate);
			nonActive.SetActive(!this.activate);	
		}
		
		public IEnumerator Coroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			this.activate = false;
		}
	}
}
