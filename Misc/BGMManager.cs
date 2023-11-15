using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Linq;

namespace AF
{
    public class BGMManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource bgmAudioSource;
        public AudioSource ambienceAudioSource;
        public AudioSource sfxAudioSource;

        public static BGMManager instance;

        public float fadeMusicSpeed = .1f;

        SceneSettings sceneSettings;

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
            StopAllCoroutines();

            if (this.bgmAudioSource.clip != null)
            {
                StartCoroutine(HandleMusicChange(musicToPlay));
            }
            else
            {
                // No music playing before, lets do a fade in
                this.bgmAudioSource.volume = 0;
                this.bgmAudioSource.clip = musicToPlay;
                this.bgmAudioSource.Play();
                StartCoroutine(FadeInCore());
            }
        }
        public void PlayBattleMusic()
        {
            //            var battleMusic = sceneSettings.battleMusic;
            StopMusic();

        }

        IEnumerator HandleMusicChange(AudioClip musicToPlay)
        {
            yield return FadeOutCore();
            yield return new WaitUntil(() => this.bgmAudioSource.volume <= 0);

            this.bgmAudioSource.clip = musicToPlay;
            this.bgmAudioSource.Play();
            yield return FadeInCore();
        }

        public void StopMusic()
        {
            StopAllCoroutines();

            if (this.bgmAudioSource.clip != null)
            {
                StartCoroutine(FadeOutCore());
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

        #region Fade In and Out Core Logic
        private IEnumerator FadeInCore()
        {
            var volumeInGamePreferences = GamePreferences.instance.GetCurrentMusicVolume();
            if (volumeInGamePreferences <= 0)
            {
                yield break;
            }

            while (this.bgmAudioSource.volume < volumeInGamePreferences)
            {
                this.bgmAudioSource.volume += fadeMusicSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        private IEnumerator FadeOutCore()
        {
            var volumeInGamePreferences = GamePreferences.instance.GetCurrentMusicVolume();
            if (volumeInGamePreferences <= 0)
            {
                yield break;
            }

            while (this.bgmAudioSource.volume > 0)
            {
                this.bgmAudioSource.volume -= fadeMusicSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            this.bgmAudioSource.Stop();
            this.bgmAudioSource.clip = null;
        }
        #endregion

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

        public void PlayMapMusicAfterKillingEnemy(CharacterManager killedEnemy)
        {
            // Check if more enemies are in chase or combat state
            var activeEnemies = FindObjectsByType<CharacterManager>(FindObjectsSortMode.None);

            if (false) //activeEnemies.FirstOrDefault(x => x.enemyCombatController.IsInCombat() && x != killedEnemy))
            {
                return;
            }

            if (sceneSettings == null)
            {
                sceneSettings = FindFirstObjectByType<SceneSettings>(FindObjectsInactive.Include);
            }

            if (sceneSettings == null)
            {
                return;
            }

            // Play map music
            sceneSettings.HandleSceneSound();
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
