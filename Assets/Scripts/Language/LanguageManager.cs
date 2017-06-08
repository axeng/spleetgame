using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

namespace Assets.Script
{
	[XmlRoot("LanguageManager")]
	public class LanguageManager
	{

		private static string g_filepath = Path.Combine(Application.dataPath, "Resources/Xml/Languages.xml");

		[XmlEnum("currentLanguage")]
		public Language currentLanguage;

		[XmlArray("translations"), XmlArrayItem("translations")]
		public List<Translation> translations;

		public LanguageManager()
		{
			currentLanguage = Language.English;
			translations = new List<Translation>();
		}


		public string GetString(string key)
		{
			return GetString(key, currentLanguage);
		}

		public string GetString(string key, Language language)
		{
			int index = FindKey(key);

			if (index >= 0)
				return translations[index].GetValue(language);

			return "";
		}


		public void Add(Translation translation)
		{
			int index = FindKey(translation.key);

			if (index >= 0)
				translations[index] = translation;
			else
				translations.Add(translation);
		}

		public void Update(Translation translation)
		{
			int index = FindKey(translation.key);

			if (index >= 0)
				translations[index] = translation;
		}

		public void Remove(Translation translation)
		{
			Remove(translation.key);
		}

		public void Remove(string key)
		{
			translations.RemoveAt(FindKey(key));
		}

		/*public void Save()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(LanguageManager));
			using (FileStream stream = new FileStream(g_filepath, FileMode.Create))
			{
				serializer.Serialize(stream, this);
			}
		}*/

		public static LanguageManager Load()
		{
			if (File.Exists(g_filepath))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(LanguageManager));
				using (FileStream stream = new FileStream(g_filepath, FileMode.Open))
				{
					return serializer.Deserialize(stream) as LanguageManager;
				}
			}
			return new LanguageManager();
		}

		public int FindKey(string key)
		{
			for (int i = 0; i < translations.Count; i++)
			{
				if (key == translations[i].key)
					return i;
			}

			return -1;
		}
	}

	public enum Language
	{
		English,
		French
	}
}