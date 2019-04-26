using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Menu
{
    public class Menu : MonoBehaviour
    {
        public void NewGame ()
        {
            string path = Path.Combine (Application.persistentDataPath, "save.sav");
            File.Delete (path);
            SceneManager.LoadScene (1);
        }

        public void ResumeGame ()
        {
            SceneManager.LoadScene (1);
        }

        public void QuitGame ()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
	        Application.Quit();
        #endif  
        }
    } 
}
