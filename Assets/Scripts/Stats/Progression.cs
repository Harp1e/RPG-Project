using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu (fileName = "Progression", menuName = "RPG Project/Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat (Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup ();
            float[] levels = lookupTable[characterClass][stat];
            //Debug.Log ("Char Class: " + characterClass + " Stat: " + stat + " Lvls Length: " + levels.Length + " Level: " + level);
            if (levels.Length < level) { return 0f; }
            return levels[level -1];
        }

        public int GetLevels (Stat stat, CharacterClass characterClass)
        {
            BuildLookup ();
            return lookupTable[characterClass][stat].Length;
        }

        private void BuildLookup ()
        {
            if (lookupTable != null) { return; }

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>> ();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]> ();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }        
    }

    [System.Serializable]
    public class ProgressionCharacterClass
    {
        public CharacterClass characterClass;
        public ProgressionStat[] stats;
    }

    [System.Serializable]
    public class ProgressionStat
    {
        public Stat stat;
        public float[] levels;
    }
}
