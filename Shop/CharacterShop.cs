using System.Collections.Generic;
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
        public List<ShopItemEntry> itemsToSell = new();

        [Header("Discount Settings")]
        public Item requiredItemForDiscounts;
        [Range(0.1f, 1f)] public float discountGivenByItemInInventory = 0.3f;

        [Header("Events")]
        public UnityEvent onShopOpen;
        public UnityEvent onShopExit;
    }
}
