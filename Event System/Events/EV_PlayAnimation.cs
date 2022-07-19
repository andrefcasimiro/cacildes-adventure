using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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

            animator.CrossFade(animationName, crossFadeTime);

            yield return null;

        }
    }
}