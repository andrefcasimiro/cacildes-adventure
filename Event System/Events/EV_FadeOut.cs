using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_FadeOut : EventBase
    {
        public float fadeTime;

        public bool wait;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(Fade());
        }

        IEnumerator Fade()
        {
            FindObjectOfType<FadeCanvas>(true).FadeOut(fadeTime);

            yield return wait ? new WaitForSeconds(fadeTime) : null;
        }
    }

}
