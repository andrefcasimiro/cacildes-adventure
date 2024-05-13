using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AF.Stats;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Shops
{
    public class CharacterShop : MonoBehaviour
    {

        [Header("Character")]
        public Character character;
        public int shopGold = 1500;

        [Header("Inventory")]
        public SerializedDictionary<Item, ShopItemEntry> itemsToSell = new();

        [Header("Discount Settings")]
        public Item requiredItemForDiscounts;
        [Range(0.1f, 1f)] public float discountGivenByItemInInventory = 0.3f;
        [Range(0.1f, 1f)] public float discountGivenByShopItself = 1f;

        [Header("Events")]
        public UnityEvent onShopOpen;
        public UnityEvent onShopExit;

        [Header("Selling Options")]
        public Item[] itemsThatCanBeSold;

        // Scene References
        UIDocumentShopMenu uIDocumentShopMenu;

        [Header("Quest Dependant Discounts")]
        public QuestParent questParent;
        public int[] questProgressesRequiredForDiscount;
        [Range(0.1f, 1f)] public float discountGivenByQuestCompleted = 1f;

        public void BuyFromCharacter()
        {
            GetUIDocumentShopMenu()?.BuyFromCharacter(this);
        }

        public void SellToCharacter()
        {
            GetUIDocumentShopMenu()?.SellToCharacter(this);
        }

        UIDocumentShopMenu GetUIDocumentShopMenu()
        {
            if (uIDocumentShopMenu == null)
            {
                uIDocumentShopMenu = FindAnyObjectByType<UIDocumentShopMenu>(FindObjectsInactive.Include);
            }

            return uIDocumentShopMenu;
        }

        public void RemoveItem(Item item, int amount)
        {
            if (!this.itemsToSell.ContainsKey(item))
            {
                return;
            }

            if (itemsToSell[item].quantity <= 1)
            {
                itemsToSell.Remove(item);
                return;
            }

            itemsToSell[item].quantity -= amount;
        }

        public void AddItem(Item item, int amount)
        {
            if (!itemsToSell.ContainsKey(item))
            {
                itemsToSell.Add(item, new() { dontShowIfPlayerAreadyOwns = false, quantity = 1 });
                return;
            }

            itemsToSell[item].quantity += amount;
        }

        public int GetItemEvaluation(Item item, InventoryDatabase inventoryDatabase, StatsBonusController statsBonusController, bool isBuying)
        {
            float discountPercentage = 0f;

            if (requiredItemForDiscounts != null && discountGivenByItemInInventory != 1 && inventoryDatabase.HasItem(requiredItemForDiscounts))
            {
                discountPercentage += 1 - discountGivenByItemInInventory;
            }

            if (discountGivenByShopItself != 1)
            {
                discountPercentage += 1 - discountGivenByShopItself;
            }

            if (discountGivenByQuestCompleted != 1
                && questParent != null
                && questProgressesRequiredForDiscount.Length > 0
                && questProgressesRequiredForDiscount.Contains(questParent.questProgress))
            {
                discountPercentage += 1 - discountGivenByQuestCompleted;
            }

            if (statsBonusController.discountPercentage > 0)
            {
                discountPercentage += 1 - statsBonusController.discountPercentage;
            }

            return ShopUtils.GetItemFinalPrice(item, isBuying, Mathf.Min(1f, discountPercentage));
        }

        List<string> GetDiscountDescriptions(InventoryDatabase inventoryDatabase, StatsBonusController statsBonusController, bool isBuying)
        {
            List<string> discountDescriptions = new();

            if (requiredItemForDiscounts != null && discountGivenByItemInInventory != 1 && inventoryDatabase.HasItem(requiredItemForDiscounts))
            {
                discountDescriptions.Add((isBuying ? "-" : "+") + (100 - discountGivenByItemInInventory * 100) + $"% prices for having {requiredItemForDiscounts.name} item in inventory");
            }
            if (discountGivenByShopItself != 1)
            {
                discountDescriptions.Add((isBuying ? "-" : "+") + (100 - discountGivenByShopItself * 100) + $"% prices from shop affinitiy towards Cacildes");
            }
            if (statsBonusController.discountPercentage > 0)
            {
                discountDescriptions.Add((isBuying ? "-" : "+") + (100 - statsBonusController.discountPercentage * 100) + $"% prices from player bonus stats");
            }
            if (discountGivenByQuestCompleted != 1
                && questParent != null
                && questProgressesRequiredForDiscount.Length > 0
                && questProgressesRequiredForDiscount.Contains(questParent.questProgress))
            {
                discountDescriptions.Add((isBuying ? "-" : "+") + (100 - discountGivenByQuestCompleted * 100) + $"% prices for completing quest: {questParent.name}");
            }

            return discountDescriptions;
        }

        public string GetShopDiscountsDescription(InventoryDatabase inventoryDatabase, StatsBonusController statsBonusController, bool isBuying)
        {
            List<string> discounts = GetDiscountDescriptions(inventoryDatabase, statsBonusController, isBuying);

            if (discounts.Count <= 0)
            {
                return "";
            }


            return "Applied discounts: \n" + string.Join("\n", discounts);
        }

    }
}
