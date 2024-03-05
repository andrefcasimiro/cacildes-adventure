using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Item")]
    public class Item : ScriptableObject
    {

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

    }
}
