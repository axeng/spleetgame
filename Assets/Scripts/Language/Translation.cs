using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Script
{
	public class Translation
	{

		[XmlAttribute("key")]
		public string key;

		[XmlElement("english")]
		public string english;

		[XmlElement("french")]
		public string french;

		public Translation()
		{
			this.key = "Key";

			english = "";
			french = "";
		}

		public string GetValue(Language language)
		{
			switch (language)
			{
				case Language.English:
					return this.english;

				case Language.French:
					return this.french;

				default:
					return "";
			}
		}
	}
}
