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

        // Stats
        public int vitalityBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int charismaBonus = 0;
        public int enduranceBonus = 0;

        public void OnEquip()
        {
            PlayerStatsManager.instance.HandleEquipmentChanges();

        }

        public void OnUnequip()
        {
            PlayerStatsManager.instance.HandleEquipmentChanges();

        }

    }

}
