using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        const string currentSceneIndex = "currentSceneBuildIndex";

        public void Save (string saveFile)
        {
            Dictionary<string, object> state = LoadFile (saveFile);
            CaptureState (state);
            SaveFile (saveFile, state);
        }

        public void Load (string saveFile)
        {
            RestoreState (LoadFile (saveFile));
        }

        public IEnumerator LoadLastScene (string savefile)
        {
            Dictionary<string, object> state = LoadFile (savefile);
            int buildIndexToRestore = SceneManager.GetActiveScene ().buildIndex;
            if (state.ContainsKey(currentSceneIndex))
            {
                buildIndexToRestore = (int)state[currentSceneIndex];
            }
            yield return SceneManager.LoadSceneAsync (buildIndexToRestore);
            RestoreState (state);
        }

        Dictionary<string, object> LoadFile (string saveFile)
        {
            string path = GetPathFromSaveFile (saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object> ();
            }
            using (FileStream stream = File.Open (path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter ();
                return (Dictionary<string, object>)(formatter.Deserialize (stream));
            }
        }

        void SaveFile (string saveFile, object state)
        {
            string path = GetPathFromSaveFile (saveFile);
            Debug.Log ("Saving to: " + GetPathFromSaveFile (saveFile));
            using (FileStream stream = File.Open (path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter ();
                formatter.Serialize (stream, state);
            }
            //Debug.Log ("Saved to: " + GetPathFromSaveFile (saveFile));
        }

        void CaptureState (Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState ();
            }

            state[currentSceneIndex] = SceneManager.GetActiveScene ().buildIndex;
        }

        void RestoreState (Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity> ())
            {
                string id = saveable.GetUniqueIdentifier ();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState (state[id]);
                }
            }
        }

        string GetPathFromSaveFile (string saveFile)
        {
            return Path.Combine (Application.persistentDataPath, saveFile + ".sav");
        }

        public void ResetGame (string saveFile)
        {
            string path = GetPathFromSaveFile (saveFile);
            if (File.Exists (path))
            {
                File.Delete (path);
            }
            SceneManager.LoadScene (1);
        }

    }
}
