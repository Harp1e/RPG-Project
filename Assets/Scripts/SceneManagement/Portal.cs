using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{  
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination = DestinationIdentifier.A;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 1f;

        void OnTriggerEnter (Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine (Transition ());
            }
        }

        IEnumerator Transition ()
        {
            if (sceneToLoad < 0) 
            {
                Debug.LogError ("SceneToLoad has not been set!");
                yield break;
            }

            DontDestroyOnLoad (gameObject);

            Fader fader = FindObjectOfType<Fader> ();
            yield return fader.FadeOut (fadeOutTime);

            yield return SceneManager.LoadSceneAsync (sceneToLoad);

            Portal otherPortal = GetOtherPortal ();
            UpdatePlayer (otherPortal);

            yield return new WaitForSeconds (fadeWaitTime);

            yield return fader.FadeIn (fadeInTime);

            Destroy (gameObject);
        }

        private void UpdatePlayer (Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag ("Player");
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal ()
        {
            Portal[] portals = GameObject.FindObjectsOfType<Portal> ();
            foreach (Portal portal in portals)
            {
                if (portal == this) { continue; }
                if (portal.destination != destination) { continue; }

                return portal;
            }
            return null;
        }
    } 
}
