using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Item")]
    public class Item : ScriptableObject
    {

        [Header("Localization")]
        public LocalizedString nameLocalized;
        public LocalizedString descriptionLocalized;
        public LocalizedString shortDescriptionLocalized;


        [Header("General")]
        public Sprite sprite;
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string itemDescription;
        public string shortDescription;

        [Header("Value")]
        public float value = 0;
        public bool isRenewable = false;
        [Tooltip("If we want to buy this item on a shop, this will override their value when trading with an NPC. E.g. Buying a boss weapon by trading a boss soul")]
        public SerializedDictionary<Item, int> tradingItemRequirements = new();

        [Header("Debug")]
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string notes;
        [TextAreaAttribute(minLines: 1, maxLines: 2)] public string location;

        [Header("Stat")]
        public IntStat onFindingItemStat;


        public string GetName()
        {
            if (nameLocalized != null && nameLocalized.IsEmpty == false)
            {
                return nameLocalized.GetLocalizedString();
            }

            return name;
        }


        public string GetDescription()
        {
            if (descriptionLocalized != null && descriptionLocalized.IsEmpty == false)
            {
                return descriptionLocalized.GetLocalizedString();
            }

            return itemDescription;
        }

        public string GetShortDescription()
        {
            if (shortDescriptionLocalized != null && shortDescriptionLocalized.IsEmpty == false)
            {
                return shortDescriptionLocalized.GetLocalizedString();
            }

            return shortDescription;
        }
    }
}
