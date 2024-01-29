using System.Collections;
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

        public AudioSource audioSource;

        public void PlaySounds()
        {
            IEnumerator PlaySounds_Coroutine()
            {
                foreach (var audio in soundEntries)
                {
                    yield return new WaitForSeconds(audio.timeToPlay);

                    audioSource.PlayOneShot(audio.audio);
                }
            }

            StartCoroutine(PlaySounds_Coroutine());
        }
    }
}
