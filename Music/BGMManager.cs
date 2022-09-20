using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace AF
{
    public class BGMManager : MonoBehaviour
    {
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

        private void Start()
        {
        }

        public void PlayMusic(AudioClip musicToPlay, float fadeAmount)
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
    }
}