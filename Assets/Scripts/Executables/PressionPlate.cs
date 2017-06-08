using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script
{
	public class PressionPlate : Executor
	{
		private int activeDuration = 5;
	
		public PressionPlate(Executable e, GameObject active, GameObject nonActive, bool tag = true)
		: base(e, active, nonActive, ExecutorType.PRESSIONPLATE, tag)
		{
		}
		
		public override void Push()
		{
			this.activate = true;
			
			UpdateColors();
			
			Game.game.StartCoroutine(Coroutine(activeDuration));
		}
		
		public IEnumerator Coroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			this.activate = false;
			UpdateColors();
			this.Exec();
		}
		
		public void changeActiveDuration(int newDuration)
		{
			this.activeDuration = newDuration;
		}
	}
}
