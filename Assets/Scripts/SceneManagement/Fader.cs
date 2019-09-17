using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup = null;
        Coroutine currentActiveFade = null;

        void Awake ()
        {
            canvasGroup = GetComponent<CanvasGroup> ();
        }

        public void FadeOutImmediate ()
        {
            canvasGroup.alpha = 1f;
        }

        public Coroutine FadeOut (float time)
        {
            return Fade (1f, time);
        }

        public Coroutine FadeIn (float time)
        {
            return Fade (0f, time);
        }

        Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine (currentActiveFade);
            }
            currentActiveFade = StartCoroutine (FadeRoutine (target, time));
            return currentActiveFade;
        }

        IEnumerator FadeRoutine (float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(
                    canvasGroup.alpha, 
                    target, 
                    Time.deltaTime / time / Time.timeScale
                    );
                yield return null;
            }
        }
    } 
}
