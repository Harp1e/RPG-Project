using RPG.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Text healthValue;
        Fighter fighter;

        void Awake ()
        {
            fighter = GameObject.FindWithTag ("Player").GetComponent<Fighter> ();
            healthValue = GetComponent<Text> ();
        }

        void Update ()
        {
            if (fighter.GetTarget () == null)
            {
                healthValue.text = "N/A";
                return;                
            }

            Health health = fighter.GetTarget ();
            healthValue.text = String.Format ("{0:0}%", health.GetPercentage ());
        }
    } 
}
