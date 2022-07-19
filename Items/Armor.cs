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

    [CreateAssetMenu(menuName = "Armor / New Armor")]
    public class Armor : Item
    {
        public ArmorSlot armorType;

        // Physical
        public float physicalDefense;

        // Elemental
        public float fireDefense;
        public float frostDefense;
        public float lightningDefense;

        // Mystical
        public float arcaneDefense;
        public float faithDefense;
        public float darknessDefense;

        // Buildups
        public float bleedDefense;
        public float poisonDefense;

        // Weight
        public float weight;

        // Graphics
        public GameObject graphic;

        // Visual
        public string graphicNameToShow;

        public string[] graphicNamesToHide;

    }

}
