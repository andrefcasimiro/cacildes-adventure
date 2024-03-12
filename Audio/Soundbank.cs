using AF.Music;
using UnityEngine;

namespace AF
{
    public class Soundbank : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;

        [Header("UI Sounds")]
        public AudioClip uiDecision;
        public AudioClip uiHover;
        public AudioClip uiCancel;
        public AudioClip uiItemReceived;
        public AudioClip insufficientStamina;
        public AudioClip quickItemSwitch;
        public AudioClip enemyGuardBreak;
        public AudioClip reputationIncreased;
        public AudioClip reputationDecreased;
        public AudioClip mainMenuOpen;
        public AudioClip uiEquip;
        public AudioClip uiUnequip;
        public AudioClip uiLockOn;
        public AudioClip uiLockOnSwitchTarget;
        public AudioClip switchTwoHand;

        [Header("Movement")]
        public AudioClip cloth;
        public AudioClip dodge;
        public AudioClip impact;


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
        public AudioClip illusionaryWallSound;
        public AudioClip uiDialogue;
        public AudioClip itemLostWithUse;

        public void PlaySound(AudioClip sound)
        {
            PlaySound(sound, null);
        }

        public void PlaySound(AudioClip sound, AudioSource audioSource)
        {
            bgmManager.PlaySound(sound, audioSource);
        }

    }

}
