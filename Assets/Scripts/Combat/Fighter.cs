﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;

        Health target;
        float timeSinceLastAttack = 0f;

        void Update ()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; }
            if (target.IsDead) { return; }

            if (!GetIsInRange ())
            {
                GetComponent<Mover> ().MoveTo (target.transform.position);
            }
            else
            {
                GetComponent<Mover> ().Cancel ();
                AttackBehaviour ();
            }
        }

        private void AttackBehaviour ()
        {
            transform.LookAt (target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
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

        // Animation Event
        void Hit ()
        {
            if (target)
            {
                target.TakeDamage(weaponDamage);
            }
           
        }

        private bool GetIsInRange ()
        {
            return Vector3.Distance (transform.position, target.transform.position) < weaponRange;
        }

        public void Attack (CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler> ().StartAction (this);
            target = combatTarget.transform.GetComponent<Health> ();
        }

        public bool CanAttack (CombatTarget combatTarget)
        {
            if (combatTarget == null) { return false; }

            Health targetToTest = combatTarget.GetComponent<Health> ();
            return targetToTest != null && !targetToTest.IsDead;
        }

        public void Cancel ()
        {
            StopAttack ();
            target = null;
        }

        private void StopAttack ()
        {
            GetComponent<Animator> ().ResetTrigger ("attack");
            GetComponent<Animator> ().SetTrigger ("stopAttack");
        }
    } 
}
