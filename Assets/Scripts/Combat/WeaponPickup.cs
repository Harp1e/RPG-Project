using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, ISaveable
    {
        [SerializeField] Weapon weapon;
        [SerializeField] float respawnTime = 5f;
        bool isEnabled = true;

        void OnTriggerEnter (Collider other)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<Fighter> ().EquipWeapon (weapon);
                StartCoroutine (HideForSeconds (respawnTime));
            }            
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
    } 
}
