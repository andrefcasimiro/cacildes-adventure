using UnityEngine;

namespace AF
{

    public class TwoHandingController : MonoBehaviour
    {
        public PlayerManager playerManager;
        public LockOnManager lockOnManager;
        public Soundbank soundbank;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        private void Awake()
        {
            UpdateTwoHandingMode();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnInput()
        {
            SetIsTwoHanding(!equipmentDatabase.isTwoHanding);
            soundbank.PlaySound(soundbank.switchTwoHand);
        }

        public void SetIsTwoHanding(bool value)
        {
            equipmentDatabase.isTwoHanding = value;

            UpdateTwoHandingMode();
        }

        public void UpdateTwoHandingMode()
        {
            if (equipmentDatabase.isTwoHanding)
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
