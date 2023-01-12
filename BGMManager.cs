using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Linq;

namespace AF
{
    public class BGMManager : MonoBehaviour
    {
        [Header("UI Sounds")]
        public AudioClip uiDecision;
        public AudioClip uiSelect;
        public AudioClip uiCancel;
        public AudioClip uiItemReceived;
        public AudioClip insufficientStamina;
        public AudioClip quickItemSwitch;

        [Header("Misc Sounds")]
        public AudioClip alert;
        public AudioClip bookFlip;
        public AudioClip cashRegister;
        public AudioClip coin;
        public AudioClip craftSuccess;
        public AudioClip craftError;
        public AudioClip gameOverFanfare;
        public AudioClip activateLever;
        public AudioClip openHeavyDoor;
        public AudioClip companionJoin;
        public AudioClip companionLeave;

        [Header("Audio Sources")]
        public AudioSource bgmAudioSource;
        public AudioSource ambienceAudioSource;
        public AudioSource sfxAudioSource;

        public static BGMManager instance;

        float maxSelectCooldownTimer = .2f;
        float selectCooldownTimer = Mathf.Infinity;

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

        private void Update()
        {
            if (selectCooldownTimer < maxSelectCooldownTimer)
            {
                selectCooldownTimer += Time.deltaTime;
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
        public void PlayUISelect() {
            if (selectCooldownTimer < maxSelectCooldownTimer)
            {
                return;
            }

            this.sfxAudioSource.PlayOneShot(uiSelect);
            selectCooldownTimer = 0;
        }
        public void PlayItem() { this.sfxAudioSource.PlayOneShot(uiItemReceived); }
        public void PlayBookFlip() { this.sfxAudioSource.PlayOneShot(bookFlip); }
        public void PlayCashRegister() { this.sfxAudioSource.PlayOneShot(cashRegister); }
        public void PlayCoin() { this.sfxAudioSource.PlayOneShot(coin); }
        public void PlayAlert() { this.sfxAudioSource.PlayOneShot(alert); }
        public void PlayCraftSuccess() { this.sfxAudioSource.PlayOneShot(craftSuccess); }
        public void PlayCraftError() { this.sfxAudioSource.PlayOneShot(craftError); }
        public void PlayGameOver() { this.sfxAudioSource.PlayOneShot(gameOverFanfare); }
        public void PlayLever() { this.sfxAudioSource.PlayOneShot(activateLever); }
        public void PlayHeavyDoor() { this.sfxAudioSource.PlayOneShot(openHeavyDoor); }
        public void PlayInsufficientStamina() { this.sfxAudioSource.PlayOneShot(insufficientStamina); }
        public void PlayQuickItemSwitch() { this.sfxAudioSource.PlayOneShot(quickItemSwitch); }
        public void PlayCompanionJoin() { this.sfxAudioSource.PlayOneShot(companionJoin); }
        public void PlayCompanionLeave() { this.sfxAudioSource.PlayOneShot(companionLeave); }

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

            if (activeEnemies.FirstOrDefault(x => x.IsInCombat() && x != killedEnemy))
            {
                return;
            }

            // Play map music
            FindObjectOfType<SceneSettings>(true).HandleSceneSound();

        }
    }
}
