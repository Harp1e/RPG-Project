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

        void Start ()
        {
            player = GameObject.FindGameObjectWithTag ("Player").transform;
        }

        void Update ()
        {
            ray = new Ray (transform.position, (player.position - transform.position));
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
