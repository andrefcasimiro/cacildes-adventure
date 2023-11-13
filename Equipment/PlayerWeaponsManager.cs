using System.Collections.Generic;
using AF.Animations;
using AF.Events;
using AF.Stats;
using TigerForge;
using UnityEngine;

namespace AF.Equipment
{
    public class PlayerWeaponsManager : MonoBehaviour
    {
        [Header("Unarmed Weapon References In-World")]
        public PlayerWeaponHitbox leftHandHitbox;
        public PlayerWeaponHitbox rightHandHitbox;
        public PlayerWeaponHitbox leftFootHitbox;
        public PlayerWeaponHitbox rightFootHitbox;

        [Header("Weapon References In-World")]
        public List<PlayerWeaponHitbox> weaponInstances;

        [Header("Current Weapon")]
        public PlayerWeaponHitbox currentWeapon;

        [Header("Database")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public PlayerManager playerManager;
        StatsBonusController statsBonusController;

        // Animator Overrides
        protected AnimatorOverrideController animatorOverrideController;
        RuntimeAnimatorController defaultAnimatorController;

        private void Awake()
        {
            defaultAnimatorController = playerManager.animator.runtimeAnimatorController;
            animatorOverrideController = new AnimatorOverrideController(playerManager.animator.runtimeAnimatorController);

            UpdateAnimatorOverrideControllerClips();

            statsBonusController = playerManager.statsBonusController;

            EventManager.StartListening(
                EventMessages.ON_WEAPON_CHANGED,
                UpdateCurrentWeapon);
        }

        public void EquipWeapon(Weapon weaponToEquip, int slot)
        {
            equipmentDatabase.EquipWeapon(weaponToEquip, slot);

            UpdateCurrentWeapon();
        }

        public void UnequipWeapon(int slot)
        {
            equipmentDatabase.UnequipWeapon(slot);

            UpdateCurrentWeapon();
        }

        void UpdateCurrentWeapon()
        {
            var CurrentWeapon = equipmentDatabase.GetCurrentWeapon();

            foreach (PlayerWeaponHitbox weaponHitbox in weaponInstances)
            {
                weaponHitbox.gameObject.SetActive(weaponHitbox.weapon?.name?.GetEnglishText() == CurrentWeapon?.name?.GetEnglishText());
            }

            UpdateAnimatorOverrideControllerClips();
        }

        public void EquipShield(Shield shieldToEquip, int slot)
        {
            equipmentDatabase.EquipShield(shieldToEquip, slot);

            UpdateCurrentShield();
        }

        public void UnequipShield(int slot)
        {
            equipmentDatabase.UnequipShield(slot);

            UpdateCurrentShield();
        }

        void UpdateCurrentShield()
        {
            var CurrentShield = equipmentDatabase.GetCurrentShield();

            statsBonusController.RecalculateEquipmentBonus();

            /*foreach (Pla weaponHitbox in shieldInstances)
            {
                weaponHitbox.gameObject.SetActive(weaponHitbox.weapon == CurrentWeapon);
            }*/
        }

        public void UpdateAnimatorOverrideControllerClips()
        {

            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            if (equipmentDatabase.GetCurrentWeapon() != null)
            {
                foreach (var animationOverride in equipmentDatabase.GetCurrentWeapon().animationOverrides)
                {

                    clipOverrides[animationOverride.animationName] = animationOverride.animationClip;
                    animatorOverrideController.ApplyOverrides(clipOverrides);

                }

                playerManager.animator.runtimeAnimatorController = animatorOverrideController;
            }
            else
            {
                playerManager.animator.runtimeAnimatorController = defaultAnimatorController;
            }
        }
    }
}
