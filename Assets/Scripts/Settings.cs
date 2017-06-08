using System;
using System.Net;
using UnityEngine;

namespace Assets.Script
{
	
	public class Settings
	{
		public KeyCode forward { get; set; }
		public KeyCode left { get; set; }
		public KeyCode backward { get; set; }
		public KeyCode right { get; set; }
		
		public KeyCode sprint { get; set; }
		
		public KeyCode jump { get; set; }
		
		public KeyCode pause { get; set; }
		public KeyCode hint { get; set; }
		
		public KeyCode fire { get; set; }

		public int mouseSensibility { get; set; }
		
		
		public int volume { get; set; }
		
		
		public bool fullscreen { get; set; }
		
		public int resolutionW { get; set; }
		public int resolutionH { get; set; }
		
		public bool vSync { get; set; }

		public ShadowResolution shadowResolution { get; set; }
		
		public bool shadow { get; set; }
		
		public int antiAliasing { get; set; }
		
		public bool particlesReduction { get; set; }
		
		public int fov { get; set; }
		
		
		public bool host { get; set; }
		public IPAddress ip { get; set; }
		public int port { get; set; }
		
		
		public string lang { get; set; }
		
		public Settings()
		{
			LoadSettings();
		}
		
		private void LoadSettings()
		{
			forward = (KeyCode)PlayerPrefs.GetInt("Sforward", (int)KeyCode.Z);
			left = (KeyCode)PlayerPrefs.GetInt("Sleft", (int)KeyCode.Q);
			backward = (KeyCode)PlayerPrefs.GetInt("Sbackward", (int)KeyCode.S);
			right = (KeyCode)PlayerPrefs.GetInt("Sright", (int)KeyCode.D);

			sprint = (KeyCode)PlayerPrefs.GetInt("Ssprint", (int)KeyCode.LeftShift);

			jump = (KeyCode)PlayerPrefs.GetInt("Sjump", (int)KeyCode.Space);
			
			pause = (KeyCode)PlayerPrefs.GetInt("Spause", (int)KeyCode.P);
			hint = (KeyCode)PlayerPrefs.GetInt("Shint", (int)KeyCode.H);

			fire = (KeyCode)PlayerPrefs.GetInt("Sfire", (int)KeyCode.Mouse0);

			mouseSensibility = PlayerPrefs.GetInt("SmouseSensibility", 20);

			
			volume = PlayerPrefs.GetInt("Svolume", 5);


			fullscreen = PlayerPrefs.GetInt("Sfullscreen", Convert.ToInt32(Screen.fullScreen)) != 0;

			resolutionW = PlayerPrefs.GetInt("SresolutionW", Screen.currentResolution.width);
			resolutionH = PlayerPrefs.GetInt("SresolutionH", Screen.currentResolution.height);
			
			vSync = PlayerPrefs.GetInt("Svsync", QualitySettings.vSyncCount) != 0;
			
			shadowResolution = (ShadowResolution)PlayerPrefs.GetInt("SshadowResolution", (int)QualitySettings.shadowResolution);
			
			shadow = PlayerPrefs.GetInt("Sshadow", Convert.ToInt32(QualitySettings.shadows == ShadowQuality.All)) != 0;
			
			antiAliasing = PlayerPrefs.GetInt("SantiAliasing", QualitySettings.antiAliasing);
			
			particlesReduction = PlayerPrefs.GetInt("SparticlesReduction", Convert.ToInt32(QualitySettings.softParticles)) != 0;

			fov = PlayerPrefs.GetInt("Sfov", 60);


			lang = PlayerPrefs.GetString("Slang", "english");
			
			host = PlayerPrefs.GetInt("Shost", 0) != 0;
			ip = IntToIpAdress(PlayerPrefs.GetInt("Sip", IpAdressToInt(IPAddress.Parse("127.0.0.1"))));
			port = PlayerPrefs.GetInt("Sport", 7777);
			
			
			SaveSettings();
		}
		
		public void SaveSettings()
		{
			PlayerPrefs.SetInt("Sforward", (int)forward);
			PlayerPrefs.SetInt("Sleft", (int)left);
			PlayerPrefs.SetInt("Sbackward", (int)backward);
			PlayerPrefs.SetInt("Sright", (int)right);
			
			PlayerPrefs.SetInt("Ssprint", (int)sprint);
			
			PlayerPrefs.SetInt("Sjump", (int)jump);
			
			PlayerPrefs.SetInt("Spause", (int)pause);
			PlayerPrefs.SetInt("Shint", (int)hint);
			
			PlayerPrefs.SetInt("Sfire", (int)fire);
			
			PlayerPrefs.SetInt("SmouseSensibility", mouseSensibility);
			
			
			PlayerPrefs.SetInt("Svolume", volume);
			
			
			PlayerPrefs.SetInt("Sfullscreen", Convert.ToInt32(fullscreen));
			
			PlayerPrefs.SetInt("SresolutionW", resolutionW);
			PlayerPrefs.SetInt("SresolutionH", resolutionH);
			
			PlayerPrefs.SetInt("Svsync", Convert.ToInt32(vSync));
			
			PlayerPrefs.SetInt("SshadowResolution", (int)shadowResolution);
			
			PlayerPrefs.SetInt("Sshadow", Convert.ToInt32(shadow));
			
			PlayerPrefs.SetInt("SantiAliasing", antiAliasing);
			
			PlayerPrefs.SetInt("SparticlesReduction", Convert.ToInt32(particlesReduction));
			
			PlayerPrefs.SetInt("Sfov", fov);
			
			PlayerPrefs.SetInt("Shost", Convert.ToInt32(host));
			PlayerPrefs.SetInt("Sip", IpAdressToInt(ip));
			PlayerPrefs.SetInt("Sport", port);
			
			PlayerPrefs.SetString("Slang", lang);
			
			PlayerPrefs.Save();
		}
		
		public int IpAdressToInt(IPAddress ip)
		{
			return BitConverter.ToInt32(IPAddress.Parse(ip.ToString()).GetAddressBytes(), 0);
		}
		
		public IPAddress IntToIpAdress(int ip)
		{
			return IPAddress.Parse(new IPAddress(BitConverter.GetBytes(ip)).ToString());
		}
	}
}
