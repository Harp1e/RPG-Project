using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] float maxLifetime = 10f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyOnHit;
        [SerializeField] float lifeAfterImpact = 0.2f;

        Health target = null;
        float damage = 0;

        void Start ()
        {
            transform.LookAt (GetAimLocation ());
            Destroy (gameObject, maxLifetime);
        }

        void Update ()
        {
            if (target == null) { return; }
            if (isHoming && !target.IsDead)
            {
                transform.LookAt (GetAimLocation ());
            }

            transform.Translate (Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget (Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }


        Vector3 GetAimLocation ()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider> ();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height * 0.6f;
        }

        void OnTriggerEnter (Collider other)
        {
            if (other.GetComponent<Health> () != target) { return; }
            if (target.IsDead) { return; }

            target.TakeDamage (damage);
            speed = 0;

            if (hitEffect != null)
            {
                Instantiate (hitEffect, GetAimLocation (), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy (toDestroy);
            }
            Destroy (gameObject, lifeAfterImpact);
            
        }

        void OnBecameInvisible ()
        {
            Destroy (gameObject);
        }
    }
}
