
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Events;

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

        public AudioClip onConfirmSfx;

        VisualElement root;
        StarterAssetsInputs inputs;

        public VisualTreeAsset receivedItemPrefab;

        public List<ItemsReceived> itemsUI = new List<ItemsReceived>();

        public UnityEvent onConfirmEvent;

        private void Awake()
        {
            this.gameObject.SetActive(false);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
        }

        private void OnEnable()
        {
            this.root = uiDocument.rootVisualElement;

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

            var rootPanel = this.root.Q<VisualElement>("ReceivedItemsContainer");
            rootPanel.Clear();

            foreach (var itemUIEntry in itemsUI)
            {

                var clone = receivedItemPrefab.CloneTree();

                clone.Q<VisualElement>("ActionButtons").Q<Label>("ItemName").text = itemUIEntry.itemName;
                clone.Q<Label>("ItemQuantity").text = itemUIEntry.quantity.ToString();
                clone.Q<IMGUIContainer>("ItemSprite").style.backgroundImage = new StyleBackground(itemUIEntry.sprite);

                rootPanel.Add(clone);

            }
        }

        private void OnDisable()
        {
            if (onConfirmEvent != null)
            {
                onConfirmEvent?.Invoke();
                onConfirmEvent?.RemoveAllListeners();
            }
        }


        private void Update()
        {
            if (!this.gameObject.activeSelf)
            {
                return;
            }

            if (inputs.interact)
            {
                inputs.interact = false;

                BGMManager.instance.PlaySound(onConfirmSfx, null);
                this.gameObject.SetActive(false);
            }
        }

    }

}
