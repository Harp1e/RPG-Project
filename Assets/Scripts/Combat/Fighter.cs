using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Resources;
using RPG.Saving;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        //[SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        //Animator animator;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;

        //void Awake ()
        //{
        //    animator = GetComponent<Animator> ();
        //}

        void Start ()
        {
            if (currentWeapon == null)
            {
                EquipWeapon (defaultWeapon);
            }            
        }

        void Update ()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; }
            if (target.IsDead ()) { return; }

            if (!GetIsInRange ())
            {
                GetComponent<Mover> ().MoveTo (target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover> ().Cancel ();
                AttackBehaviour ();
            }
        }

        public void EquipWeapon (Weapon weapon)
        {
            if (weapon == null) { return; }
            
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator> ();
            weapon.Spawn (rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget ()
        {
            return target;
        } 
            
        private void AttackBehaviour ()
        {
            transform.LookAt (target.transform);
            if (timeSinceLastAttack > currentWeapon.GetTimeBetweenAttacks())
            {
                TriggerAttack ();
                timeSinceLastAttack = 0;
            }
        }

        void TriggerAttack ()
        {
            GetComponent<Animator> ().ResetTrigger ("stopAttack");
            GetComponent<Animator> ().SetTrigger ("attack");
        }

        // Animation Events
        void Hit ()
        {
            if (target)
            {
                if (currentWeapon.HasProjectile())
                {
                    currentWeapon.LaunchProjectile (rightHandTransform, leftHandTransform, target, gameObject);
                }
                else
                {
                    target.TakeDamage (gameObject, currentWeapon.GetDamage ());
                }
            }           
        }

        void Shoot ()
        {
            // Used by Bow Animation
            Hit ();
        }

        // Helper Functions
        private bool GetIsInRange ()
        {
            return Vector3.Distance (transform.position, target.transform.position) < currentWeapon.GetRange ();
        }

        public void Attack (GameObject combatTarget)
        {
            GetComponent<ActionScheduler> ().StartAction (this);
            target = combatTarget.transform.GetComponent<Health> ();
        }

        public bool CanAttack (GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }

            Health targetToTest = combatTarget.GetComponent<Health> ();
            return targetToTest != null && !targetToTest.IsDead ();
        }

        public void Cancel ()
        {
            StopAttack ();
            GetComponent<Mover> ().Cancel ();
            target = null;
        }

        private void StopAttack ()
        {
            GetComponent<Animator> ().ResetTrigger ("attack");
            GetComponent<Animator> ().SetTrigger ("stopAttack");
        }

        public object CaptureState ()
        {
            return currentWeapon.name;
        }

        public void RestoreState (object state)
        {
            Weapon weapon = UnityEngine.Resources.Load<Weapon> ((string)state);
            EquipWeapon (weapon);
        }
    } 
}
