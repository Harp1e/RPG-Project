using GameDevTV.Utils;
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
        [SerializeField] bool shouldUseModifiers = false;

        public event Action OnLevelup;

        Experience experience;
        LazyValue<int> currentLevel;

        void Awake ()
        {
            experience = GetComponent<Experience> ();
            currentLevel = new LazyValue<int> (CalculateLevel);
        }

        void OnEnable ()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        void OnDisable ()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        void Start ()
        {
            currentLevel.ForceInit ();         
        }

        void UpdateLevel ()
        {
            int newLevel = CalculateLevel ();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
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
            return (GetBaseStat (stat) + GetAdditiveModifier (stat)) * (1f + GetPercentageModifier (stat) / 100f);
        }

        public int GetLevel ()
        {
            return currentLevel.value;
        }

        float GetAdditiveModifier (Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float total = 0;
            
            foreach (IModifierProvider provider in GetComponents<IModifierProvider> ())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        float GetPercentageModifier (Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider> ())
            {
                foreach (float modifier in provider.GetPercentageModifiers (stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        float GetBaseStat (Stat stat)
        {
            return progression.GetStat (stat, characterClass, GetLevel ());
        }

        int CalculateLevel ()
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
