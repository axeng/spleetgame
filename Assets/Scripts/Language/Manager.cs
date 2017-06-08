using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script
{
	public class Manager : MonoBehaviour
	{
		public List<Text> allTexts;
		public LanguageManager langManager;
		public static Manager manager;
		
		public Language lang;
		
		void Awake()
		{
			manager = this;
			allTexts = new List<Text>();
			langManager = LanguageManager.Load();
		}
		
		public void Refresh()
		{
			allTexts.RemoveAll(p => !p.text.Contains("ml:"));
			
			foreach (Text text in allTexts)
			{
				bool capslock = text.text.Contains("(CAPSLOCK)");

				if (capslock)
					text.text = text.text.Replace("(CAPSLOCK)", "");
					
				text.text = langManager.GetString(text.text.Replace("ml:", ""), lang);

				if (capslock)
					text.text = text.text.ToUpper();

				text.text = text.text.Replace("(LINEBREAK)", "\n");
			}
		}
	}
}
