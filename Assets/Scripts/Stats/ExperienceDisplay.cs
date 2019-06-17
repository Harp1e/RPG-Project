using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Text experienceValue;
        Experience experience;

        void Awake ()
        {
            experience = GameObject.FindWithTag ("Player").GetComponent<Experience> ();
            experienceValue = GetComponent<Text> ();
        }

        void Update ()
        {
            experienceValue.text = experience.GetPoints().ToString ();
        }
    } 
}
