using System.Collections.Generic;
using System.Linq;
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
        public CharacterWeaponHitbox leftHandHitbox;
        public CharacterWeaponHitbox rightHandHitbox;
        public CharacterWeaponHitbox leftFootHitbox;
        public CharacterWeaponHitbox rightFootHitbox;

        [Header("Weapon References In-World")]
        public List<CharacterWeaponHitbox> weaponInstances;
        public List<ShieldInstance> shieldInstances;

        [Header("Current Weapon")]
        public CharacterWeaponHitbox currentWeaponInstance;
        public ShieldInstance currentShieldInstance;

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

            statsBonusController = playerManager.statsBonusController;

            EventManager.StartListening(
                EventMessages.ON_WEAPON_CHANGED,
                UpdateCurrentWeapon);

            EventManager.StartListening(
                EventMessages.ON_SHIELD_CHANGED,
                UpdateCurrentShield);

            EventManager.StartListening(
                EventMessages.ON_ARROW_CHANGED,
                UpdateCurrentArrows);

            EventManager.StartListening(
                EventMessages.ON_SPELL_CHANGED,
                UpdateCurrentSpells);
        }

        private void Start()
        {
            UpdateCurrentWeapon();
            UpdateCurrentShield();
            UpdateCurrentArrows();
            UpdateCurrentSpells();
        }

        public void ResetStates()
        {
            CloseAllWeaponHitboxes();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void CloseAllWeaponHitboxes()
        {
            currentWeaponInstance?.DisableHitbox();
            leftFootHitbox?.DisableHitbox();
            rightFootHitbox?.DisableHitbox();
            leftHandHitbox?.DisableHitbox();
            rightHandHitbox?.DisableHitbox();
        }

        void UpdateCurrentWeapon()
        {
            var CurrentWeapon = equipmentDatabase.GetCurrentWeapon();

            if (currentWeaponInstance != null)
            {
                currentWeaponInstance = null;
            }

            foreach (CharacterWeaponHitbox weaponHitbox in weaponInstances)
            {
                weaponHitbox.DisableHitbox();
                weaponHitbox.gameObject.SetActive(false);
            }

            if (CurrentWeapon != null)
            {
                var gameObjectWeapon = weaponInstances.FirstOrDefault(x => x.weapon.name == CurrentWeapon.name);
                currentWeaponInstance = gameObjectWeapon;
                currentWeaponInstance.gameObject.SetActive(true);
            }

            UpdateAnimatorOverrideControllerClips();

            // If we equipped a bow, we must hide any active shield
            if (equipmentDatabase.IsBowEquipped() || equipmentDatabase.IsStaffEquipped())
            {
                UnassignShield();
            }
            // Otherwise, we need to check if we should activate a bow if we just switched from a bow to other weapon that might allow a shield
            else
            {
                UpdateCurrentShield();
            }
        }

        void UpdateCurrentArrows()
        {
            if (equipmentDatabase.IsBowEquipped() == false)
            {
                return;
            }

            UnassignShield();
        }

        void UpdateCurrentSpells()
        {
            if (equipmentDatabase.IsStaffEquipped() == false)
            {
                return;
            }

            UnassignShield();
        }

        void UpdateCurrentShield()
        {
            var CurrentShield = equipmentDatabase.GetCurrentShield();

            statsBonusController.RecalculateEquipmentBonus();

            if (currentShieldInstance != null)
            {
                currentShieldInstance = null;
            }

            foreach (ShieldInstance shieldInstance in shieldInstances)
            {
                shieldInstance.gameObject.SetActive(false);
            }

            if (CurrentShield != null)
            {
                var gameObjectShield = shieldInstances.FirstOrDefault(x => x.shield.name == CurrentShield.name);
                currentShieldInstance = gameObjectShield;
                currentShieldInstance.gameObject.SetActive(true);
            }
        }

        void UnassignShield()
        {
            if (currentShieldInstance != null)
            {
                currentShieldInstance.gameObject.SetActive(false);
                currentShieldInstance = null;
            }
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

        public void UpdateAnimatorOverrideControllerClips()
        {
            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            playerManager.animator.runtimeAnimatorController = defaultAnimatorController;

            if (equipmentDatabase.GetCurrentWeapon() != null && equipmentDatabase.GetCurrentWeapon().animationOverrides.Count > 0)
            {
                foreach (var animationOverride in equipmentDatabase.GetCurrentWeapon().animationOverrides)
                {
                    clipOverrides[animationOverride.animationName] = animationOverride.animationClip;
                    animatorOverrideController.ApplyOverrides(clipOverrides);
                }

                playerManager.animator.runtimeAnimatorController = animatorOverrideController;
            }
        }

        public void UpdateAnimatorOverrideControllerClip(string animationName, AnimationClip animationClip)
        {
            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            playerManager.animator.runtimeAnimatorController = defaultAnimatorController;

            clipOverrides[animationName] = animationClip;

            animatorOverrideController.ApplyOverrides(clipOverrides);
            playerManager.animator.runtimeAnimatorController = animatorOverrideController;
        }

        public void ShowEquipment()
        {
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.ShowWeapon();
            }

            if (currentShieldInstance != null)
            {
                currentShieldInstance.ShowShield();
            }
        }

        public void HideEquipment()
        {
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.HideWeapon();
            }

            if (currentShieldInstance != null)
            {
                currentShieldInstance.HideShield();
            }
        }

        public void HideShield()
        {
            if (currentShieldInstance != null)
            {
                currentShieldInstance.HideShield();
            }
        }
    }
}
