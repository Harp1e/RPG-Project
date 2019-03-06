using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        void Start ()
        {
            if (target == null)
            {
                target = FindObjectOfType<PlayerController> ().transform;
            }           
        }

        void LateUpdate ()
        {
            transform.position = target.position;
        }
    }
}