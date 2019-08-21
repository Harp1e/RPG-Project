//using RPG.Core;
using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination = DestinationIdentifier.A;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 1f;
        [SerializeField] [Range(0.01f, 1f)] float portalSpeedEffect = 1f;

        void OnTriggerEnter (Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine (Transition (other.gameObject));
            }
        }

        IEnumerator Transition (GameObject player)
        {
            if (sceneToLoad < 0) 
            {
                Debug.LogError ("PORTAL - Transition: SceneToLoad has not been set!");
                yield break;
            }

            DontDestroyOnLoad (gameObject);

            Fader fader = FindObjectOfType<Fader> ();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper> ();

            Time.timeScale = portalSpeedEffect;

            yield return fader.FadeOut (fadeOutTime);
            Time.timeScale = 1f;

            savingWrapper.Save ();
            yield return SceneManager.LoadSceneAsync (sceneToLoad);

            savingWrapper.Load ();

            Portal otherPortal = GetOtherPortal ();
            UpdatePlayer (otherPortal);

            savingWrapper.Save ();

            yield return new WaitForSeconds (fadeWaitTime);

            yield return fader.FadeIn (fadeInTime);

            Destroy (gameObject);
        }

        private void UpdatePlayer (Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag ("Player");
            player.GetComponent<NavMeshAgent> ().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent> ().enabled = true;
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
