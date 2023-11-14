using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class AppliedConsumable
    {
        public Consumable.ConsumableEffect consumableEffect;

        public Sprite consumableEffectSprite;

        public string consumableItemName;

        public float currentDuration;

    }
}

