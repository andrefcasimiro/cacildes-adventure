using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

namespace AF.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Database", menuName = "System/New Inventory Database", order = 0)]
    public class InventoryDatabase : ScriptableObject
    {

        [Header("Inventory")]
        [SerializedDictionary("Item", "Quantity")]
        public SerializedDictionary<Item, ItemAmount> ownedItems = new();

        public SerializedDictionary<Item, ItemAmount> defaultItems = new();

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;


#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif
        public void Clear()
        {
            ownedItems.Clear();
        }

        public void SetDefaultItems()
        {
            ownedItems.Clear();

            foreach (var defaultItem in defaultItems)
            {
                ownedItems.Add(defaultItem.Key, defaultItem.Value);

                if (defaultItem.Key is Armor armor)
                {
                    equipmentDatabase.EquipArmor(armor);
                }
                else if (defaultItem.Key is Legwear legwear)
                {
                    equipmentDatabase.EquipLegwear(legwear);
                }
            }
        }

        public void ReplenishItems()
        {
            foreach (var item in ownedItems)
            {
                if (item.Value.usages > 0)
                {
                    item.Value.amount += item.Value.usages;
                    item.Value.usages = 0;
                }
            }
        }

        public void AddItem(Item itemToAdd)
        {
            AddItem(itemToAdd, 1);
        }

        public void AddItem(Item itemToAdd, int quantity)
        {

            if (HasItem(itemToAdd))
            {
                ownedItems[itemToAdd].amount += quantity;
            }
            else
            {
                ownedItems.Add(itemToAdd, new ItemAmount() { amount = quantity, usages = 0 });
            }
        }

        public void RemoveItem(Item itemToAdd)
        {
            RemoveItem(itemToAdd, 1);
        }

        public void RemoveItem(Item itemToRemove, int quantity)
        {
            if (!ownedItems.ContainsKey(itemToRemove))
            {
                return;
            }

            if (ownedItems[itemToRemove].amount <= 1)
            {
                // If not reusable item
                if (itemToRemove.isRenewable)
                {
                    ownedItems[itemToRemove].amount = 0;
                    ownedItems[itemToRemove].usages++;
                }
                else
                {
                    UnequipItemToRemove(itemToRemove);

                    // Remove item 
                    ownedItems.Remove(itemToRemove);
                }
            }
            else
            {
                ownedItems[itemToRemove].amount -= quantity;

                if (itemToRemove.isRenewable)
                {
                    ownedItems[itemToRemove].usages++;
                }
            }
        }

        void UnequipItemToRemove(Item item)
        {
            equipmentDatabase.UnequipItem(item);
        }

        public int GetItemAmount(Item itemToFind)
        {
            if (!ownedItems.ContainsKey(itemToFind))
            {
                return -1;
            }

            return this.ownedItems[itemToFind].amount;
        }

        public bool HasItem(Item itemToFind)
        {
            return this.ownedItems.ContainsKey(itemToFind);
        }

        public int GetWeaponsCount()
        {
            return ownedItems.Count(x => x.Key is Weapon);
        }

        public int GetSpellsCount()
        {
            return ownedItems.Count(x => x.Key is Spell);
        }
    }
}
