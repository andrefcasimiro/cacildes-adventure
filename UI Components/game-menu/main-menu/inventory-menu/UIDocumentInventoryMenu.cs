using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

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

        void UpdateGUI()
        {
            // Clean up scroll view contents
            this.itemListScrollView.Clear();

            bool isFirstButton = false;

            var itemsToShow = PlayerInventoryManager.instance.currentItems;

            if (this.showConsumableToggle.value == true)
            {
                itemsToShow = PlayerInventoryManager.instance.currentItems.Where(item =>
                {
                    Consumable consumable = item as Consumable;

                    if (consumable != null)
                    {
                        return true;
                    }

                    return false;
                }).ToList();
            }

            foreach (Item item in itemsToShow)
            {
                var btn = CreateButton(
                    item.name,
                    item.sprite,
                    // Click Callback
                    () =>
                    {
                        Consumable consumable = item as Consumable;

                        if (consumable != null)
                        {
                            consumable.OnConsume();

                            this.UpdateGUI();
                        }
                    },
                    // Focus Callback
                    () =>
                    {
                        if (item.sprite != null)
                        {
                            this.itemSpriteContainer.style.backgroundImage = new StyleBackground(item.sprite);
                            this.itemSpriteContainer.RemoveFromClassList("hide");
                        }
                        else
                        {
                            this.itemSpriteContainer.AddToClassList("hide");
                        }

                        this.itemDescription.text = item.description;
                    });

                Consumable consumable = item as Consumable;
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

        UnityEngine.UIElements.Button CreateButton(string name, Sprite itemSprite, UnityAction clickCallback, UnityAction focusCallback)
        {
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

            VisualElement favoriteSprite = new VisualElement();
            favoriteSprite.style.backgroundImage = new StyleBackground(favoriteIconSprite);
            favoriteSprite.style.height = 20;
            favoriteSprite.style.width = 20;
            btn.Add(favoriteSprite);

            /* var spriteContainer = btn.Q<VisualElement>("Icon");
            spriteContainer.style.backgroundImage = new StyleBackground(itemSprite);
            */

            this.SetupButtonClick(btn, clickCallback);

            btn.RegisterCallback<FocusEvent>(ev =>
            {
                focusCallback.Invoke();
            });

            btn.RegisterCallback<PointerEnterEvent>(ev =>
            {
                focusCallback.Invoke();
            });

            this.itemListScrollView.Add(btn);

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
