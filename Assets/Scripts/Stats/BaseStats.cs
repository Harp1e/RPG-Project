using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        
        [SerializeField] [Range (1, 99)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect;

        public event Action OnLevelup;

        //Experience experience;
        int currentLevel = 0;

        void Start ()
        {
            Experience experience = GetComponent<Experience> ();
            currentLevel = CalculateLevel ();
            //Debug.Log (gameObject.name + " Base Stats - Awake - Curr level: " + currentLevel);

            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        void OnDisable ()
        {
            Experience experience = GetComponent<Experience> ();
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        void UpdateLevel ()
        {
            int newLevel = CalculateLevel ();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect ();
                OnLevelup ();
            }
        }

        private void LevelUpEffect ()
        {
            Instantiate (levelUpParticleEffect, transform);
        }

        public float GetStat (Stat stat)
        {
            //Debug.Log ("Base Stats - GetStat");
            return progression.GetStat (stat, characterClass, GetLevel ());
        }

        public int GetLevel ()
        {
            return currentLevel;
        }

        public int CalculateLevel ()
        {
            Experience experience = GetComponent<Experience> ();
            if (experience == null) { return startingLevel; }

            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels (Stat.ExperienceToLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat (Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    } 
}
