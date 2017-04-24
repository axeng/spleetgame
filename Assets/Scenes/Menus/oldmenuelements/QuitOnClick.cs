using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuitOnClick : MonoBehaviour
{

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
    
    public void Exit()
    {
    	SceneManager.LoadScene(0);
	}

}