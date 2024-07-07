using System;
using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace AF.Shops
{
    public class UIDocumentShopMenu : MonoBehaviour
    {
        [Header("Player Settings")]
        public Character playerCharacter;

        [Header("Components")]
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public Soundbank soundbank;

        [Header("UI Components")]
        public UIDocument uiDocument;
        public VisualTreeAsset buySellButton;
        public UIDocumentPlayerGold uIDocumentPlayerGold;
        public NotificationManager notificationManager;
        VisualElement root;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

        Label buyerName, buyerGold, sellerName, sellerGold;
        VisualElement buyerIcon, sellerIcon;

        // Item Preview
        VisualElement itemPreview;
        IMGUIContainer itemPreviewItemIcon;
        Label itemPreviewItemDescription;

        // Last scroll position
        int lastScrollElementIndex = -1;

        // Memoizations
        CharacterShop currentCharacterShop;

        public LocalizedString buyFor_LocalizedString; // Buy for
        public LocalizedString sellFor_LocalizedString; // Sell for
        public LocalizedString coins_LocalizedString; // Coins
        public LocalizedString offer_LocalizedString; // Offer
        public LocalizedString exitShopLabel_LocalizedString; // Exit Shop

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            currentCharacterShop = null;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                ExitShop();
            }
        }

        void SetupRefs()
        {
            this.root = uiDocument.rootVisualElement;

            var buyer = root.Q<VisualElement>("Buyer");
            buyerName = buyer.Q<Label>("Name");
            buyerGold = buyer.Q<Label>("Gold");
            buyerIcon = buyer.Q<VisualElement>("BuyerIcon");

            var seller = root.Q<VisualElement>("Seller");
            sellerName = seller.Q<Label>("Name");
            sellerGold = seller.Q<Label>("Gold");
            sellerIcon = seller.Q<VisualElement>("SellerIcon");

            this.itemPreview = root.Q<VisualElement>("ItemPreview");
            this.itemPreviewItemIcon = itemPreview.Q<IMGUIContainer>("ItemIcon");
            this.itemPreviewItemDescription = itemPreview.Q<Label>("Description");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="characterShop"></param>
        public void BuyFromCharacter(CharacterShop characterShop)
        {
            currentCharacterShop = characterShop;

            characterShop?.onShopOpen?.Invoke();
            gameObject.SetActive(true);
            playerManager.playerComponentManager.DisableComponents();

            Invoke(nameof(DisplayCursor), 0f);
            DrawBuyMenu(characterShop);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="characterShop"></param>
        public void SellToCharacter(CharacterShop characterShop)
        {
            currentCharacterShop = characterShop;

            characterShop?.onShopOpen?.Invoke();
            gameObject.SetActive(true);
            playerManager.playerComponentManager.DisableComponents();

            Invoke(nameof(DisplayCursor), 0f);
            DrawSellMenu(characterShop);
        }

        void DisplayCursor()
        {
            cursorManager.ShowCursor();
        }

        private void OnEnable()
        {
            SetupRefs();
            DisplayCursor();
        }

        Button SetupExitButton(ScrollView scrollView)
        {
            Button exitButton = new() { text = exitShopLabel_LocalizedString.GetLocalizedString() };
            exitButton.AddToClassList("primary-button");

            UIUtils.SetupButton(exitButton, () =>
            {
                ExitShop();
            }
            ,
            () =>
            {
                root.Q<ScrollView>().ScrollTo(exitButton);
                exitButton.Focus();
            },
            () =>
            {

            },
            true, soundbank);

            scrollView.Add(exitButton);

            return exitButton;
        }

        void ExitShop()
        {
            currentCharacterShop?.onShopExit?.Invoke();

            playerManager.playerComponentManager.EnableComponents();
            cursorManager.HideCursor();
            this.gameObject.SetActive(false);
        }

        void SetupCharactersGUI(CharacterShop characterShop, bool playerIsBuying)
        {
            if (playerIsBuying)
            {
                buyerName.text = playerManager.playerAppearance.GetPlayerName();
                buyerGold.text = playerStatsDatabase.gold.ToString() + " " + coins_LocalizedString.GetLocalizedString();
                buyerIcon.style.backgroundImage = new StyleBackground(playerManager.playerAppearance.GetPlayerPortrait());

                sellerName.text = characterShop.character.GetCharacterName();
                sellerGold.text = characterShop.shopGold.ToString();
                sellerIcon.style.backgroundImage = new StyleBackground(characterShop.character.avatar);
            }
            else
            {
                buyerName.text = characterShop.character.GetCharacterName();
                buyerGold.text = characterShop.shopGold.ToString() + " " + coins_LocalizedString.GetLocalizedString();
                buyerIcon.style.backgroundImage = new StyleBackground(characterShop.character.avatar);

                sellerName.text = playerManager.playerAppearance.GetPlayerName();
                sellerGold.text = playerStatsDatabase.gold.ToString();
                sellerIcon.style.backgroundImage = new StyleBackground(playerManager.playerAppearance.GetPlayerPortrait());
            }

            root.Q<Label>("AppliedDiscountsLabel").text = characterShop.GetShopDiscountsDescription(inventoryDatabase, playerManager.statsBonusController, playerIsBuying);
        }

        void DrawBuyMenu(CharacterShop characterShop)
        {
            SetupCharactersGUI(characterShop, true);

            List<Item> shopItemsToDisplay = new();

            foreach (var item in characterShop.itemsToSell)
            {
                Item clonedItem = item.Key;

                if (item.Value.dontShowIfPlayerAreadyOwns && inventoryDatabase.HasItem(item.Key))
                {
                    continue;
                }

                if (item.Key.tradingItemRequirements != null && item.Key.tradingItemRequirements.Count() > 0)
                {
                    bool hasAllItems = true;

                    foreach (var requiredItemForTrading in item.Key.tradingItemRequirements)
                    {
                        if (!inventoryDatabase.HasItem(requiredItemForTrading.Key))
                        {
                            hasAllItems = false;
                        }
                    }

                    if (!hasAllItems)
                    {
                        continue;
                    }
                }

                shopItemsToDisplay.Add(clonedItem);
            }

            DrawItemsList(shopItemsToDisplay, true, characterShop);
        }

        void DrawSellMenu(CharacterShop characterShop)
        {
            SetupCharactersGUI(characterShop, false);

            List<Item> shopItemsToDisplay = new();

            foreach (var item in inventoryDatabase.ownedItems)
            {
                if (item.Key is KeyItem)
                {
                    continue;
                }

                // Don't sell unique items like spells and so on
                if (item.Key.isRenewable)
                {
                    continue;
                }

                if (characterShop.itemsThatCanBeSold != null && characterShop.itemsThatCanBeSold.Length > 0 && !characterShop.itemsThatCanBeSold.Contains(item.Key))
                {
                    continue;
                }

                if (item.Key is Weapon)
                {
                    for (int i = 0; i < item.Value.amount; i++)
                    {
                        shopItemsToDisplay.Add(item.Key);
                    }
                }
                else
                {
                    shopItemsToDisplay.Add(item.Key);
                }
            }



            DrawItemsList(shopItemsToDisplay, false, characterShop);
        }

        void DrawBuySellLabel(Button buySellButton, Item item, bool isPlayerBuying, CharacterShop characterShop)
        {
            buySellButton.Q<VisualElement>("RequiredItemSprite").style.display = DisplayStyle.None;
            buySellButton.Q<VisualElement>("OriginalValueContainer").style.display = DisplayStyle.None;

            Label buySellLabel = buySellButton.Q<Label>("BuySellLabel");
            Label currentValueLabel = buySellButton.Q<Label>("CurrentValue");

            if (ShopUtils.ItemRequiresCoinsToBeBought(item))
            {
                int finalValue = characterShop.GetItemEvaluation(item, inventoryDatabase, playerManager.statsBonusController, isPlayerBuying);

                if (item.value != finalValue)
                {
                    buySellButton.Q<Label>("OriginalValue").text = item.value.ToString();
                    buySellButton.Q<VisualElement>("OriginalValueContainer").style.display = DisplayStyle.Flex;
                }

                buySellLabel.text = (isPlayerBuying ? buyFor_LocalizedString.GetLocalizedString() : sellFor_LocalizedString.GetLocalizedString()) + " ";
                currentValueLabel.text = finalValue + " " + coins_LocalizedString.GetLocalizedString();
            }
            else if (item.tradingItemRequirements != null && item.tradingItemRequirements.Count > 0)
            {
                buySellButton.Q<VisualElement>("RequiredItemSprite").style.backgroundImage = new StyleBackground(item.tradingItemRequirements.ElementAt(0).Key.sprite);
                buySellButton.Q<VisualElement>("RequiredItemSprite").style.display = DisplayStyle.Flex;
                buySellLabel.text = offer_LocalizedString.GetLocalizedString() + " ";
                currentValueLabel.text = item.tradingItemRequirements.ElementAt(0).Key.GetName() + "";
            }
        }

        bool PlayerCanBuy(CharacterShop characterShop, Item item)
        {
            if (item.tradingItemRequirements != null && item.tradingItemRequirements.Count > 0)
            {
                bool canBuy = true;

                foreach (var requiredTradingItem in item.tradingItemRequirements)
                {
                    if (
                        !inventoryDatabase.HasItem(requiredTradingItem.Key)
                        || inventoryDatabase.ownedItems[requiredTradingItem.Key].amount < requiredTradingItem.Value)
                    {
                        canBuy = false;
                        break;
                    }
                }

                return canBuy;
            }

            int finalValue = characterShop.GetItemEvaluation(item, inventoryDatabase, playerManager.statsBonusController, true);

            return playerStatsDatabase.gold >= finalValue;
        }

        bool ShopCanBuy(CharacterShop characterShop, Item item)
        {
            int finalValue = characterShop.GetItemEvaluation(item, inventoryDatabase, playerManager.statsBonusController, false);

            return characterShop.shopGold >= finalValue;
        }

        void DrawItemsList(List<Item> itemsToSell, bool playerIsBuying, CharacterShop characterShop)
        {
            root.Q<ScrollView>().Clear();

            HideItemPreview();

            Button exitButton = SetupExitButton(root.Q<ScrollView>());

            int i = 0;
            foreach (var item in itemsToSell)
            {
                int currentIndex = i;

                VisualElement cloneButton = buySellButton.CloneTree();
                Button buySellItemButton = cloneButton.Q<Button>("BuySellButton");

                cloneButton.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(item.sprite);
                cloneButton.Q<Label>("ItemName").text = ShopUtils.GetItemDisplayName(item, playerIsBuying, inventoryDatabase, characterShop.itemsToSell);

                bool playerCanBuy = playerIsBuying && PlayerCanBuy(characterShop, item);
                bool playerCanSell = !playerIsBuying && ShopCanBuy(characterShop, item);

                buySellItemButton.style.opacity = (playerIsBuying && playerCanBuy || !playerIsBuying && playerCanSell) ? 1 : 0.5f;

                DrawBuySellLabel(buySellItemButton, item, playerIsBuying, characterShop);

                buySellItemButton.RegisterCallback<PointerEnterEvent>((ev) =>
                {
                    RenderItemPreview(item);
                });

                buySellItemButton.RegisterCallback<PointerOutEvent>((ev) =>
                {
                    HideItemPreview();
                });

                UIUtils.SetupButton(buySellItemButton,
                () =>
                {
                    lastScrollElementIndex = currentIndex;

                    if (playerCanBuy)
                    {
                        BuyItem(item, characterShop);
                    }
                    else if (playerCanSell)
                    {
                        SellItem(item, characterShop);
                    }
                },
                () =>
                {
                    RenderItemPreview(item);

                    root.Q<ScrollView>().ScrollTo(buySellItemButton);
                    buySellItemButton.Focus();
                },
                () =>
                {
                    HideItemPreview();
                },
                false, soundbank);

                root.Q<ScrollView>().Add(buySellItemButton);

                i++;
            }

            if (lastScrollElementIndex == -1)
            {
                exitButton.Focus();
            }

            Invoke(nameof(GiveFocus), 0f);
        }

        void GiveFocus()
        {
            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                root.Q<ScrollView>(),
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }

        void BuyItem(Item item, CharacterShop characterShop)
        {
            int price = characterShop.GetItemEvaluation(
                item,
                inventoryDatabase,
                playerManager.statsBonusController,
                true);

            if (!PlayerCanBuy(characterShop, item))
            {
                return;
            }

            ShopUtils.BuyItem(
                item,
                (goldLost) =>
                {

                    uIDocumentPlayerGold.LoseGold(price);
                    characterShop.shopGold += price;
                },
                (onItemsTraded) =>
                {
                    foreach (var tradedItem in onItemsTraded)
                    {

                        playerManager.playerInventory.RemoveItem(tradedItem.Key, tradedItem.Value);
                    }
                },
                (receivedItem) =>
                {
                    // Give item to player
                    playerManager.playerInventory.AddItem(item, 1);
                    soundbank.PlaySound(soundbank.uiItemReceived);
                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Bought") + " " + item.GetName() + "", item.sprite);

                    characterShop.RemoveItem(receivedItem, 1);

                    DrawBuyMenu(characterShop);
                }
            );
        }

        void SellItem(Item item, CharacterShop characterShop)
        {
            int price = characterShop.GetItemEvaluation(
                        item,
                        inventoryDatabase,
                        playerManager.statsBonusController,
                        false);


            uIDocumentPlayerGold.AddGold(price);
            characterShop.shopGold -= price;

            // Remove item from player
            playerManager.playerInventory.RemoveItem(item, 1);

            // Give item to NPC
            characterShop.AddItem(item, 1);

            soundbank.PlaySound(soundbank.uiItemReceived);
            notificationManager.ShowNotification(
                LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Sold") + " " + item.GetName() + "", item.sprite);

            DrawSellMenu(characterShop);
        }

        void RenderItemPreview(Item item)
        {
            if (item == null)
            {
                return;
            }

            itemPreviewItemIcon.style.backgroundImage = new StyleBackground(item.sprite);
            itemPreviewItemDescription.text = item.GetDescription();
            itemPreview.style.opacity = 1;
        }

        void HideItemPreview()
        {
            itemPreview.style.opacity = 0;
        }
    }
}
