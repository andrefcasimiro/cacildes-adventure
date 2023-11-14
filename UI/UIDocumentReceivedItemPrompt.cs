
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentReceivedItemPrompt : MonoBehaviour
    {
        [System.Serializable]
        public class ItemsReceived
        {
            public string itemName = "Item Name";
            public int quantity = 0;
            public Sprite sprite = null;
        }

        public UIDocument uiDocument;

        public VisualTreeAsset receivedItemPrefab;

        bool isPopupActive = false;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void DisplayItemsReceived(List<ItemsReceived> itemsReceived)
        {
            isPopupActive = false;

            var root = uiDocument.rootVisualElement;

            if (Gamepad.current != null)
            {
                root.Q<IMGUIContainer>("KeyboardIcon").style.display = DisplayStyle.None;
                root.Q<IMGUIContainer>("GamepadIcon").style.display = DisplayStyle.Flex;
            }
            else
            {
                root.Q<IMGUIContainer>("KeyboardIcon").style.display = DisplayStyle.Flex;
                root.Q<IMGUIContainer>("GamepadIcon").style.display = DisplayStyle.None;
            }

            var rootPanel = root.Q<VisualElement>("ReceivedItemsContainer");
            rootPanel.Clear();

            foreach (var itemUIEntry in itemsReceived)
            {

                var clone = receivedItemPrefab.CloneTree();

                clone.Q<VisualElement>("ActionButtons").Q<Label>("ItemName").text = itemUIEntry.itemName;
                clone.Q<Label>("ItemQuantity").text = itemUIEntry.quantity.ToString();
                clone.Q<IMGUIContainer>("ItemSprite").style.backgroundImage = new StyleBackground(itemUIEntry.sprite);

                rootPanel.Add(clone);
            }

            // Force the flag to be activated in the next frame so that OnInteract doesn't overlap with another OnInteract event of the same frame (i.e. Opening a chest)
            Invoke(nameof(SetIsPopupActiveToTrue), 0f);
        }

        void SetIsPopupActiveToTrue()
        {
            isPopupActive = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnInteract()
        {
            if (isPopupActive)
            {
                isPopupActive = false;

                gameObject.SetActive(false);
            }
        }

    }
}
