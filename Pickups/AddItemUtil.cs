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

        public void OnAddItem()
        {

            List<UIDocumentReceivedItemPrompt.ItemsReceived> itemsToDisplay = new();

            foreach (var item in itemsToAdd)
            {
                itemsToDisplay.Add(new()
                {
                    itemName = item.Key.name.GetEnglishText(),
                    quantity = item.Value.amount,
                    sprite = item.Key.sprite
                });

                playerInventory.AddItem(item.Key, item.Value.amount);
            }

            uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
            uIDocumentReceivedItemPrompt.DisplayItemsReceived(itemsToDisplay);

            Soundbank.instance.PlayItemReceived();
        }
    }
}
