using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Consumable")]
    public class Consumable : Item
    {
        [Header("FX")]

        [Header("Remove Negative Status")]
        public StatusEffect[] statusesToRemove;

        [Header("Options")]
        public bool shouldNotRemoveOnUse = false;
        public bool shouldHideEquipmentWhenConsuming = true;
        public bool isBossToken = false;
        public bool canBeConsumedForGold = false;

        [Header("Consume Effects")]
        public StatusEffect[] statusEffectsWhenConsumed;
        public float effectsDurationInSeconds = 6;
    }
}
