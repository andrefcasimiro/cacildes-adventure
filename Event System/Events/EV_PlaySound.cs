using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_PlaySound : EventBase
    {
        public AudioSource audioSource;
        public AudioClip soundClip;

        SFXManager sfxManager;

        private void Awake()
        {
            sfxManager = FindObjectOfType<SFXManager>(true);
        }

        public override IEnumerator Dispatch()
        {
            yield return null;

            if (soundClip != null && sfxManager != null)
            {
                sfxManager.PlaySound(soundClip, audioSource);
            }
        }
    }

}
