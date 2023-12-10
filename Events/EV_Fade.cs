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

        public override IEnumerator Dispatch()
        {
            fadeManager.FadeIn(duration);
            yield return new WaitForSeconds(duration);
            fadeManager.FadeOut(1f);
            duringFadeTransitionsEventCallback?.Invoke();
        }
    }
}
