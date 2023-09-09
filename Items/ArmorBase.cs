using UnityEngine;

namespace AF
{
    public enum ArmorSlot
    {
        Head,
        Chest,
        Arms,
        Legs,
    }

    public class ArmorBase : Item
    {
        [System.Serializable]
        public class StatusEffectResistance
        {
            public StatusEffect statusEffect;
            public float resistanceBonus;
        }

        [Header("Stats")]
        public float physicalDefense;

        [Header("Elemental")]
        public float fireDefense;
        public float frostDefense;
        public float lightningDefense;
        public float magicDefense = 0;

        [Header("Negative Status Resistances")]
        public StatusEffectResistance[] statusEffectResistances;

        [Header("Graphics")]
        public string graphicNameToShow;
        public string[] graphicNamesToHide;

        [Header("Attribute Bonus")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int intelligenceBonus = 0;

        [Header("Poise")]
        public int poiseBonus = 0;

        [Header("Speed Penalties")]
        public float speedPenalty = 0;

        [Header("Coins")]
        [Range(0, 100f)]
        public float additionalCoinPercentage = 0f;

        [Header("Reputation")]
        public int reputationBonus = 0;

    }

}
