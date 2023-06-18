using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class ShopItem
    {
        public Item item;

        public int quantity;

        public int priceModifier = 0;

        public bool isRestockable = true;
    }

    [CreateAssetMenu(menuName = "Data / New Shop")]
    public class ShopEntry : ScriptableObject
    {
        // The items here are restocked if dayThatTradingBegan is != -1 and the daysToRespawnStock have passed
        public List<ShopItem> itemStock = new();
    }

}
