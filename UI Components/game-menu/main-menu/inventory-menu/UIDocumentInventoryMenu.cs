using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

namespace AF
{

    public class UIDocumentInventoryMenu : UIDocumentScrollerBase
    {

        ScrollView itemListScrollView;

        IMGUIContainer itemSpriteContainer;

        Label itemDescription;

        UnityEngine.UIElements.Toggle showConsumableToggle;

        public Sprite favoriteIconSprite;

        protected override void Start()
        {
            base.Start();

            // EQUIPMENT SELECTION MENU SCREEN

            this.itemDescription = this.root.Q<Label>("SelectedItemDescription");
            this.itemSpriteContainer = this.root.Q<IMGUIContainer>("ItemSpriteContainer");
            this.itemListScrollView = this.root.Q<ScrollView>("ScrollView");
            this.showConsumableToggle = this.root.Q<UnityEngine.UIElements.Toggle>("ShowConsumableToggle");

            this.showConsumableToggle.RegisterValueChangedCallback((ev) => { UpdateGUI(); });
            this.showConsumableToggle.RegisterCallback<FocusEvent>((ev) => {
                
                this.itemDescription.text = "";
                this.itemSpriteContainer.AddToClassList("hide");
            });

            this.Disable();
        }

        private void Update()
        {
        }

        void UpdateGUI()
        {

            // Clean up scroll view contents
            this.itemListScrollView.Clear();

            bool isFirstButton = false;

            var itemsToShow = PlayerInventoryManager.instance.currentItems;

            if (this.showConsumableToggle.value == true)
            {
                itemsToShow = PlayerInventoryManager.instance.currentItems.Where(itemEntry =>
                {
                    Consumable consumable = itemEntry.item as Consumable;

                    if (consumable != null)
                    {
                        return true;
                    }

                    return false;
                }).ToList();
            }

            foreach (ItemEntry itemEntry in itemsToShow)
            {
                var btn = CreateButton(
                    itemEntry.item,
                    itemEntry.item.name + " (x" + itemEntry.amount + ")",
                    itemEntry.item.sprite,
                    // Click Callback
                    () =>
                    {
                        Consumable consumable = itemEntry.item as Consumable;

                        if (consumable != null)
                        {
                            consumable.OnConsume();

                            this.UpdateGUI();
                        }
                    },
                    // Focus Callback
                    () =>
                    {
                        if (itemEntry.item.sprite != null)
                        {
                            this.itemSpriteContainer.style.backgroundImage = new StyleBackground(itemEntry.item.sprite);
                            this.itemSpriteContainer.RemoveFromClassList("hide");
                        }
                        else
                        {
                            this.itemSpriteContainer.AddToClassList("hide");
                        }

                        this.itemDescription.text = itemEntry.item.description;
                    });

                Consumable consumable = itemEntry.item as Consumable;
                if (consumable == null)
                {
                    btn.style.opacity = 0.5f;
                }

                if (isFirstButton == false)
                {
                    btn.Focus();
                    isFirstButton = true;
                }
            }

        }

        UnityEngine.UIElements.Button CreateButton(Item item, string name, Sprite itemSprite, UnityAction clickCallback, UnityAction focusCallback)
        {
            VisualElement visualElementContainer = new VisualElement();
            visualElementContainer.style.flexDirection = FlexDirection.Row;

            UnityEngine.UIElements.Button btn = new UnityEngine.UIElements.Button();
            btn.text = "";
            btn.name = name.Replace(" ", "");

            btn.AddToClassList("game-button");
            btn.style.flexDirection = FlexDirection.Row;
            btn.style.alignItems = Align.Center;
            btn.style.justifyContent = Justify.FlexStart;

            VisualElement iconSprite = new VisualElement();
            iconSprite.style.backgroundImage = new StyleBackground(itemSprite);
            iconSprite.style.height = 30;
            iconSprite.style.width = 30;
            btn.Add(iconSprite);

            Label itemName = new Label();
            itemName.text = name;
            btn.Add(itemName);

            Button favoriteSpriteButton = new Button();
            favoriteSpriteButton.AddToClassList("game-button");

            VisualElement favoriteSprite = new VisualElement();
            favoriteSprite.style.backgroundImage = new StyleBackground(favoriteIconSprite);
            favoriteSprite.style.height = 20;
            favoriteSprite.style.width = 20;

            favoriteSpriteButton.Add(favoriteSprite);
            btn.Add(favoriteSpriteButton);


            SetupButtonClick(favoriteSpriteButton, () =>
            {
                // Only allow favorites on consumable items
                Consumable consumable = item as Consumable;

                if (consumable == null)
                {
                    return;
                }

                PlayerInventoryManager.instance.ToggleFavoriteItem(item);

                if (PlayerInventoryManager.instance.currentFavoriteItems.Exists(favItem => favItem == item))
                {
                    favoriteSprite.style.opacity = 1;
                }
                else
                {
                    favoriteSprite.style.opacity = .25f;
                }
            });

            if (PlayerInventoryManager.instance.currentFavoriteItems.Exists(favItem => favItem == item))
            {
                favoriteSprite.style.opacity = 1;
            }
            else
            {
                favoriteSprite.style.opacity = .25f;
            }

            this.SetupButtonClick(btn, clickCallback);

            btn.RegisterCallback<FocusEvent>(ev =>
            {
                focusCallback.Invoke();
            });

            btn.RegisterCallback<PointerEnterEvent>(ev =>
            {
                focusCallback.Invoke();
            });

            visualElementContainer.Add(btn);
            visualElementContainer.Add(favoriteSpriteButton);

            this.itemListScrollView.Add(visualElementContainer);

            return btn;
        }

        public override void Enable()
        {
            base.Enable();

            UpdateGUI();
        }

        public override void Disable()
        {
            base.Disable();
        }

    }
}
