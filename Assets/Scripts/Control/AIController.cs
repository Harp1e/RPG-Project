using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCooldownTime = 3f;
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDelayTime = 5f;
        [SerializeField] float waypointDelayVariance = 1f;
        [SerializeField] int currentWaypointIndex = 0;       
        [SerializeField] [Range (0,1)] float patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 5f;

        GameObject player;
        Fighter fighter;
        Mover mover;
        Health health;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeAtWaypoint = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;

        void Awake ()
        {
            player = GameObject.FindWithTag ("Player");
            fighter = GetComponent<Fighter> ();
            mover = GetComponent<Mover> ();
            health = GetComponent<Health> ();
            guardPosition = new LazyValue<Vector3> (GetGuardPosition);
        }

        Vector3 GetGuardPosition ()
        {
            return transform.position;
        }

        void Start ()
        {
            guardPosition.ForceInit ();
        }

        void Update ()
        {
            if (health.IsDead ()) { return; }

            if (IsAggravated () && fighter.CanAttack (player))
            {                
                AttackBehaviour ();
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                SuspicionBehaviour ();
            }
            else
            {
                PatrolBehaviour ();
            }
            UpdateTimers ();
        }

        public void Aggravate ()
        {
            timeSinceAggravated = 0;
        }

        private void UpdateTimers ()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void PatrolBehaviour ()
        {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeAtWaypoint = 0f;
                    CycleWaypoint ();
                }
                nextPosition = GetCurrentWaypoint ();
            }
            if (timeAtWaypoint > waypointDelayTime + Random.Range(-waypointDelayVariance, waypointDelayVariance))
            {
                mover.StartMoveAction (nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWaypoint ()
        {
            float distanceToWaypoint = Vector3.Distance (transform.position, GetCurrentWaypoint ());
            return  distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint ()
        {
            currentWaypointIndex = patrolPath.GetNextIndex (currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint ()
        {
            return patrolPath.GetWaypoint (currentWaypointIndex);           
        }

        private void SuspicionBehaviour ()
        {
            GetComponent<ActionScheduler> ().CancelCurrentAction ();
        }

        private void AttackBehaviour ()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack (player);
            AggravateNearbyEnemies ();
        }

        void AggravateNearbyEnemies ()
        {
            RaycastHit[] hits = Physics.SphereCastAll (transform.position, shoutDistance, Vector3.up, 0f);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController> ();
                if (ai == null) { continue; }
                ai.Aggravate ();
            }
        }

        bool IsAggravated ()
        {
            float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);
            return distanceToPlayer <= chaseDistance || timeSinceAggravated < aggroCooldownTime;
        }

        void OnDrawGizmosSelected ()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere (transform.position, chaseDistance);
        }
    } 
}
