using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class TransparencyTriggerV2 : MonoBehaviour
    {
        Transform player;
        Ray ray;
        RaycastHit[] hits;
        List<TransparentFX> fxs = new List<TransparentFX> ();
        LayerMask layerMask;

        void Awake ()
        {
            layerMask = 1 << 1;
            player = GameObject.FindGameObjectWithTag ("Player").transform;
        }

        void Update ()
        {
            Vector3 direction = player.position - transform.position;
            ray = new Ray (transform.position, direction);

            hits = Physics.RaycastAll (ray, Mathf.Infinity, layerMask);
            ClearAllFX ();

            if (hits != null)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    TransparentFX fx = hits[i].collider.GetComponent<TransparentFX> ();                    
                    if (fx != null)
                    {
                        fxs.Add (fx);
                        fxs[i].SwitchMaterial (true);
                    }
                }
            }
        }

        void ClearAllFX ()
        {
            if (fxs == null) { return; }

            foreach (TransparentFX fX in fxs)
            {
                if (fX != null)
                {
                    fX.SwitchMaterial (false);
                }
            }
            fxs = new List<TransparentFX> ();
        }
    } 
}
