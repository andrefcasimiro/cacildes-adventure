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

            BGMManager.instance.PlaySound(soundClip, audioSource);

            yield return null;
        
        }
    }

}
