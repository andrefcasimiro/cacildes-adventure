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

            float discountAmount = item.value * discountPercentage;

            // Adjust discount amount based on whether player is buying or selling
            if (playerIsBuying)
            {
                discountAmount = -discountAmount; // Negate discount for buying
            }

            float finalPrice = item.value + discountAmount;

            // Ensure final price is not lower than 0
            finalPrice = Mathf.Max(finalPrice, 0f);

            return (int)finalPrice;
        }


        public static bool CanBuy(Item itemToBuy, int characterGold)
        {
            return CanBuy(itemToBuy, characterGold, null);
        }

        public static bool CanBuy(
            Item itemToBuy,
            int characterGold,
            SerializedDictionary<Item, ItemAmount> characterInventory)
        {
            if (itemToBuy.tradingItemRequirements != null && itemToBuy.tradingItemRequirements.Count > 0)
            {
                if (characterInventory == null)
                {
                    return false;
                }

                bool canBuy = true;

                foreach (var requiredTradingItem in itemToBuy.tradingItemRequirements)
                {
                    if (
                        !characterInventory.ContainsKey(requiredTradingItem.Key)
                        || characterInventory[requiredTradingItem.Key].amount < requiredTradingItem.Value)
                    {
                        canBuy = false;
                        break;
                    }
                }

                return canBuy;
            }

            return characterGold >= itemToBuy.value;
        }

        public static bool ItemRequiresCoinsToBeBought(Item item)
        {
            return item.tradingItemRequirements == null || item.tradingItemRequirements.Count <= 0;
        }

        public static void BuyItem(
            Item itemToBuy,
            int buyerGold,
            SerializedDictionary<Item, ItemAmount> buyerInventory,
            UnityAction<int> onGoldLost,
            UnityAction<Dictionary<Item, int>> onItemsTraded,
            UnityAction<Item> onTransactionCompleted
        )
        {
            if (!CanBuy(itemToBuy, buyerGold, buyerInventory))
            {
                return;
            }

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

            string itemName = item.name;

            if (playerIsBuying && npcItemsToSell.ContainsKey(item) && npcItemsToSell[item].quantity > 0)
            {
                itemName += $" ({npcItemsToSell[item].quantity})";
            }
            else
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
