using System.Collections.Generic;
using System.Linq;
using AF.Animations;
using AF.Events;
using AF.Health;
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

        private void Awake()
        {
            statsBonusController = playerManager.statsBonusController;

            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);
        }

        private void Start()
        {
            UpdateEquipment();
        }

        void UpdateEquipment()
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

            playerManager.UpdateAnimatorOverrideControllerClips();

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
                shieldInstance.shieldInTheBack.SetActive(false);
            }

            if (CurrentShield != null)
            {
                var gameObjectShield = shieldInstances.FirstOrDefault(x => x.shield.name == CurrentShield.name);
                currentShieldInstance = gameObjectShield;
                currentShieldInstance.gameObject.SetActive(true);
                currentShieldInstance.SetIsUsingShield(true);
            }
        }

        void UnassignShield()
        {
            if (currentShieldInstance != null)
            {
                currentShieldInstance.gameObject.SetActive(false);
                currentShieldInstance.shieldInTheBack.gameObject.SetActive(false);
                currentShieldInstance = null;
            }
        }

        public void EquipWeapon(Weapon weaponToEquip, int slot)
        {
            equipmentDatabase.EquipWeapon(weaponToEquip, slot);

            UpdateCurrentWeapon();

            playerManager.twoHandingController.UpdateTwoHandingMode();
        }

        public void UnequipWeapon(int slot)
        {
            equipmentDatabase.UnequipWeapon(slot);

            UpdateCurrentWeapon();

            playerManager.twoHandingController.UpdateTwoHandingMode();
        }

        public void EquipShield(Shield shieldToEquip, int slot)
        {
            equipmentDatabase.EquipShield(shieldToEquip, slot);

            UpdateCurrentShield();

            playerManager.twoHandingController.UpdateTwoHandingMode();
        }

        public void UnequipShield(int slot)
        {
            equipmentDatabase.UnequipShield(slot);

            UpdateCurrentShield();

            playerManager.twoHandingController.UpdateTwoHandingMode();
        }

        public void ShowEquipment()
        {
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.ShowWeapon();
            }

            if (currentShieldInstance != null)
            {
                currentShieldInstance.ResetStates();
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

        public void ShowShield()
        {
            if (currentShieldInstance != null)
            {
                currentShieldInstance.ShowShield();
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyFireToWeapon()
        {
            if (currentWeaponInstance == null || currentWeaponInstance.characterWeaponBuffs == null)
            {
                return;
            }

            currentWeaponInstance.characterWeaponBuffs.ApplyFireBuff();
        }

        public Damage GetBuffedDamage(Damage weaponDamage)
        {
            if (currentWeaponInstance == null || currentWeaponInstance.characterWeaponBuffs == null || currentWeaponInstance.characterWeaponBuffs.hasBuff == false)
            {
                return weaponDamage;
            }

            if (currentWeaponInstance.characterWeaponBuffs.fireBuffContainer != null)
            {
                weaponDamage.fire += currentWeaponInstance.characterWeaponBuffs.fireBuffBonus;
            }

            return weaponDamage;
        }
    }
}
