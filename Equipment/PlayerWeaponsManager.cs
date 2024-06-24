using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AF.Health;
using AF.Stats;
using TigerForge;
using UnityEngine;
using UnityEngine.Localization;

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

        [Header("Dual Wielding")]
        public CharacterWeaponHitbox leftWeaponInstance;

        [Header("Database")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public PlayerManager playerManager;
        StatsBonusController statsBonusController;
        public NotificationManager notificationManager;

        [Header("Localization")]

        // "Can not apply buff to this weapon"
        public LocalizedString CanNotApplyBuffToThisWeapon;
        // "Weapon is already buffed"
        public LocalizedString WeaponIsAlreadyBuffed;
        // "Not enough mana to use weapon special"
        public LocalizedString NotEnoughManaToUseWeaponSpecial;

        private void Awake()
        {
            statsBonusController = playerManager.statsBonusController;

            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, UpdateCurrentShield);
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
            leftWeaponInstance?.DisableHitbox();
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
                weaponHitbox?.DisableHitbox();
                weaponHitbox?.gameObject.SetActive(false);
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

        bool CanApplyBuff()
        {
            if (currentWeaponInstance == null || currentWeaponInstance.characterWeaponBuffs == null)
            {
                notificationManager.ShowNotification(
                    CanNotApplyBuffToThisWeapon.GetLocalizedString(), notificationManager.systemError);
                return false;
            }
            else if (currentWeaponInstance.characterWeaponBuffs.HasOnGoingBuff())
            {
                notificationManager.ShowNotification(
                    WeaponIsAlreadyBuffed.GetLocalizedString(), notificationManager.systemError);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyFireToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.FIRE, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyFrostToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.FROST, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyLightningToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.LIGHTNING, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyMagicToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.MAGIC, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyDarknessToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.DARKNESS, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyPoisonToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.POISON, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyBloodToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.BLOOD, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplySharpnessToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.SHARPNESS, customDuration);
        }


        public void ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName weaponBuffName, float customDuration)
        {
            if (!CanApplyBuff())
            {
                return;
            }

            if (customDuration > 0)
            {
                currentWeaponInstance.characterWeaponBuffs.ApplyBuff(weaponBuffName, customDuration);
            }
            else
            {
                currentWeaponInstance.characterWeaponBuffs.ApplyBuff(weaponBuffName);
            }
        }

        public Damage GetBuffedDamage(Damage weaponDamage)
        {
            if (currentWeaponInstance == null || currentWeaponInstance.characterWeaponBuffs == null || currentWeaponInstance.characterWeaponBuffs.HasOnGoingBuff() == false)
            {
                return weaponDamage;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.FIRE)
            {
                weaponDamage.fire += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FIRE].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.FROST)
            {
                weaponDamage.frost += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FROST].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.LIGHTNING)
            {
                weaponDamage.lightning += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.LIGHTNING].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.MAGIC)
            {
                weaponDamage.magic += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.MAGIC].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.DARKNESS)
            {
                weaponDamage.darkness += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.DARKNESS].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.SHARPNESS)
            {
                weaponDamage.physical += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.SHARPNESS].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.POISON)
            {
                StatusEffectEntry statusEffectToApply = new()
                {
                    statusEffect = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.POISON].statusEffect,
                    amountPerHit = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.POISON].statusEffectAmountToApply,
                };

                if (weaponDamage.statusEffects == null)
                {
                    weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                }
                else
                {
                    weaponDamage.statusEffects.Append(statusEffectToApply);
                }
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.BLOOD)
            {
                StatusEffectEntry statusEffectToApply = new()
                {
                    statusEffect = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.BLOOD].statusEffect,
                    amountPerHit = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.BLOOD].statusEffectAmountToApply,
                };

                if (weaponDamage.statusEffects == null)
                {
                    weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                }
                else
                {
                    weaponDamage.statusEffects.Append(statusEffectToApply);
                }
            }

            return weaponDamage;
        }

        public void EquipLeftWeapon(CharacterWeaponHitbox leftWeaponGameObject)
        {
            leftWeaponInstance = leftWeaponGameObject;
            leftWeaponInstance.gameObject.SetActive(true);
        }

        public void UnequipLeftWeapon()
        {
            if (leftWeaponInstance != null)
            {
                leftWeaponInstance.gameObject.SetActive(false);
            }

            leftWeaponInstance = null;
        }

        public int GetCurrentBlockStaminaCost()
        {
            if (playerManager.playerWeaponsManager.currentShieldInstance == null)
            {
                return playerManager.characterBlockController.unarmedStaminaCostPerBlock;
            }

            return (int)playerManager.playerWeaponsManager.currentShieldInstance.shield.blockStaminaCost;
        }

        public Damage GetCurrentShieldDefenseAbsorption(Damage incomingDamage)
        {
            if (equipmentDatabase.isTwoHanding && equipmentDatabase.GetCurrentWeapon() != null)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * equipmentDatabase.GetCurrentWeapon().blockAbsorption);
                return incomingDamage;
            }
            else if (currentShieldInstance == null || currentShieldInstance.shield == null)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * playerManager.characterBlockController.unarmedDefenseAbsorption);
                return incomingDamage;
            }

            return currentShieldInstance.shield.FilterDamage(incomingDamage);
        }
        public Damage GetCurrentShieldPassiveDamageFilter(Damage incomingDamage)
        {
            if (currentShieldInstance == null || currentShieldInstance.shield == null)
            {
                return incomingDamage;
            }

            return currentShieldInstance.shield.FilterPassiveDamage(incomingDamage);
        }

        public void ApplyShieldDamageToAttacker(CharacterManager attacker)
        {
            if (currentShieldInstance == null || currentShieldInstance.shield == null)
            {
                return;
            }

            currentShieldInstance.shield.AttackShieldAttacker(attacker);
        }

        public void HandleWeaponSpecial()
        {
            if (
                playerManager.playerWeaponsManager.currentWeaponInstance == null
                || playerManager.playerWeaponsManager.currentWeaponInstance.onWeaponSpecial == null
                || playerManager.playerWeaponsManager.currentWeaponInstance.weapon == null
                )
            {
                return;
            }

            if (playerManager.manaManager.playerStatsDatabase.currentMana < playerManager.playerWeaponsManager.currentWeaponInstance.weapon.manaCostToUseWeaponSpecialAttack)
            {
                notificationManager.ShowNotification(NotEnoughManaToUseWeaponSpecial.GetLocalizedString());
                return;
            }

            playerManager.manaManager.DecreaseMana(
                playerManager.playerWeaponsManager.currentWeaponInstance.weapon.manaCostToUseWeaponSpecialAttack
            );

            playerManager.playerWeaponsManager.currentWeaponInstance.onWeaponSpecial?.Invoke();

        }

    }
}
