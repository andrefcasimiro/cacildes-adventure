using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_Fade : EventBase
    {
        public float duration = 1f;


        [Header("Unity Events")]
        public UnityEvent duringFadeTransitionsEventCallback;
        [Header("Settings")]
        public bool fadeIn = false;
        public bool fadeOut = false;

        [TextArea]
        public string comment;

        // Scene Refs    
        FadeManager fadeManager;

        public override IEnumerator Dispatch()
        {
            if (fadeIn)
            {
                GetFadeManager().FadeIn(duration);
                yield return new WaitForSeconds(duration);
                yield break;
            }
            else if (fadeOut)
            {
                GetFadeManager().FadeOut(duration);
                yield return new WaitForSeconds(duration);
                yield break;
            }

            GetFadeManager().FadeIn(duration);
            yield return new WaitForSeconds(duration);
            GetFadeManager().FadeOut(1f);
            duringFadeTransitionsEventCallback?.Invoke();
        }

        FadeManager GetFadeManager()
        {
            if (fadeManager == null)
            {
                fadeManager = FindAnyObjectByType<FadeManager>(FindObjectsInactive.Include);
            }

            return fadeManager;
        }
    }
}
