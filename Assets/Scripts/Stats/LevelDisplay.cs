using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        Text levelValue;
        BaseStats baseStats;

        void Awake ()
        {
            baseStats = GameObject.FindWithTag ("Player").GetComponent<BaseStats> ();
            levelValue = GetComponent<Text> ();
        }

        void Update ()
        {
            levelValue.text = baseStats.GetLevel ().ToString ();
        }
    } 
}
