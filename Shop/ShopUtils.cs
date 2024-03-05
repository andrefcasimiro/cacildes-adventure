using System.Collections.Generic;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine.Events;

namespace AF.Shops
{
    public static class ShopUtils
    {
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
