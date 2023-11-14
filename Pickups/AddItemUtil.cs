using System.Collections.Generic;
using AF.Inventory;
using UnityEngine;

namespace AF.Pickups
{
    public class AddItemUtil : MonoBehaviour
    {
        [Header("Data")]
        public ItemEntry[] itemsToAdd;

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
                    itemName = item.item.name.GetText(),
                    quantity = item.amount,
                    sprite = item.item.sprite
                });

                playerInventory.AddItem(item.item, item.amount);
            }

            uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
            uIDocumentReceivedItemPrompt.DisplayItemsReceived(itemsToDisplay);

            Soundbank.instance.PlayItemReceived();
        }
    }
}
