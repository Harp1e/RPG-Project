using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] float maxLifetime = 10f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 0.2f;
        [SerializeField] UnityEvent onhit = null;

        Health target = null;
        GameObject instigator;

        float damage = 0;

        void Start ()
        {
            transform.LookAt (GetAimLocation ());
        }

        void Update ()
        {
            if (target == null) { return; }
            if (isHoming && !target.IsDead ())
            {
                transform.LookAt (GetAimLocation ());
            }

            transform.Translate (Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget (Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy (gameObject, maxLifetime);
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
            if (target.IsDead ()) { return; }

            target.TakeDamage (instigator, damage);
            speed = 0;

            onhit.Invoke ();

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
