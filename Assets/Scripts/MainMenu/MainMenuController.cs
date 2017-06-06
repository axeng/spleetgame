using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

	public MovieTexture IntroVideo;
	public MovieTexture VideoBoucle;

	public GameObject Menu;

	private bool playIntro;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.mainTexture = IntroVideo;
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
			GetComponent<Renderer>().material.mainTexture = VideoBoucle;
			VideoBoucle.Play();
			VideoBoucle.loop = true;
		}
	}
}
