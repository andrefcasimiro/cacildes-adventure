using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_PlaySound : EventBase
    {
        public AudioSource audioSource;
        public AudioClip soundClip;

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (soundClip != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(soundClip);
                }
                else
                {
                    BGMManager.instance.PlaySound(soundClip, audioSource);
                }
            }
        }
    }

}
