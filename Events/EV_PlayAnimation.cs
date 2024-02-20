using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_PlayAnimation : EventBase
    {
        public Animator animator;

        public string animationName;

        public float crossFadeTime = 0.1f;

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (crossFadeTime <= 0)
            {
                animator.Play(animationName);
            }
            else
            {
                animator.CrossFade(animationName, crossFadeTime);
            }

            yield return null;

        }
    }
}
