using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    [System.Serializable]
    public class TakeDamageEvent : UnityEvent<float> { }

    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] [Range(0, 100)] float regenerationPercentage = 70f;
        [SerializeField] TakeDamageEvent takeDamage = null;
        [SerializeField] UnityEvent onDie = null;
        
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
            healthPoints.value = Mathf.Max (healthPoints.value - damage, 0);

            if (healthPoints.value <= 0)
            {
                onDie.Invoke ();
                Die ();
                AwardExperience (instigator);
            }
            else
            {
                takeDamage.Invoke (damage);
            }
        }

        public void Heal (float healthToRestore)
        {
            healthPoints.value = Mathf.Min (healthPoints.value + healthToRestore, GetMaxHealthPoints ());
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
            return 100 * GetFraction ();
        }

        public float GetFraction ()
        {
            return healthPoints.value / GetComponent<BaseStats> ().GetStat (Stat.Health);
        }

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
            float regenHealthPoints = 
                GetComponent<BaseStats> ().GetStat (Stat.Health) * (regenerationPercentage / 100f);
            healthPoints.value = Mathf.Max (healthPoints.value, regenHealthPoints);
        }
        
        public object CaptureState ()
        {
            return healthPoints.value;
        }

        public void RestoreState (object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value <= 0)
            {
                Die ();
            }
        }
    }
}
