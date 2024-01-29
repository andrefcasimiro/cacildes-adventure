using System.Collections;
using AF.Music;
using UnityEngine;

namespace AF
{

    public class EV_PlaySound : EventBase
    {
        [Header("Components")]
        public BGMManager bgmManager;
        public AudioSource audioSource;
        public AudioClip soundClip;

        public override IEnumerator Dispatch()
        {
            bgmManager.PlaySound(soundClip, audioSource);
            yield return null;
        }
    }
}
