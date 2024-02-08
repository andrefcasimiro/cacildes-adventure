using AF.Animations;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Animations
{
    public class PlayerAnimationEventListener : MonoBehaviour, IAnimationEventListener
    {
        public PlayerManager playerManager;

        [Header("Unity Events")]
        public UnityEvent onLeftFootstep;
        public UnityEvent onRightFootstep;

        [Header("Components")]
        public AudioSource combatAudioSource;
        public Soundbank soundbank;

        public void OpenLeftWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.leftHandHitbox != null)
            {
                playerManager.playerWeaponsManager.leftHandHitbox.EnableHitbox();
            }

            playerManager.thirdPersonController.canRotateCharacter = false;
        }

        public void CloseLeftWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.leftHandHitbox != null)
            {
                playerManager.playerWeaponsManager.leftHandHitbox.DisableHitbox();
            }

        }

        public void OpenRightWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.currentWeaponInstance != null)
            {
                playerManager.playerWeaponsManager.currentWeaponInstance.EnableHitbox();
            }
            else if (playerManager.playerWeaponsManager.rightHandHitbox != null)
            {
                playerManager.playerWeaponsManager.rightHandHitbox.EnableHitbox();
            }

            playerManager.thirdPersonController.canRotateCharacter = false;
        }

        public void CloseRightWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.currentWeaponInstance != null)
            {
                playerManager.playerWeaponsManager.currentWeaponInstance.DisableHitbox();
            }
            if (playerManager.playerWeaponsManager.rightHandHitbox != null)
            {
                playerManager.playerWeaponsManager.rightHandHitbox.DisableHitbox();
            }

        }

        public void OpenLeftFootHitbox()
        {
            if (playerManager.playerWeaponsManager.leftFootHitbox != null)
            {
                playerManager.playerWeaponsManager.leftFootHitbox.EnableHitbox();
            }

            playerManager.thirdPersonController.canRotateCharacter = false;
        }

        public void CloseLeftFootHitbox()
        {
            if (playerManager.playerWeaponsManager.leftFootHitbox != null)
            {
                playerManager.playerWeaponsManager.leftFootHitbox.DisableHitbox();
            }

        }

        public void OpenRightFootHitbox()
        {
            if (playerManager.playerWeaponsManager.rightFootHitbox != null)
            {
                playerManager.playerWeaponsManager.rightFootHitbox.EnableHitbox();
            }

            playerManager.thirdPersonController.canRotateCharacter = false;
        }

        public void CloseRightFootHitbox()
        {
            if (playerManager.playerWeaponsManager.rightFootHitbox != null)
            {
                playerManager.playerWeaponsManager.rightFootHitbox.DisableHitbox();
            }

        }
        public void DisableRotation()
        {
            playerManager.thirdPersonController.canRotateCharacter = false;
        }

        public void EnableRootMotion()
        {
            playerManager.animator.applyRootMotion = true;
        }

        public void DisableRootMotion()
        {
            playerManager.animator.applyRootMotion = false;
        }

        public void FaceTarget()
        {

        }

        public void SetAnimatorBool_True(string parameterName)
        {
            playerManager.animator.SetBool(parameterName, true);
        }

        public void SetAnimatorBool_False(string parameterName)
        {
            playerManager.animator.SetBool(parameterName, false);
        }

        public void OnSpellCast()
        {
            playerManager.playerShootingManager.CastSpell();
        }

        public void OnFireArrow()
        {
        }

        public void OnLeftFootstep()
        {

            onLeftFootstep?.Invoke();
        }

        public void OnRightFootstep()
        {
            onRightFootstep?.Invoke();
        }

        public void OnCloth()
        {
            if (playerManager.thirdPersonController.Grounded)
            {
                soundbank.PlaySound(soundbank.dodge, combatAudioSource);
            }
            else
            {
                soundbank.PlaySound(soundbank.cloth, combatAudioSource);
            }
        }

        public void OnImpact()
        {
            soundbank.PlaySound(soundbank.impact, combatAudioSource);
        }

        public void OnBuff()
        {

        }

        public void OpenCombo()
        {

        }

        public void OnThrow()
        {
            playerManager.projectileSpawner.ThrowProjectile();
        }
    }
}
