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
        [Header("Stats")]
        public float physicalDefense;

        public float fireDefense;
        public float frostDefense;
        public float lightningDefense;

        public float bleedResistance;
        public float fatigueResistance;
        public float poisonResistance;

        [Header("Graphics")]
        public string graphicNameToShow;
        public string[] graphicNamesToHide;

        [Header("Attribute Bonus")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;

        public int poiseBonus = 0;

    }

}
