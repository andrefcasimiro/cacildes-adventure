using System.Collections.Generic;
using System.Linq;
using AF.StatusEffects;
using UnityEngine;
using static AF.ArmorBase;

namespace AF.Stats
{
    public class StatsBonusController : MonoBehaviour
    {
        [Header("Bonus")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int intelligenceBonus = 0;

        public float fireDefenseBonus = 0;
        public float frostDefenseBonus = 0;
        public float lightningDefenseBonus = 0;
        public float magicDefenseBonus = 0;

        public float additionalCoinPercentage = 0;

        public int parryPostureDamageBonus = 0;

        public int reputationBonus = 0;

        public float chanceToStealBonus = 0;

        [Header("Equipment Modifiers")]
        public float weightPenalty = 0f;
        public int equipmentPoise = 0;
        public float equipmentPhysicalDefense = 0;

        [Header("Status Controller")]
        public StatusController statusController;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        public void RecalculateEquipmentBonus()
        {
            UpdateWeightPenalty();
            UpdateArmorPoise();
            UpdateEquipmentPhysicalDefense();
            UpdateStatusEffectResistances();
            UpdateAttributes();
            UpdateAdditionalCoinPercentage();
        }

        void UpdateWeightPenalty()
        {
            weightPenalty = 0f;

            if (equipmentDatabase.GetCurrentWeapon() != null)
            {
                weightPenalty += equipmentDatabase.GetCurrentWeapon().speedPenalty;
            }
            if (equipmentDatabase.GetCurrentShield() != null)
            {
                weightPenalty += equipmentDatabase.GetCurrentShield().speedPenalty;
            }
            if (equipmentDatabase.helmet != null)
            {
                weightPenalty += equipmentDatabase.helmet.speedPenalty;
            }
            if (equipmentDatabase.armor != null)
            {
                weightPenalty += equipmentDatabase.armor.speedPenalty;
            }
            if (equipmentDatabase.gauntlet != null)
            {
                weightPenalty += equipmentDatabase.gauntlet.speedPenalty;
            }
            if (equipmentDatabase.legwear != null)
            {
                weightPenalty += equipmentDatabase.legwear.speedPenalty;
            }

            weightPenalty += equipmentDatabase.accessories.Sum(x => x == null ? 0 : x.speedPenalty);

            weightPenalty = Mathf.Max(0, weightPenalty); // Ensure weightPenalty is non-negative
        }

        void UpdateArmorPoise()
        {
            equipmentPoise = 0;

            if (equipmentDatabase.helmet != null)
            {
                equipmentPoise += equipmentDatabase.helmet.poiseBonus;
            }
            if (equipmentDatabase.armor != null)
            {
                equipmentPoise += equipmentDatabase.armor.poiseBonus;
            }
            if (equipmentDatabase.gauntlet != null)
            {
                equipmentPoise += equipmentDatabase.gauntlet.poiseBonus;
            }
            if (equipmentDatabase.legwear != null)
            {
                equipmentPoise += equipmentDatabase.legwear.poiseBonus;
            }

            equipmentPoise += equipmentDatabase.accessories.Sum(x => x == null ? 0 : x.poiseBonus);
        }

        void UpdateEquipmentPhysicalDefense()
        {
            equipmentPhysicalDefense = 0f;

            if (equipmentDatabase.helmet != null)
            {
                equipmentPhysicalDefense += equipmentDatabase.helmet.physicalDefense;
            }

            if (equipmentDatabase.armor != null)
            {
                equipmentPhysicalDefense += equipmentDatabase.armor.physicalDefense;
            }

            if (equipmentDatabase.gauntlet != null)
            {
                equipmentPhysicalDefense += equipmentDatabase.gauntlet.physicalDefense;
            }

            if (equipmentDatabase.legwear != null)
            {
                equipmentPhysicalDefense += equipmentDatabase.legwear.physicalDefense;
            }

            equipmentPhysicalDefense += equipmentDatabase.accessories.Sum(x => x == null ? 0 : x.physicalDefense);
        }

        void UpdateStatusEffectResistances()
        {
            statusController.statusEffectResistanceBonuses.Clear();

            HandleStatusEffectEntries(equipmentDatabase.helmet?.statusEffectResistances);
            HandleStatusEffectEntries(equipmentDatabase.armor?.statusEffectResistances);
            HandleStatusEffectEntries(equipmentDatabase.gauntlet?.statusEffectResistances);
            HandleStatusEffectEntries(equipmentDatabase.legwear?.statusEffectResistances);

            var accessoryResistances = equipmentDatabase.accessories
                .Where(a => a != null)
                .SelectMany(a => a.statusEffectResistances ?? Enumerable.Empty<StatusEffectResistance>())
                .ToArray();
            HandleStatusEffectEntries(accessoryResistances);
        }

        void HandleStatusEffectEntries(StatusEffectResistance[] resistances)
        {
            if (resistances != null && resistances.Length > 0)
            {
                foreach (var resistance in resistances)
                {
                    HandleStatusEffectEntry(resistance);
                }
            }
        }

        void HandleStatusEffectEntry(StatusEffectResistance statusEffectResistance)
        {
            if (statusController.statusEffectResistanceBonuses.ContainsKey(statusEffectResistance.statusEffect))
            {
                statusController.statusEffectResistanceBonuses[statusEffectResistance.statusEffect]
                    += (int)statusEffectResistance.resistanceBonus;
            }
            else
            {
                statusController.statusEffectResistanceBonuses.Add(
                    statusEffectResistance.statusEffect, (int)statusEffectResistance.resistanceBonus);
            }
        }

        void UpdateAttributes()
        {
            ResetAttributes();

            ApplyEquipmentAttributes(equipmentDatabase.helmet);
            ApplyEquipmentAttributes(equipmentDatabase.armor);
            ApplyEquipmentAttributes(equipmentDatabase.gauntlet);
            ApplyEquipmentAttributes(equipmentDatabase.legwear);

            ApplyAccessoryAttributes();
        }

        void ResetAttributes()
        {
            vitalityBonus = enduranceBonus = strengthBonus = dexterityBonus = intelligenceBonus = 0;
            fireDefenseBonus = frostDefenseBonus = lightningDefenseBonus = magicDefenseBonus = 0;
            reputationBonus = parryPostureDamageBonus = 0;
        }

        void ApplyEquipmentAttributes(ArmorBase equipment)
        {
            if (equipment != null)
            {
                vitalityBonus += equipment.vitalityBonus;
                enduranceBonus += equipment.enduranceBonus;
                strengthBonus += equipment.strengthBonus;
                dexterityBonus += equipment.dexterityBonus;
                intelligenceBonus += equipment.intelligenceBonus;
                fireDefenseBonus += equipment.fireDefense;
                frostDefenseBonus += equipment.frostDefense;
                lightningDefenseBonus += equipment.lightningDefense;
                magicDefenseBonus += equipment.magicDefense;
                reputationBonus += equipment.reputationBonus;
            }
        }

        void ApplyAccessoryAttributes()
        {
            foreach (var accessory in equipmentDatabase.accessories)
            {
                vitalityBonus += accessory?.vitalityBonus ?? 0;
                enduranceBonus += accessory?.enduranceBonus ?? 0;
                strengthBonus += accessory?.strengthBonus ?? 0;
                dexterityBonus += accessory?.dexterityBonus ?? 0;
                intelligenceBonus += accessory?.intelligenceBonus ?? 0;
                fireDefenseBonus += accessory?.fireDefense ?? 0;
                frostDefenseBonus += accessory?.frostDefense ?? 0;
                lightningDefenseBonus += accessory?.lightningDefense ?? 0;
                magicDefenseBonus += accessory?.magicDefense ?? 0;
                reputationBonus += accessory?.reputationBonus ?? 0;
                parryPostureDamageBonus += accessory?.postureDamagePerParry ?? 0;
            }
        }

        void UpdateAdditionalCoinPercentage()
        {
            additionalCoinPercentage = GetEquipmentCoinPercentage(equipmentDatabase.helmet)
                                   + GetEquipmentCoinPercentage(equipmentDatabase.armor)
                                   + GetEquipmentCoinPercentage(equipmentDatabase.gauntlet)
                                   + GetEquipmentCoinPercentage(equipmentDatabase.legwear)
                                   + equipmentDatabase.accessories.Sum(x => x == null ? 0 : x.additionalCoinPercentage);
        }

        float GetEquipmentCoinPercentage(ArmorBase equipment)
        {
            return equipment != null ? equipment.additionalCoinPercentage : 0f;
        }
    }
}
