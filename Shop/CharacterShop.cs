using System.Collections.Generic;
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

        [Header("Events")]
        public UnityEvent onShopOpen;
        public UnityEvent onShopExit;

        // Scene References
        public UIDocumentShopMenu uIDocumentShopMenu;

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
    }
}
