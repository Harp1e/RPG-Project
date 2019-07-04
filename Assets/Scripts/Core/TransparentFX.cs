using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Core
{
    public class TransparentFX : MonoBehaviour
    {
        [SerializeField] Material transparentMaterial;

        Material[] originals;
        MeshRenderer[] meshRenderers;

        void Awake ()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer> ();
            originals = new Material[meshRenderers.Length];

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                originals[i] = meshRenderers[i].material;
            }
        }

        public void SwitchMaterial (bool makeTransparent)
        {
            if (makeTransparent)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material = transparentMaterial;
                }
            }
            else
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material = originals[i];
                }
            }
        }
    } 
}
