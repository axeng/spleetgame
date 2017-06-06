using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlay : MonoBehaviour {

	public MovieTexture movTexture;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer> ().material.mainTexture = movTexture;
		movTexture.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
