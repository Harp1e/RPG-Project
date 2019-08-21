using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6f;

        NavMeshAgent agent;
        Health health;

        void Awake ()
        {
            health = GetComponent<Health> ();
            agent = GetComponent<NavMeshAgent> ();
        }

        void Update () 
	    {            
            agent.enabled = !health.IsDead ();
            UpdateAnimator ();
	    }

        public void StartMoveAction (Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler> ().StartAction (this);
            MoveTo (destination, speedFraction);
        }
	
        public void MoveTo (Vector3 destination, float speedFraction)
        {
            agent.speed = maxSpeed * Mathf.Clamp01 (speedFraction);
            agent.destination = destination;
            agent.isStopped = false;           
        }

        public void Cancel ()
        {
            agent.isStopped = true;
        }
        
        void UpdateAnimator ()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection (velocity);
            float speed = localVelocity.z;
            GetComponent<Animator> ().SetFloat ("forwardSpeed", speed);
        }

        public object CaptureState ()
        {
            return new SerializableVector3 (transform.position);
        }

        public void RestoreState (object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            agent.enabled = false;
            transform.position = position.ToVector ();
            agent.enabled = true;     // OR
            //agent.Move (position.ToVector ());
            GetComponent<ActionScheduler> ().CancelCurrentAction ();
        }
    }
}