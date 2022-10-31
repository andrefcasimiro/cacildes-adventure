using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace AF
{
    public class BGMManager : MonoBehaviour
    {
        [Header("UI Sounds")]
        public AudioClip uiDecision;
        public AudioClip uiSelect;
        public AudioClip uiCancel;

        [Header("Misc Sounds")]
        public AudioClip alert;
        public AudioClip bookFlip;
        public AudioClip cash;

        [Header("Audio Sources")]
        public AudioSource bgmAudioSource;
        public AudioSource ambienceAudioSource;
        public AudioSource sfxAudioSource;

        public static BGMManager instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void PlayMusic(AudioClip musicToPlay)
        {
            this.bgmAudioSource.clip = musicToPlay;
            this.bgmAudioSource.Play();
        }

        public void StopMusic()
        {
            this.bgmAudioSource.Stop();
        }

        public void PlayAmbience(AudioClip ambience)
        {
            this.ambienceAudioSource.clip = ambience;
            this.ambienceAudioSource.Play();
        }

        public void StopAmbience()
        {
            this.ambienceAudioSource.clip = null;
            this.ambienceAudioSource.Stop();
        }

        public void PlaySound(AudioClip sfxToPlay, AudioSource customAudioSource)
        {
            if (customAudioSource != null)
            {
                customAudioSource.PlayOneShot(sfxToPlay);
                return;
            }


            this.sfxAudioSource.PlayOneShot(sfxToPlay);
        }

        public void PlaySoundWithPitchVariation(AudioClip sfxToPlay, AudioSource customAudioSource)
        {
            float pitch = UnityEngine.Random.Range(0.99f, 1.01f);
            customAudioSource.pitch = pitch;
            customAudioSource.PlayOneShot(sfxToPlay);
        }

        public void PlayUIDecision() { this.sfxAudioSource.PlayOneShot(uiDecision); }
        public void PlayUICancel() { this.sfxAudioSource.PlayOneShot(uiCancel); }
        public void PlayUISelect() { this.sfxAudioSource.PlayOneShot(uiSelect); }
        public void PlayBookFlip() { this.sfxAudioSource.PlayOneShot(bookFlip); }
        public void PlayCash() { this.sfxAudioSource.PlayOneShot(cash); }
        public void PlayAlert() { this.sfxAudioSource.PlayOneShot(alert); }
    }
}
