
using UnityEngine;

namespace AF
{

    public static class Formulas
    {

        public static int CalculateAIHealth(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 4) * Mathf.Pow(1.025f, currentLevel));
        }

        public static int CalculateAIAttack(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 2) * Mathf.Pow(1.15f, currentLevel));
        }

        public static int CalculateAIPosture(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 4) * Mathf.Pow(1.05f, currentLevel));
        }

        public static int CalculateAIGenericValue(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 2) * Mathf.Pow(1.05f, currentLevel));
        }

        public static int CalculateSpellValue(int baseValue, int currentIntelligence)
        {
            if (currentIntelligence <= 1)
            {
                currentIntelligence = 0;
            }

            return baseValue + Mathf.RoundToInt((baseValue / 4) * Mathf.Pow(1.04f, currentIntelligence));
        }

        public static int CalculateIncomingElementalAttack(int damageToReceive, WeaponElementType weaponElementType, DefenseStatManager defenseStatManager)
        {
            // Apply elemental defense reduction based on weaponElementType
            float elementalDefense = 0f;
            switch (weaponElementType)
            {
                case WeaponElementType.Fire:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetFireDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
                case WeaponElementType.Frost:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetFrostDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
                case WeaponElementType.Lightning:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetLightningDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
                case WeaponElementType.Magic:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetMagicDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
                case WeaponElementType.Darkness:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetDarknessDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
            }

            // Calculate the final damage to receive, considering elemental defense
            if (elementalDefense > 0)
            {
                damageToReceive = (int)(damageToReceive * (1 - elementalDefense)); // Subtract elemental defense as a percentage
            }

            return damageToReceive;
        }
    }

}