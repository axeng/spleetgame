using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	public MovieTexture IntroVideo;
	public MovieTexture VideoBoucle;

	public GameObject Menu;

	private bool playIntro;

	// Use this for initialization
	void Start () {
		GetComponent<RawImage>().texture = IntroVideo;
		IntroVideo.Play();
		playIntro = false;
		Menu.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		if (!playIntro && !IntroVideo.isPlaying)
		{
			playIntro = true;
			Menu.SetActive(true);
			GetComponent<RawImage>().texture = VideoBoucle;
			VideoBoucle.Play();
			VideoBoucle.loop = true;
		}
	}
}
