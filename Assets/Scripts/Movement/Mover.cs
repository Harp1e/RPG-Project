using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction 
    {
        NavMeshAgent agent;

        void Start ()
        {
            agent = GetComponent<NavMeshAgent> ();
        }

        void Update () 
	    {
            UpdateAnimator ();
	    }

        public void StartMoveAction (Vector3 destination)
        {
            GetComponent<ActionScheduler> ().StartAction (this);
            MoveTo (destination);
        }
	
        public void MoveTo (Vector3 destination)
        {
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
    }
}