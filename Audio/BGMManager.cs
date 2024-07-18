using UnityEngine;
using System.Collections;
using System.Linq;
using TigerForge;
using AF.Events;

namespace AF.Music
{
    public class BGMManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource bgmAudioSource;
        public AudioSource ambienceAudioSource;
        public AudioSource sfxAudioSource;

        [Header("Settings")]
        public GameSettings gameSettings;
        public float fadeMusicSpeed = .1f;

        [Header("Components")]
        public SceneSettings sceneSettings;

        // Internal
        Coroutine HandleMusicChangeCoroutine;
        Coroutine FadeInCoreCoroutine;
        Coroutine FadeOutCoreCoroutine;

        public float fadeDuration = 1f;

        // Flags
        public bool isPlayingBossMusic = false;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MUSIC_VOLUME_CHANGED, HandleVolume);
        }

        private void Start()
        {
            HandleVolume();
        }

        void HandleVolume()
        {
            bgmAudioSource.volume = gameSettings.GetMusicVolume();
            ambienceAudioSource.volume = gameSettings.GetMusicVolume();
        }

        public void PlayMusic(AudioClip musicToPlay)
        {
            if (this.bgmAudioSource.clip != null)
            {
                if (HandleMusicChangeCoroutine != null)
                {
                    StopCoroutine(HandleMusicChangeCoroutine);
                }

                HandleMusicChangeCoroutine = StartCoroutine(HandleMusicChange_Coroutine(musicToPlay));
            }
            else
            {
                // No music playing before, lets do a fade in
                this.bgmAudioSource.volume = 0;
                this.bgmAudioSource.clip = musicToPlay;
                this.bgmAudioSource.Play();

                if (FadeInCoreCoroutine != null)
                {
                    StopCoroutine(FadeInCoreCoroutine);
                }

                FadeInCoreCoroutine = StartCoroutine(FadeCore(false));
            }
        }

        IEnumerator HandleMusicChange_Coroutine(AudioClip musicToPlay)
        {
            yield return FadeCore(fadeOut: true);

            bgmAudioSource.clip = musicToPlay;
            bgmAudioSource.Play();

            yield return FadeCore(fadeOut: false);
        }

        IEnumerator FadeCore(bool fadeOut)
        {
            float targetVolume = fadeOut ? 0f : gameSettings.GetMusicVolume();
            float startVolume = bgmAudioSource.volume;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
                bgmAudioSource.volume = newVolume;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            bgmAudioSource.volume = targetVolume;

            if (fadeOut)
            {
                bgmAudioSource.Stop();
                bgmAudioSource.clip = null;
            }
        }

        void StopCoroutines()
        {
            if (FadeInCoreCoroutine != null)
            {
                StopCoroutine(FadeInCoreCoroutine);
            }

            if (HandleMusicChangeCoroutine != null)
            {
                StopCoroutine(HandleMusicChangeCoroutine);
            }

            if (FadeOutCoreCoroutine != null)
            {
                StopCoroutine(FadeOutCoreCoroutine);
            }
        }

        public void StopMusic()
        {
            StopCoroutines();

            if (this.bgmAudioSource.clip != null)
            {
                FadeOutCoreCoroutine = StartCoroutine(FadeCore(true));
            }
            else
            {
                this.bgmAudioSource.Stop();
                this.bgmAudioSource.clip = null;
            }
        }

        public void StopMusicImmediately()
        {
            this.bgmAudioSource.Stop();
            this.bgmAudioSource.clip = null;
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

        public void PlayMapMusicAfterKillingEnemy()
        {
            sceneSettings.HandleSceneSound(true);
        }

        public bool IsPlayingMusicClip(string clipName)
        {
            if (this.bgmAudioSource.clip == null)
            {
                return false;
            }

            if (this.bgmAudioSource.clip.name == clipName)
            {
                return true;
            }

            return false;
        }

        public bool IsNotPlayingMusic()
        {
            return this.bgmAudioSource.clip == null;
        }
    }
}
