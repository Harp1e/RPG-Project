using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool alreadyTriggered = false;

        public object CaptureState ()
        {
            return alreadyTriggered;
        }

        public void RestoreState (object state)
        {
            alreadyTriggered = (bool)state;
        }

        void OnTriggerEnter (Collider other)
        {
            if (other.tag == "Player" && !alreadyTriggered)
            {
                alreadyTriggered = true;
                GetComponent<PlayableDirector> ().Play ();
            }
        }
    } 
}
