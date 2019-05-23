using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, ISaveable
    {
        [SerializeField] Weapon weapon;

        bool isEnabled = true;

        void OnTriggerEnter (Collider other)
        {
            if (other.tag == "Player")
            {
               other.GetComponent<Fighter> ().EquipWeapon (weapon);
                DisablePickup ();
            }            
        }

        void DisablePickup ()
        {
            GetComponent<SphereCollider> ().enabled = false;
            transform.GetChild (0).gameObject.SetActive (false);
            isEnabled = false;
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
                DisablePickup ();
            }
        }
    } 
}
