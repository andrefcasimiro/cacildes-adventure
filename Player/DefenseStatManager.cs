using System.Collections.Generic;
using AF.Stats;
using UnityEngine;

namespace AF
{

    public class DefenseStatManager : MonoBehaviour
    {
        [Header("Physical Defense")]
        public int basePhysicalDefense = 30;
        [Tooltip("Increases with endurance level")]
        public float levelMultiplier = 3.25f;

        [Header("Status defense bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalDefenseBonus = 0f;
        [Range(0, 100f)] public float physicalDefenseAbsorption = 0f;

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
            return 1f;
        }

        public float GetMagicDefense()
        {
            return playerStatsBonusController.magicDefenseBonus;
        }

        public float GetDarknessDefense()
        {
            return playerStatsBonusController.darkDefenseBonus;
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

        public void SetDefenseAbsorption(int value)
        {
            physicalDefenseAbsorption = value;
        }

        public void ResetDefenseAbsorption()
        {
            physicalDefenseAbsorption = 0f;
        }
    }
}
