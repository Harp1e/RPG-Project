using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class AudioRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] sfxClips = null;

        AudioSource audioSource;

        void Awake ()
        {
            audioSource = GetComponent<AudioSource> ();
        }

        public void PlayRandomSFX ()
        {
            audioSource.clip = sfxClips[Random.Range (0, sfxClips.Length)];
            audioSource.Play ();
        }
    }
}