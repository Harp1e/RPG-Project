using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        //[SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        Animator animator;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        void Awake ()
        {
            animator = GetComponent<Animator> ();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon> (SetupDefaultWeapon);
        }

        Weapon SetupDefaultWeapon ()
        {            
            return AttachWeapon (defaultWeapon);
        }

        void Start ()
        {
            currentWeapon.ForceInit ();
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

        public void EquipWeapon (WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon (weapon);
        }

        private Weapon AttachWeapon (WeaponConfig weapon)
        {
            //Animator animator = GetComponent<Animator> ();
            return weapon.Spawn (rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget ()
        {
            return target;
        } 
            
        private void AttackBehaviour ()
        {
            transform.LookAt (target.transform);
            if (timeSinceLastAttack > currentWeaponConfig.GetTimeBetweenAttacks())
            {
                TriggerAttack ();
                timeSinceLastAttack = 0;
            }
        }

        void TriggerAttack ()
        {
            //GetComponent<Animator> ().ResetTrigger ("stopAttack");
            //GetComponent<Animator> ().SetTrigger ("attack");
            animator.ResetTrigger ("stopAttack");
            animator.SetTrigger ("attack");
        }

        // Animation Events
        void Hit ()
        {
            if (target)
            {
                float damage = GetComponent<BaseStats> ().GetStat (Stat.Damage);

                if (currentWeapon.value != null)
                {
                    currentWeapon.value.OnHit ();
                }

                if (currentWeaponConfig.HasProjectile())
                {
                    currentWeaponConfig.LaunchProjectile (
                        rightHandTransform, leftHandTransform, target, gameObject, damage);
                }
                else
                {
                    target.TakeDamage (gameObject, damage);
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
            return Vector3.Distance (transform.position, target.transform.position) < 
                currentWeaponConfig.GetRange ();
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
            //GetComponent<Animator> ().ResetTrigger ("attack");
            //GetComponent<Animator> ().SetTrigger ("stopAttack");
            animator.ResetTrigger ("attack");
            animator.SetTrigger ("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers (Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage ();
            }
        }

        public IEnumerable<float> GetPercentageModifiers (Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus ();
            }
        }

        public object CaptureState ()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState (object state)
        {
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig> ((string)state);
            EquipWeapon (weapon);
        }
    } 
}
