using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] [Range(0, 100)] float regenerationPercentage = 70f;

        float healthPoints = -1;
        //float damagePoints = 0f;
        bool isDead = false;        

        void Start ()
        {
            GetComponent<BaseStats> ().OnLevelup += RegenerateHealth;
            if (healthPoints < 0)
            {
                //Debug.Log (gameObject.name + "Health Start");
                healthPoints = GetComponent<BaseStats> ().GetStat (Stat.Health);
            }
        }

        public bool IsDead ()
        {
            return isDead;
        }

        public void TakeDamage (GameObject instigator, float damage)
        {
            healthPoints = Mathf.Max (healthPoints - damage, 0);
            //damagePoints += damage;
            //damagePoints = Mathf.Clamp (damagePoints, 0, GetComponent<BaseStats> ().GetStat (Stat.Health));
            if (healthPoints <= 0)
            {
                Die ();
                AwardExperience (instigator);
            }
        }

        public float GetPercentage ()
        {
            //float totalHealth = GetComponent<BaseStats> ().GetStat (Stat.Health);
            //return 100f * (1 - damagePoints / totalHealth);
            //Debug.Log ("Health - GetPercentage");
            return 100 * (healthPoints / GetComponent<BaseStats> ().GetStat (Stat.Health));
        }

        //bool ShouldDie ()
        //{
        //    float totalHealth = GetComponent<BaseStats> ().GetStat (Stat.Health);
        //    return damagePoints >= totalHealth; 
        //}

        void Die ()
        {
            if (isDead) { return; }
            isDead = true;
            GetComponent<Animator> ().SetTrigger ("die");
            GetComponent<ActionScheduler> ().CancelCurrentAction ();
        }

        void AwardExperience (GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience> ();
            if (experience == null) { return; }

            experience.GainExperience (GetComponent<BaseStats> ().GetStat (Stat.ExperienceReward));
        }

        void RegenerateHealth ()
        {
            //Debug.Log ("Health - RegenerateHealth");
            float regenHealthPoints = 
                GetComponent<BaseStats> ().GetStat (Stat.Health) * (regenerationPercentage / 100f);
            //float regenDamagePoints = GetComponent<BaseStats> ().GetStat (Stat.Health) - regenHealthPoints;
            //damagePoints = Mathf.Min (damagePoints, regenDamagePoints);
            healthPoints = Mathf.Max (healthPoints, regenHealthPoints);
        }
        
        public object CaptureState ()
        {
            return healthPoints;
            //return damagePoints;
        }

        public void RestoreState (object state)
        {
            healthPoints = (float)state;
            //damagePoints = (float)state;
            if (healthPoints <= 0)
            {
                Die ();
            }
        }
    }
}
