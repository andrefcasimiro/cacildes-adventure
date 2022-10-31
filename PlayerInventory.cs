using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PlayerInventory : MonoBehaviour
    {
        FavoriteItemsManager favoriteItemsManager => GetComponent<FavoriteItemsManager>();

        public void AddItem(Item item, int quantity)
        {
            Player.ItemEntry itemEntry = new Player.ItemEntry();
            itemEntry.item = item;
            itemEntry.amount = quantity;

            var idx = Player.instance.ownedItems.FindIndex(x => x.item == item);
            if (idx != -1)
            {
                Player.instance.ownedItems[idx].amount += itemEntry.amount;
            }
            else
            {
                Player.instance.ownedItems.Add(itemEntry);
            }

            FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();
        }

        public void RemoveItem(Item item, int amount)
        {
            int itemEntryIndex = Player.instance.ownedItems.FindIndex(_itemEntry => _itemEntry.item == item);

            if (itemEntryIndex == -1)
            {
                return;
            }

            if (Player.instance.ownedItems[itemEntryIndex].amount <= 1)
            {
                // Remove item 
                Player.instance.ownedItems.RemoveAt(itemEntryIndex);

                // Remove item from favorite
                var idxOfThisItemInFavorites = Player.instance.favoriteItems.IndexOf(item);
                if (idxOfThisItemInFavorites != -1)
                {
                    Player.instance.favoriteItems.Remove(item);

                    if (idxOfThisItemInFavorites == 0)
                    {
                        favoriteItemsManager.SwitchFavoriteItemsOrder();
                    }
                }
            }
            else
            {
                Player.instance.ownedItems[itemEntryIndex].amount -= amount;
            }

            FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();
        }

        public List<Weapon> GetWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleWeapon = item.item as Weapon;
                if (possibleWeapon != null)
                {
                    weapons.Add(possibleWeapon);
                }
            }

            return weapons;
        }

        public List<Helmet> GetHelmets()
        {
            List<Helmet> items = new List<Helmet>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Helmet;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }

        public List<Armor> GetArmors()
        {
            List<Armor> items = new List<Armor>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Armor;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }

        public List<Legwear> GetLegwears()
        {
            List<Legwear> items = new List<Legwear>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Legwear;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }


        public List<Gauntlet> GetGauntlets()
        {
            List<Gauntlet> items = new List<Gauntlet>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Gauntlet;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }
    }

}
