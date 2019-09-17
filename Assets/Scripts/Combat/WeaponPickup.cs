using RPG.Attributes;
using RPG.Control;
using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, ISaveable, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0f;
        [SerializeField] float respawnTime = 5f;

        bool isEnabled = true;

        void OnTriggerEnter (Collider other)
        {
            if (other.tag == "Player")
            {
                Pickup (other.gameObject);
            }
        }

        private void Pickup (GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<Fighter> ().EquipWeapon (weapon);
            }
            if (healthToRestore > 0)
            {
                subject.GetComponent<Health> ().Heal (healthToRestore);
            }
            StartCoroutine (HideForSeconds (respawnTime));
        }

        public object CaptureState ()
        {
            return isEnabled;
        }

        public void RestoreState (object state)
        {
            isEnabled = (bool)state;
            if (!isEnabled)
            {
                StartCoroutine (HideForSeconds (respawnTime));
            }
        }

        IEnumerator HideForSeconds (float seconds)
        {
            ShowPickup (false);
            yield return new WaitForSeconds (seconds);
            ShowPickup (true);
        }

        private void ShowPickup (bool shouldShow)
        {
            GetComponent<Collider> ().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive (shouldShow);
            }
            isEnabled = shouldShow;
        }

        public bool HandleRaycast (PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup (callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType ()
        {
            return CursorType.PICKUP;
        }
    } 
}
