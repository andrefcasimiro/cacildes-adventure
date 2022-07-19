using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXManager : MonoBehaviour
    {
        AudioSource audioSource => GetComponent<AudioSource>();

        public void PlaySound(AudioClip sfxToPlay, AudioSource customAudioSource)
        {
            if (customAudioSource != null)
            {
                customAudioSource.PlayOneShot(sfxToPlay);
                return;
            }

            this.audioSource.PlayOneShot(sfxToPlay);
        }

    }
}