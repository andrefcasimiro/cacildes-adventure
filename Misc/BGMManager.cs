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

        public void PlayBattleMusic()
        {
            var battleMusic = FindObjectOfType<SceneSettings>(true).battleMusic;

            if (battleMusic != null)
            {

                if (this.bgmAudioSource.clip != null && this.bgmAudioSource.clip.name == battleMusic.name)
                {
                    return;
                }


                PlayMusic(battleMusic);
            }
        }

        public void PlayMapMusicAfterKillingEnemy(EnemyManager killedEnemy)
        {
            // Check if more enemies are in chase or combat state
            var activeEnemies = FindObjectsOfType<EnemyManager>();

            if (activeEnemies.FirstOrDefault(x => x.enemyCombatController.IsInCombat() && x != killedEnemy))
            {
                return;
            }

            // Play map music
            FindObjectOfType<SceneSettings>(true).HandleSceneSound();
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
