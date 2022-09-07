using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_PushObject : EventBase
    {
        public GameObject objectToPush;
        public int pushForce = 1;
        public int pushSpeed = 2;
        public float timeBeforePushOccurs = .5f;

        public Animator animator;
        public string pushingAnimation = "Pushing";
        public AudioClip pushingSfx;
        public AudioSource pushingSfxAudioSource;

        public override IEnumerator Dispatch()
        {
            yield return null;
                
            animator.CrossFade(pushingAnimation, 0.15f);
            yield return new WaitForSeconds(timeBeforePushOccurs);

            var t = 0f;
            var start = transform.position;
            var target = transform.position + animator.transform.forward * pushForce;

            if (pushingSfx != null)
            {
                BGMManager.instance.PlaySound(pushingSfx, pushingSfxAudioSource);
            }

            while (t < 1)
            {
                t += Time.deltaTime / pushSpeed;

                if (t > 1) t = 1;

                objectToPush.transform.position = Vector3.Lerp(start, target, t);

                yield return null;
            }
        }

    }
}
