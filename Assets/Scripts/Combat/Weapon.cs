﻿using System;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu (fileName = "New Weapon", menuName = "Weapons/Make New Weapon", order = 1)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public void Spawn (Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon (rightHand, leftHand);

            if (equippedPrefab != null)
            {
                Transform handTransform = GetHandTransform (rightHand, leftHand);
                GameObject weapon = Instantiate (equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon (Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find (weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find (weaponName);
            }
            if (oldWeapon == null) { return; }

            oldWeapon.name = "DESTROYING";
            Destroy (oldWeapon.gameObject);
        }

        public bool HasProjectile ()
        {
            return projectile != null;
        }

        public float GetTimeBetweenAttacks ()
        {
            return timeBetweenAttacks;
        }

        private Transform GetHandTransform (Transform rightHand, Transform leftHand)
        {
            return (isRightHanded) ? rightHand : leftHand;
        }

        public void LaunchProjectile (Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate 
                (projectile, GetHandTransform (rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget (target, weaponDamage);
        }

        public float GetRange ()
        {
            return weaponRange;
        }

        public float GetDamage ()
        {
            return weaponDamage;
        }
    } 
}