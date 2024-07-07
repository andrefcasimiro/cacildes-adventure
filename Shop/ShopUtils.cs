using System.Collections.Generic;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Shops
{
    public static class ShopUtils
    {

        public static int GetItemFinalPrice(Item item, bool playerIsBuying, float discountPercentage)
        {
            if (discountPercentage <= 0f)
            {
                return (int)item.value;
            }

            int discountAmount = (int)(item.value * discountPercentage);

            if (playerIsBuying)
            {
                return (int)(item.value - discountAmount);
            }
            else
            {
                return (int)(item.value + discountAmount);
            }
        }

        public static bool ItemRequiresCoinsToBeBought(Item item)
        {
            return item.tradingItemRequirements == null || item.tradingItemRequirements.Count <= 0;
        }

        public static void BuyItem(
            Item itemToBuy,
            UnityAction<int> onGoldLost,
            UnityAction<Dictionary<Item, int>> onItemsTraded,
            UnityAction<Item> onTransactionCompleted
        )
        {
            if (ItemRequiresCoinsToBeBought(itemToBuy))
            {
                onGoldLost((int)itemToBuy.value);
            }
            else
            {
                onItemsTraded(itemToBuy.tradingItemRequirements);
            }

            onTransactionCompleted(itemToBuy);
        }

        public static string GetItemDisplayName(Item item, bool playerIsBuying, InventoryDatabase inventoryDatabase, SerializedDictionary<Item, ShopItemEntry> npcItemsToSell)
        {
            if (item == null)
            {
                return "";
            }

            string itemName = item.GetName();

            if (playerIsBuying && npcItemsToSell.ContainsKey(item) && npcItemsToSell[item].quantity > 0)
            {
                itemName += $" ({npcItemsToSell[item].quantity})";
            }
            else if (item is not Weapon)
            {
                int itemAmount = inventoryDatabase.GetItemAmount(item);
                if (itemAmount > 0)
                {
                    itemName += $" ({itemAmount})";
                }
            }

            if (item is Weapon wp)
            {
                itemName += " +" + wp.level;
            }

            return itemName;
        }

    }
}
