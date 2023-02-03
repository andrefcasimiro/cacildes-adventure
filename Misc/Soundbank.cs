using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class Soundbank : MonoBehaviour
    {


        [Header("UI Sounds")]
        public AudioClip uiDecision;
        public AudioClip uiHover;
        public AudioClip uiCancel;
        public AudioClip uiItemReceived;
        public AudioClip insufficientStamina;
        public AudioClip quickItemSwitch;

        [Header("Movement")]
        public AudioClip cloth;
        public AudioClip dodge;
        public AudioClip landing;

        [Header("Misc Sounds")]
        public AudioClip alert;
        public AudioClip bookFlip;
        public AudioClip coin;
        public AudioClip craftSuccess;
        public AudioClip craftError;
        public AudioClip gameOverFanfare;
        public AudioClip activateLever;
        public AudioClip openHeavyDoor;
        public AudioClip companionJoin;
        public AudioClip companionLeave;

        public static Soundbank instance;

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

        #region UI Sounds
        public void PlayUIDecision()
        {
            BGMManager.instance.PlaySound(uiDecision, null);
        }

        public void PlayUICancel()
        {
            BGMManager.instance.PlaySound(uiCancel, null);
        }

        public void PlayUIHover()
        {
            if (selectCooldownTimer < maxSelectCooldownTimer)
            {
                return;
            }

            BGMManager.instance.PlaySound(uiHover, null);
            selectCooldownTimer = 0;
        }
        #endregion

        #region Movement
        public void PlayCloth(AudioSource audioSource)
        {
            BGMManager.instance.PlaySound(cloth, audioSource);
        }

        public void PlayDodge(AudioSource audioSource)
        {
            BGMManager.instance.PlaySound(dodge, audioSource);
        }

        public void PlayLanding(AudioSource audioSource)
        {
            BGMManager.instance.PlaySound(landing, audioSource);
        }
        #endregion

        public void PlayItemReceived()
        {
            BGMManager.instance.PlaySound(uiItemReceived, null);
        }

        public void PlayBookFlip()
        {
            BGMManager.instance.PlaySound(bookFlip, null);
        }

        public void PlayCoin()
        {
            BGMManager.instance.PlaySound(coin, null);
        }

        public void PlayNotificationAlert()
        {
            BGMManager.instance.PlaySound(alert, null);
        }
        
        public void PlayCraftSuccess()
        {
            BGMManager.instance.PlaySound(craftSuccess, null);
        }
        
        public void PlayCraftError()
        {
            BGMManager.instance.PlaySound(craftError, null);
        }
        
        public void PlayGameOver()
        {
            BGMManager.instance.PlaySound(gameOverFanfare, null);
        }

        public void PlayLeverActivated()
        {
            BGMManager.instance.PlaySound(activateLever, null);
        }

        public void PlayHeavyDoor()
        {
            BGMManager.instance.PlaySound(openHeavyDoor, null);
        }

        public void PlayQuickItemSwitch()
        {
            BGMManager.instance.PlaySound(quickItemSwitch, null);
        }

        public void PlayCompanionJoinParty()
        {
            BGMManager.instance.PlaySound(companionJoin, null);
        }

        public void PlayCompanionLeaveParty()
        {
            BGMManager.instance.PlaySound(companionLeave, null);
        }

    }

}
