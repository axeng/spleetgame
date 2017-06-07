using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script;
using System.Net;
using UnityEngine.UI;
using System;

namespace Assets.Script
{
	public class NetworkMenuManager : MonoBehaviour
	{
		public InputField ipF;
		public InputField portF;

		bool second;
	
		// Use this for initialization
		void Start()
		{
			second = false;
		}

		// Update is called once per frame
		void Update()
		{
			if (!second)
			{
				second = true;
				ipF.text = SettingsMenuManager.settings.ip.ToString();
				portF.text = SettingsMenuManager.settings.port.ToString();
			}
		}
		
		public void Host()
		{
			SettingsMenuManager.settings.host = true;
			SettingsMenuManager.settings.SaveSettings();
		}
		
		public void CheckIp()
		{
			string ip = ipF.text;
			IPAddress adress = IPAddress.Any;
			if (ip.Split('.').Length != 4 || !IPAddress.TryParse(ip, out adress))
			{
				ColorBlock b = ipF.colors;
				b.normalColor = Color.red;
				b.highlightedColor = Color.red;
				ipF.colors = b;
			}
			else
			{
				ColorBlock b = ipF.colors;
				b.normalColor = Color.green;
				b.highlightedColor = Color.green;
				ipF.colors = b;
				SettingsMenuManager.settings.ip = adress;
				SettingsMenuManager.settings.SaveSettings();
			}
		}
		
		public void CheckPort()
		{
			string port = portF.text;
			int p = -1;
			if (!Int32.TryParse(port, out p) || p < 0 || p >= 65535)
			{
				ColorBlock b = portF.colors;
				b.normalColor = Color.red;
				b.highlightedColor = Color.red;
				portF.colors = b;
			}
			else
			{
				ColorBlock b = portF.colors;
				b.normalColor = Color.green;
				b.highlightedColor = Color.green;
				portF.colors = b;
				SettingsMenuManager.settings.port = p;
				SettingsMenuManager.settings.SaveSettings();
			}
		}
		
		public void Join()
		{
			if (ipF.colors.highlightedColor == Color.green && portF.colors.highlightedColor == Color.green)
				UnityEngine.SceneManagement.SceneManager.LoadScene(3);
		}
	}
}
