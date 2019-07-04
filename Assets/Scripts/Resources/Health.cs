using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] [Range(0, 100)] float regenerationPercentage = 70f;

        LazyValue <float> healthPoints;
        bool isDead = false;

        void Awake ()
        {
            healthPoints = new LazyValue<float> (GetInitialHealth);
        }

        float GetInitialHealth ()
        {
            return GetComponent<BaseStats> ().GetStat (Stat.Health);
        }

        void OnEnable ()
        {
            GetComponent<BaseStats> ().OnLevelup += RegenerateHealth;
        }

        void OnDisable ()
        {
            GetComponent<BaseStats> ().OnLevelup -= RegenerateHealth;
        }

        void Start ()
        {
            healthPoints.ForceInit ();
        }

        public bool IsDead ()
        {
            return isDead;
        }

        public void TakeDamage (GameObject instigator, float damage)
        {
            //Debug.Log (gameObject.name + " took damage: " + damage); 
            healthPoints.value = Mathf.Max (healthPoints.value - damage, 0);
            //damagePoints += damage;
            //damagePoints = Mathf.Clamp (damagePoints, 0, GetComponent<BaseStats> ().GetStat (Stat.Health));
            if (healthPoints.value <= 0)
            {
                Die ();
                AwardExperience (instigator);
            }
        }

        public float GetHealthPoints ()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints ()
        {
            return GetComponent<BaseStats> ().GetStat (Stat.Health);
        }

        public float GetPercentage ()
        {
            //float totalHealth = GetComponent<BaseStats> ().GetStat (Stat.Health);
            //return 100f * (1 - damagePoints / totalHealth);
            //Debug.Log ("Health - GetPercentage");
            return 100 * (healthPoints.value / GetComponent<BaseStats> ().GetStat (Stat.Health));
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
            healthPoints.value = Mathf.Max (healthPoints.value, regenHealthPoints);
        }
        
        public object CaptureState ()
        {
            return healthPoints;
            //return damagePoints;
        }

        public void RestoreState (object state)
        {
            healthPoints.value = (float)state;
            //damagePoints = (float)state;
            if (healthPoints.value <= 0)
            {
                Die ();
            }
        }
    }
}
