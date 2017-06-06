using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

	public MovieTexture IntroVideo;
	public MovieTexture AfterIntroVideo;
	public MovieTexture VideoBoucle;

	public GameObject Menu;

	private bool playIntro;
	private bool playAfterIntro;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.mainTexture = IntroVideo;
		IntroVideo.Play();
		playIntro = false;
		playAfterIntro = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!playIntro && !IntroVideo.isPlaying)
		{
			playIntro = true;
			GetComponent<Renderer>().material.mainTexture = AfterIntroVideo;
			AfterIntroVideo.Play();
		}

		if (playIntro && !playAfterIntro && !AfterIntroVideo.isPlaying)
		{
			playAfterIntro = true;
			GetComponent<Renderer>().material.mainTexture = VideoBoucle;
			VideoBoucle.Play();
			VideoBoucle.loop = true;
		}

		/*if (playIntro && playAfterIntro && !VideoBoucle.isPlaying)
		{
			GetComponent<Renderer>().material.mainTexture = VideoBoucle;
			VideoBoucle.Play();
		}*/
	}
}
