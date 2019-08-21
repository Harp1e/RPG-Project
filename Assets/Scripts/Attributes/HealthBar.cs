using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground = null;

        Canvas rootCanvas = null;
        Health healthComponent = null;

        void Awake ()
        {
            rootCanvas = GetComponentInChildren<Canvas> ();
            healthComponent = GetComponentInParent<Health> ();
        }

        void Update ()
        {
            float health = healthComponent.GetFraction ();
            if (Mathf.Approximately (health, 0f) || Mathf.Approximately (health, 1f))
            {
                rootCanvas.enabled = false;
                return;
            }
            rootCanvas.enabled = true;
            foreground.localScale = new Vector3 (health, 1f, 1f);
        }
    } 
}
