using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class TransparencyTrigger : MonoBehaviour
    {
        Transform player;
        Ray ray;
        RaycastHit hit;
        TransparentFX fx;

        void Awake ()
        {
            player = GameObject.FindGameObjectWithTag ("Player").transform;
        }

        void Update ()
        {
            Vector3 direction = player.position - transform.position;
            ray = new Ray (transform.position, direction);
            if (Physics.Raycast (ray, out hit, Mathf.Infinity))
            {
                if (fx == null)
                {
                    fx = hit.collider.GetComponent<TransparentFX> ();
                    if (fx != null)
                    {
                        fx.SwitchMaterial (true);
                    }
                }
                else
                {
                    if (hit.collider.CompareTag ("Player"))
                    {
                        fx.SwitchMaterial (false);
                        fx = null;
                    }
                }
            }
        }
    } 
}
