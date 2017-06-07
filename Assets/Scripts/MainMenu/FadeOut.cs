using UnityEngine;
using UnityEngine.UI;
 
public class FadeOut : MonoBehaviour {
 
    public float fadeTime = 1f;
   
    void Start() {
        Graphic g = GetComponent<Graphic>();    // Text, Button, Image, etc.
        g.CrossFadeAlpha(0, fadeTime, true);
    }
}