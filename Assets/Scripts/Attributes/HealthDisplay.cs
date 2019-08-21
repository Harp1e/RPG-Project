using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Text healthValue;
        Health health;

        void Awake ()
        {
            health = GameObject.FindWithTag ("Player").GetComponent<Health> ();
            healthValue = GetComponent<Text> ();
        }

        void Update ()
        {
            healthValue.text = String.Format("{0:0}/{1:0}" ,
                health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    } 
}
