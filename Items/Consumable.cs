using UnityEngine;
using UnityEngine.Localization.Settings;

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

        public string GetFormattedRemovedStatusEffects()
        {
            string result = "";

            foreach (var statusEffectToRemove in statusesToRemove)
            {
                if (statusEffectToRemove != null)
                {
                    result += $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Removes Status:")} {statusEffectToRemove.GetName()}\n";
                }
            }

            return result.TrimEnd();
        }

        public string GetFormattedAppliedStatusEffects()
        {
            string result = "";

            foreach (var statusEffect in statusEffectsWhenConsumed)
            {
                if (statusEffect != null && statusEffect.GetName().Length > 0)
                {
                    result += $"{statusEffect.GetName()}\n";
                }
            }

            return result.TrimEnd();
        }
    }
}
