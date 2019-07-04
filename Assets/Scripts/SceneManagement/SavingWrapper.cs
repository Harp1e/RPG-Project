using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField] float IntialFadeTime = 2f;

        void Awake ()
        {
            StartCoroutine (LoadLastScene ());
        }

        IEnumerator LoadLastScene ()
        {            
            yield return StartCoroutine (GetComponent<SavingSystem> ().LoadLastScene (defaultSaveFile));
            Fader fader = FindObjectOfType<Fader> ();
            fader.FadeOutImmediate ();
            yield return fader.FadeIn (IntialFadeTime);
        }

        void Update ()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save ();
            }

            if (Input.GetKeyDown (KeyCode.L))
            {
                Load ();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteProfile ();
            }
        }

        public void Load ()
        {
            GetComponent<SavingSystem> ().Load (defaultSaveFile);
        }

        public void Save ()
        {
            GetComponent<SavingSystem> ().Save (defaultSaveFile);
        }

        void DeleteProfile ()
        {
            GetComponent<SavingSystem> ().ResetGame (defaultSaveFile);
            Debug.Log ("Save file Deleted");
        }
    }
}
