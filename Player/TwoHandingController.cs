using UnityEngine;

namespace AF
{

    public class TwoHandingController : MonoBehaviour
    {
        public PlayerManager playerManager;
        public LockOnManager lockOnManager;

        [Header("Flags")]
        public bool isTwoHanding = false;

        private void Awake()
        {
            UpdateTwoHandingMode();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnInput()
        {
            SetIsTwoHanding(!isTwoHanding);
        }

        public void SetIsTwoHanding(bool value)
        {
            this.isTwoHanding = value;

            UpdateTwoHandingMode();
        }

        public void UpdateTwoHandingMode()
        {
            if (isTwoHanding)
            {
                playerManager.playerWeaponsManager.HideShield();
                playerManager.playerWeaponsManager.currentShieldInstance?.SetIsUsingShield(false);

                if (playerManager.playerWeaponsManager.currentWeaponInstance != null && playerManager.playerWeaponsManager.currentWeaponInstance.UseCustomTwoHandTransform())
                {
                    playerManager.playerWeaponsManager.currentWeaponInstance.characterTwoHandRef.UseTwoHandTransform();
                }
            }
            else
            {
                playerManager.playerWeaponsManager.ShowShield();
                playerManager.playerWeaponsManager.currentShieldInstance?.SetIsUsingShield(true);

                if (playerManager.playerWeaponsManager.currentWeaponInstance != null && playerManager.playerWeaponsManager.currentWeaponInstance.UseCustomTwoHandTransform())
                {
                    playerManager.playerWeaponsManager.currentWeaponInstance.characterTwoHandRef.UseDefaultTransform();
                }
            }

            playerManager.UpdateAnimatorOverrideControllerClips();

            if (lockOnManager.isLockedOn)
            {
                LockOnRef tmp = lockOnManager.nearestLockOnTarget;
                lockOnManager.DisableLockOn();
                lockOnManager.nearestLockOnTarget = tmp;
                lockOnManager.EnableLockOn();
            }
        }
    }
}
