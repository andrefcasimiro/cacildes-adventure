using System.Collections.Generic;
using AF.Stats;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class NegativeStatusResistance
    {
        public StatusEffect statusEffect;

        public float resistance = 100f;

        public float decreaseRateWithoutDamage = 5;

        public float decreaseRateWithDamage = 1;
    }

    public class DefenseStatManager : MonoBehaviour
    {
        [Header("Physical Defense")]
        public int basePhysicalDefense = 60;
        [Tooltip("Increases with endurance level")]
        public float levelMultiplier = 3.25f;

        [Header("Status defense bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalDefenseBonus = 0f;

        [Header("Negative Status Resistances")]
        public List<NegativeStatusResistance> negativeStatusResistances = new List<NegativeStatusResistance>();

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Database")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        public bool ignoreDefense = false;

        public float GetDefenseAbsorption()
        {
            if (ignoreDefense)
            {
                return 0f;
            }

            return (int)(
                GetCurrentPhysicalDefense()
                + playerStatsBonusController.equipmentPhysicalDefense // Equipment Bonus
                + physicalDefenseBonus
                + (playerStatsBonusController.enduranceBonus * levelMultiplier)
            );
        }

        public int GetCurrentPhysicalDefense()
        {
            return (int)(this.basePhysicalDefense + playerStatsDatabase.endurance * levelMultiplier) / 2;
        }

        public int GetCurrentPhysicalDefenseForGivenEndurance(int endurance)
        {
            return (int)(this.basePhysicalDefense + ((endurance * levelMultiplier) / 2));
        }

        public float GetMaximumStatusResistanceBeforeSufferingStatusEffect(StatusEffect statusEffect)
        {
            var target = this.negativeStatusResistances.Find(x => x.statusEffect == statusEffect);

            if (target == null)
            {
                // Fallback for unknown status
                return 100f;
            }

            var bonusFromEquipment = 0f;

            var idx = playerStatsBonusController.statusEffectResistances.FindIndex(x => x.statusEffect == statusEffect);
            if (idx != -1)
            {
                bonusFromEquipment += playerStatsBonusController.statusEffectResistances[idx].resistanceBonus;
            }

            return target.resistance + bonusFromEquipment + (playerStatsDatabase.endurance * levelMultiplier);
        }

        public float GetMagicDefense()
        {
            return playerStatsBonusController.magicDefenseBonus;
        }

        public float GetFireDefense()
        {
            return playerStatsBonusController.fireDefenseBonus;
        }

        public float GetFrostDefense()
        {
            return playerStatsBonusController.frostDefenseBonus;
        }

        public float GetLightningDefense()
        {
            return playerStatsBonusController.lightningDefenseBonus;
        }

        public int CompareHelmet(Helmet helmet)
        {
            if (equipmentDatabase.helmet == null)
            {
                return 1;
            }

            if (helmet.physicalDefense > equipmentDatabase.helmet.physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.helmet.physicalDefense == helmet.physicalDefense)
            {
                return 0;
            }

            return -1;
        }

        public int CompareArmor(Armor armor)
        {
            if (equipmentDatabase.armor == null)
            {
                return 1;
            }

            if (armor.physicalDefense > equipmentDatabase.armor.physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.armor.physicalDefense == armor.physicalDefense)
            {
                return 0;
            }

            return -1;
        }

        public int CompareGauntlet(Gauntlet gauntlet)
        {
            if (equipmentDatabase.gauntlet == null)
            {
                return 1;
            }

            if (gauntlet.physicalDefense > equipmentDatabase.gauntlet.physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.gauntlet.physicalDefense == gauntlet.physicalDefense)
            {
                return 0;
            }

            return -1;
        }

        public int CompareLegwear(Legwear legwear)
        {
            if (equipmentDatabase.legwear == null)
            {
                return 1;
            }

            if (legwear.physicalDefense > equipmentDatabase.legwear.physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.legwear.physicalDefense == legwear.physicalDefense)
            {
                return 0;
            }

            return -1;
        }
    }
}
