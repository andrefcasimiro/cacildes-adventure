using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EV_Fade : EventBase
    {
        public float duration = 1f;

        FadeManager fadeManager;

        private void Awake()
        {
            fadeManager = FindObjectOfType<FadeManager>(true);
        }

        public override IEnumerator Dispatch()
        {
            fadeManager.Fade();
            yield return new WaitForSeconds(duration);
        }

    }
}
