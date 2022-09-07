using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class BGMManager : MonoBehaviour
    {
        public AudioSource bgmAudioSource;
        public AudioSource ambienceAudioSource;
        public AudioSource sfxAudioSource;

        [HideInInspector]
        public AudioClip currentMusic;
        [HideInInspector]
        public AudioClip previousMusic;

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

        public void PlayMusic(AudioClip musicToPlay)
        {
            this.previousMusic = this.currentMusic;
            this.currentMusic = musicToPlay;

            this.bgmAudioSource.clip = this.currentMusic;
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

        public void PlayPreviousMusic()
        {
            AudioClip musicToPlay = this.previousMusic;

            this.PlayMusic(musicToPlay);
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