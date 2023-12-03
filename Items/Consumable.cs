using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Consumable")]
    public class Consumable : Item
    {
        public enum OnConsumeActionType
        {
            EAT,
            DRINK,
            CLOCK
        }

        [Header("FX")]
        public GameObject graphic;
        public GameObject particleOnConsume;
        public AudioClip sfxOnConsume;
        public bool destroyItemOnConsumeMoment = true;

        [Header("Remove Negative Status")]
        public StatusEffect[] statusesToRemove;

        [Header("Options")]
        public bool shouldNotRemoveOnUse = false;

        public bool isAlcoholic = false;

        [Header("Consume Effects")]
        public StatusEffect[] statusEffectsWhenConsumed;
        public float effectsDuration = 60f;

        public OnConsumeActionType onConsumeActionType = OnConsumeActionType.DRINK;
    }
}
