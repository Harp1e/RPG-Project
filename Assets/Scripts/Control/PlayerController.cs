using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{ 
    public class PlayerController : MonoBehaviour 
    {
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;

        LayerMask layerMask;
        Health health;

        void Awake ()
        {
            health = GetComponent<Health> ();
            layerMask = (1 << 1) | (1 << 2);
            layerMask = ~layerMask;
        }

        void Update ()
        {
            if (InteractWithUI()) { return; }
            if (health.IsDead ())
            {
                SetCursor (CursorType.NONE);
                return;
            }
            if (InteractWithComponent ()) { return; }
            if (InteractWithMovement ()) { return; }

            SetCursor (CursorType.NONE);
        }

        private bool InteractWithUI ()
        {
            if (EventSystem.current.IsPointerOverGameObject ())
            {
                SetCursor (CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent ()
        {
            RaycastHit[] hits = RaycastAllSorted ();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable> ();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor (raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted ()
        {
            RaycastHit[] hits = Physics.SphereCastAll (GetMouseRay (), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort (distances, hits);
            return hits;
        }

        private bool InteractWithMovement ()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh (out target);           
            if (hasHit)
            {
                if (!GetComponent<Mover> ().CanMoveTo (target)) { return false; }

                if (Input.GetMouseButton (0))
                {
                    GetComponent<Mover> ().StartMoveAction (target, 1f);
                }
                SetCursor (CursorType.MOVEMENT);
                return true;
            }
            return false;
        }

        bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3 ();
            RaycastHit hit;
            bool hasHit = Physics.Raycast (GetMouseRay (), out hit, Mathf.Infinity, layerMask);
            if (!hasHit) { return false; }

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition (
                hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) { return false; }

            target = navMeshHit.position;
            return true;
        }

        void SetCursor (CursorType type)
        {
            CursorMapping mapping = GetCursorMapping (type);
            Cursor.SetCursor (mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping (CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            Debug.LogWarning ("PLAYER CONTROLLER - GetCursorMapping: Undefined Cursor Affordance type: " + type.ToString ());
            return cursorMappings[0];
        }

        private static Ray GetMouseRay ()
        {
            return Camera.main.ScreenPointToRay (Input.mousePosition);
        }
    }
}