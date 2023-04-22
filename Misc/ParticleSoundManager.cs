using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class ParticleSoundEntry
    {
        public AudioClip audio;
        public float timeToPlay;
    }

    public class ParticleSoundManager : MonoBehaviour
    {
        public ParticleSoundEntry[] soundEntries;

        AudioSource audioSource => GetComponent<AudioSource>();

        private void Start()
        {
            StartCoroutine(PlaySounds());
        }

        IEnumerator PlaySounds()
        {
            foreach (var audio in soundEntries)
            {
                yield return new WaitForSeconds(audio.timeToPlay);

                audioSource.PlayOneShot(audio.audio);
            }
        }

    }

}
