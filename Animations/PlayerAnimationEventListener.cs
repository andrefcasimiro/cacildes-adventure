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

        public void OnLeftFootstep()
        {
            onLeftFootstep?.Invoke();
        }

        public void OnRightFootstep()
        {
            onRightFootstep?.Invoke();
        }

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
            if (playerManager.playerWeaponsManager.currentWeapon != null)
            {
                playerManager.playerWeaponsManager.currentWeapon.EnableHitbox();
            }
            else if (playerManager.playerWeaponsManager.rightHandHitbox != null)
            {
                playerManager.playerWeaponsManager.rightHandHitbox.EnableHitbox();
            }

            playerManager.thirdPersonController.canRotateCharacter = false;
        }

        public void CloseRightWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.currentWeapon != null)
            {
                playerManager.playerWeaponsManager.currentWeapon.DisableHitbox();
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

    }
}
