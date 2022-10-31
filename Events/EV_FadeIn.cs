using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_FadeIn : EventBase
    {
        public float fadeTime;

        public bool wait = false;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(Fade());
        }

        IEnumerator Fade()
        {
            FindObjectOfType<FadeCanvas>(true).FadeIn(fadeTime);
            yield return wait ? new WaitForSeconds(fadeTime) : null;
        }
    }

}
