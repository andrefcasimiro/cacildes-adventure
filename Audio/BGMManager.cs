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

                FadeInCoreCoroutine = StartCoroutine(FadeInCore_Coroutine());
            }
        }

        IEnumerator HandleMusicChange_Coroutine(AudioClip musicToPlay)
        {
            yield return FadeOutCore();

            yield return new WaitUntil(() => this.bgmAudioSource.volume <= 0);

            this.bgmAudioSource.clip = musicToPlay;
            this.bgmAudioSource.Play();

            yield return FadeInCore_Coroutine();
        }

        private IEnumerator FadeInCore_Coroutine()
        {
            var volumeInGamePreferences = gameSettings.GetMusicVolume();

            while (this.bgmAudioSource.volume < volumeInGamePreferences)
            {
                this.bgmAudioSource.volume += fadeMusicSpeed * Time.deltaTime;
                yield return null;
            }
        }
        private IEnumerator FadeOutCore()
        {
            while (this.bgmAudioSource.volume > 0)
            {
                this.bgmAudioSource.volume -= fadeMusicSpeed * Time.deltaTime;
                yield return null;
            }

            this.bgmAudioSource.Stop();
            this.bgmAudioSource.clip = null;
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
                FadeOutCoreCoroutine = StartCoroutine(FadeOutCore());
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
    }
}
