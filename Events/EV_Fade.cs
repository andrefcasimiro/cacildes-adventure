using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_Fade : EventBase
    {
        public float duration = 1f;

        [Header("Components")]
        public FadeManager fadeManager;

        [Header("Unity Events")]
        public UnityEvent duringFadeTransitionsEventCallback;
        [Header("Settings")]
        public bool fadeIn = false;
        public bool fadeOut = false;

        [TextArea]
        public string comment;

        public override IEnumerator Dispatch()
        {
            if (fadeIn)
            {
                fadeManager.FadeIn(duration);
                yield return new WaitForSeconds(duration);
                yield break;
            }
            else if (fadeOut)
            {
                fadeManager.FadeOut(duration);
                yield return new WaitForSeconds(duration);
                yield break;
            }

            fadeManager.FadeIn(duration);
            yield return new WaitForSeconds(duration);
            fadeManager.FadeOut(1f);
            duringFadeTransitionsEventCallback?.Invoke();
        }
    }
}
