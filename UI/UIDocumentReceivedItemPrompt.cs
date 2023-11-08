using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Linq;
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

            root.Q<VisualElement>("AddToFavoriteItems").style.display = DisplayStyle.None;

            if (itemsUI.Count == 1)
            {
                var idx = Player.instance.favoriteItems.FindIndex(favItem => favItem.name.GetText() == itemsUI.ElementAt(0).itemName);
                if (idx == -1)
                {
                    var itemInstance = Player.instance.ownedItems.FirstOrDefault(x => x.item.name.GetText() == itemsUI[0].itemName);

                    if (itemInstance != null)
                    {
                        var consumable = itemInstance.item as Consumable;
                        if (consumable != null && Gamepad.current == null)
                        {

                            if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                            {
                                root.Q<VisualElement>("AddToFavoriteItems").Q<Label>("AddToFavoriteItemsLabel").text = "Adicionar aos favoritos";
                            }
                            else
                            {
                                root.Q<VisualElement>("AddToFavoriteItems").Q<Label>("AddToFavoriteItemsLabel").text = "Add to Favorite Items";
                            }

                            root.Q<VisualElement>("AddToFavoriteItems").style.display = DisplayStyle.Flex;

                        }
                    }

                }

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

            if (inputs.dodge || Input.GetKeyDown(KeyCode.F))
            {
                inputs.dodge = false;

                if (itemsUI.Count == 1)
                {
                    var idx = Player.instance.favoriteItems.FindIndex(favItem => favItem.name.GetText() == itemsUI.ElementAt(0).itemName);
                    if (idx == -1)
                    {
                        var itemInstance = Player.instance.ownedItems.FirstOrDefault(x => x.item.name.GetText() == itemsUI[0].itemName);

                        
                        if (itemInstance != null)
                        {
                            var consumable = itemInstance.item as Consumable;
                        
                            if (consumable != null)
                            {
                                FindObjectOfType<FavoriteItemsManager>(true).AddFavoriteItemToList(itemInstance.item);
                                FindObjectOfType<FavoriteItemsManager>(true).SwitchToFavoriteItem(itemInstance.item.name.GetText());
                            }
                        }

                        BGMManager.instance.PlaySound(onConfirmSfx, null);
                        this.gameObject.SetActive(false);
                    }

                }
            }
        }

    }

}
