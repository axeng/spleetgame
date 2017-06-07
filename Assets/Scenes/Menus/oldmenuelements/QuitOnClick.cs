using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuitOnClick : MonoBehaviour
{

    public void Quit()
    {
		PlayerPrefs.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
    
    public void Exit()
    {
    	PlayerPrefs.Save();
    	SceneManager.LoadScene(1);
	}

}