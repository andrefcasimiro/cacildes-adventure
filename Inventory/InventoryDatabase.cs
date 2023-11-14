using System.Collections.Generic;
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
    }
}
