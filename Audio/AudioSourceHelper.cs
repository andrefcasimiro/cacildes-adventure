using UnityEngine;

namespace AF
{
    public class AudioSourceHelper : MonoBehaviour
    {
        public AudioSource audioSource;

        public float minPitchVariation = .95f;
        public float maxPitchVariation = 1.05f;

        public AudioClip[] audioClips;

        public void SafePlay()
        {
            audioSource.pitch = Random.Range(minPitchVariation, maxPitchVariation);
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        }
    }
}
