using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Script
{
	public class SettingsMenuManager : MonoBehaviour
	{

		public static Settings settings;

		public Slider volumeSlider;

		public GameObject fullscreen_off;
		public GameObject fullscreen_on;

		public GameObject[] resolutions;
		private Dictionary<string, GameObject> resolutionsDico;

		public GameObject Vsync_off;
		public GameObject Vsync_on;

		public GameObject[] shadowResolutions;
		private Dictionary<ShadowResolution, GameObject> shadowResolutionsDico;

		public GameObject shadow_off;
		public GameObject shadow_on;

		public GameObject[] antiAliasings;
		public Dictionary<int, GameObject> antiAliasingsDico;

		public GameObject particlesReduction_off;
		public GameObject particlesReduction_on;

		public Slider fovSlider;


		public Text forwardText;
		public Text leftText;
		public Text backwardText;
		public Text rightText;

		public Text sprintText;

		public Text jumpText;

		public Text pauseText;
		public Text hintText;

		public Text lastWait;

		public bool inGame;
		
		void Start()
		{
			inGame = GameObject.Find("Main") != null;
		
			settings = new Settings();
		
			volumeSlider.value = settings.volume;
			changeVolume();

			fullscreen_on.SetActive(settings.fullscreen);
			fullscreen_off.SetActive(!settings.fullscreen);
			changeFullscreen(settings.fullscreen);

			//RESOLUTIONS
			List<Resolution> rs = new List<Resolution>();
			rs.AddRange(Screen.resolutions);

			resolutionsDico = new Dictionary<string, GameObject>();

			foreach (GameObject g in resolutions)
			{
				string width = g.name.Split('x')[0];
				string height = g.name.Split('x')[1];

				if (rs.TrueForAll(p => p.width + "" != width || p.height + "" != height))
					g.SetActive(false);
				else
				{
					Resolution r = rs.Find(p => p.width + "" == width && p.height + "" == height);

					resolutionsDico.Add(r.width + "x" + r.height, g);

					if (settings.resolutionW + "" == width && settings.resolutionH + "" == height)
					{
						//g.GetComponent<Image>().color = new Color(62, 255, 0);
						changeResolution(settings.resolutionW + "x" + settings.resolutionH);
					}
				}
			}
			//-------

			Vsync_on.SetActive(settings.vSync);
			Vsync_off.SetActive(!settings.vSync);
			changeVsync(settings.vSync);

			//SHADOW RESOLUTIONS
			shadowResolutionsDico = new Dictionary<ShadowResolution, GameObject>();

			foreach (GameObject g in shadowResolutions)
			{
				ShadowResolution res = getShadowResolution(g.name);
				shadowResolutionsDico.Add(res, g);
				if (res == QualitySettings.shadowResolution)
				{
					//g.GetComponent<Image>().color = new Color(62, 255, 0);
					changeShadowResolution(res);
				}
			}
			//------

			shadow_on.SetActive(settings.shadow);
			shadow_off.SetActive(!settings.shadow);
			changeShadow(settings.shadow);

			//ANTI ALIASING
			antiAliasingsDico = new Dictionary<int, GameObject>();

			foreach (GameObject g in antiAliasings)
			{
				int aa = Int32.Parse(g.name.Replace("x", ""));
				antiAliasingsDico.Add(aa, g);
				if (aa == QualitySettings.antiAliasing)
				{
					changeAntiAliasing(aa);
				}
			}
			//-------

			particlesReduction_on.SetActive(settings.particlesReduction);
			particlesReduction_off.SetActive(!settings.particlesReduction);
			changeParticlesReduction(settings.particlesReduction);

			fovSlider.value = settings.fov;
			if (Camera.main != null)
				changeFov();


			forwardText.text = settings.forward.ToString();
			leftText.text = settings.left.ToString();
			backwardText.text = settings.backward.ToString();
			rightText.text = settings.right.ToString();
			sprintText.text = settings.sprint.ToString();
			jumpText.text = settings.jump.ToString();
			pauseText.text = settings.pause.ToString();
			hintText.text = settings.hint.ToString();
			lastWait = null;
		}

		// Update is called once per frame
		void Update()
		{
			if (lastWait != null && Input.anyKeyDown)
			{
				foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
				{
					if (Input.GetKeyDown(vKey))
					{
						keyPressed(vKey);
						//Debug.Log(vKey.ToString());
						break;
					}
				}
			}
		}

		public void changeVolume()
		{
			int newVolume = (int)volumeSlider.value;
			AudioListener.volume = newVolume / 10.0f;
			settings.volume = newVolume;
			Save();
		}

		public void changeFullscreen(bool fullscreen)
		{
			Screen.fullScreen = fullscreen;
			settings.fullscreen = fullscreen;
			Save();
		}

		public void changeResolution(string resolution)
		{
			resolutionsDico[settings.resolutionW + "x" + settings.resolutionH].GetComponent<Image>().color = new Color(255, 255, 255);
			resolutionsDico[resolution].GetComponent<Image>().color = new Color(62, 255, 0);

			int width = Int32.Parse(resolution.Split('x')[0]);
			int height = Int32.Parse(resolution.Split('x')[1]);

			Screen.SetResolution(width, height, settings.fullscreen);

			settings.resolutionW = width;
			settings.resolutionH = height;
			Save();
		}

		public void changeVsync(bool vsync)
		{
			QualitySettings.vSyncCount = Convert.ToInt32(vsync);
			settings.vSync = vsync;
			Save();
		}

		public void changeShadowResolution(string resolution)
		{
			changeShadowResolution(getShadowResolution(resolution));
		}

		private void changeShadowResolution(ShadowResolution resolution)
		{
			QualitySettings.shadowResolution = resolution;

			shadowResolutionsDico[settings.shadowResolution].GetComponent<Image>().color = new Color(255, 255, 255);
			shadowResolutionsDico[resolution].GetComponent<Image>().color = new Color(62, 255, 0);

			settings.shadowResolution = resolution;

			Save();
		}

		private ShadowResolution getShadowResolution(string resolution)
		{
			ShadowResolution res = QualitySettings.shadowResolution;
			switch (resolution)
			{
				case "Low":
					res = ShadowResolution.Low;
					break;

				case "Medium":
					res = ShadowResolution.Medium;
					break;

				case "High":
					res = ShadowResolution.High;
					break;

				case "Very High":
					res = ShadowResolution.VeryHigh;
					break;
			}
			return res;
		}

		public void changeShadow(bool shadow)
		{
			if (shadow)
				QualitySettings.shadows = ShadowQuality.All;
			else
				QualitySettings.shadows = ShadowQuality.Disable;

			settings.shadow = shadow;
			Save();
		}

		public void changeAntiAliasing(int aa)
		{
			QualitySettings.antiAliasing = aa;

			antiAliasingsDico[settings.antiAliasing].GetComponent<Image>().color = new Color(255, 255, 255);
			antiAliasingsDico[aa].GetComponent<Image>().color = new Color(62, 255, 0);

			settings.antiAliasing = aa;

			Save();
		}

		public void changeParticlesReduction(bool particlesReduction)
		{
			QualitySettings.softParticles = particlesReduction;
			settings.particlesReduction = particlesReduction;
			Save();
		}

		public void changeFov()
		{
			int newFov = (int)fovSlider.value;
			if (Camera.main != null)
				Camera.main.fieldOfView = newFov;
			settings.fov = newFov;
			Save();
		}

		public void changeKey(Text text)
		{
			text.text = "Press a key";

			GameObject.FindWithTag("EventSystem").transform.GetChild(0).gameObject.SetActive(false);
			StartCoroutine(StartListen(text));
		}

		public IEnumerator StartListen(Text text)
		{
			yield return new WaitForSeconds(0.1f);
			lastWait = text;
		}

		private void keyPressed(KeyCode key)
		{
			Text text = lastWait;

			text.text = key.ToString();

			if (text == forwardText)
				settings.forward = key;

			else if (text == leftText)
				settings.left = key;

			else if (text == backwardText)
				settings.backward = key;

			else if (text == rightText)
				settings.right = key;

			else if (text == sprintText)
				settings.sprint = key;

			else if (text == jumpText)
				settings.jump = key;

			else if (text == pauseText)
				settings.pause = key;

			else if (text == hintText)
				settings.hint = key;

			Save();

			lastWait = null;
			GameObject.FindWithTag("EventSystem").transform.GetChild(0).gameObject.SetActive(true);
		}
		
		public void InitProgress()
		{
			PlayerPrefs.SetInt("STPlevel", 3);
			PlayerPrefs.Save();
		}


		public void Save()
		{
			if (inGame)
				Player.settings = settings;
			settings.SaveSettings();
		}
	}
}