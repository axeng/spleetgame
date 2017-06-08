using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script
{
	public class CanvasManager : MonoBehaviour
	{
		
		void Start()
		{
			foreach (Text t in GetComponentsInChildren<Text>(true))
				Manager.manager.allTextsDico.Add(t, t.text);
				//Manager.manager.allTexts.Add(t);

			Manager.manager.Refresh();
		}
		
	}
}
