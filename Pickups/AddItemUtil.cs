using System.Collections.Generic;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace AF.Pickups
{
    public class AddItemUtil : MonoBehaviour
    {
        [Header("Data")]

        [Header("Effect Instances")]
        [SerializedDictionary("Item", "Quantity")]
        public SerializedDictionary<Item, ItemAmount> itemsToAdd;

        [Header("UI Components")]
        public UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;

        [Header("Components")]
        public PlayerInventory playerInventory;
        public Soundbank soundbank;

        public void OnAddItem()
        {
            List<UIDocumentReceivedItemPrompt.ItemsReceived> itemsToDisplay = new();

            foreach (var item in itemsToAdd)
            {
                if (item.Value.chanceToGet != 100)
                {
                    int chance = Random.Range(0, 100);
                    if (chance > item.Value.chanceToGet)
                    {
                        continue;
                    }
                }

                itemsToDisplay.Add(new()
                {
                    itemName = item.Key.name,
                    quantity = item.Value.amount,
                    sprite = item.Key.sprite
                });

                playerInventory.AddItem(item.Key, item.Value.amount);
            }

            uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
            uIDocumentReceivedItemPrompt.DisplayItemsReceived(itemsToDisplay);

            soundbank.PlaySound(soundbank.uiItemReceived);
        }
    }
}
