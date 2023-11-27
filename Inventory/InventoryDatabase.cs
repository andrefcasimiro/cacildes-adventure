using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AF.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Database", menuName = "System/New Inventory Database", order = 0)]
    public class InventoryDatabase : ScriptableObject
    {
        public List<ItemEntry> ownedItems = new();

        public bool shouldClearOnExitPlayMode = false;

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode && shouldClearOnExitPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }

        public void Clear()
        {
            ownedItems.Clear();
        }

        public void RemoveItem(Item itemToRemove, int quantity)
        {
            int idx = this.ownedItems.FindIndex(item => item.item == itemToRemove);
            if (idx == -1)
            {
                return;
            }

            this.ownedItems[idx].amount -= quantity;

            if (
                this.ownedItems[idx].amount <= 0
                && this.ownedItems[idx].item.lostUponUse)
            {
                this.ownedItems.RemoveAt(idx);
            }
        }

        public int GetItemAmount(Item itemToFind)
        {
            int idx = this.ownedItems.FindIndex(item => item.item == itemToFind);
            if (idx == -1)
            {
                return 0;
            }

            return this.ownedItems[idx].amount;
        }
    }
}
