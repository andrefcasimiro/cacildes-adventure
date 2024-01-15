using System.Collections.Generic;
using AF.Inventory;
using UnityEngine;
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

        [Header("Systems")]
        public WorldSettings worldSettings;

        Label buyerName, buyerGold, sellerName, sellerGold;
        VisualElement buyerIcon, sellerIcon;

        // Item Preview
        VisualElement itemPreview;
        IMGUIContainer itemPreviewItemIcon;
        Label itemPreviewItemDescription;

        private void Start()
        {
            this.gameObject.SetActive(false);
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
            characterShop?.onShopOpen?.Invoke();

            gameObject.SetActive(true);

            cursorManager.ShowCursor();

            playerManager.playerComponentManager.DisableComponents();

            DrawBuyMenu(characterShop);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="characterShop"></param>
        public void SellToCharacter(CharacterShop characterShop)
        {
            characterShop?.onShopOpen?.Invoke();

            gameObject.SetActive(true);

            cursorManager.ShowCursor();

            playerManager.playerComponentManager.DisableComponents();

            DrawSellMenu(characterShop);
        }

        private void OnEnable()
        {
            SetupRefs();
        }

        void SetupExitButton(CharacterShop characterShop, ScrollView scrollView)
        {
            Button exitButton = new() { text = "Exit shop" };
            exitButton.AddToClassList("primary-button");

            UIUtils.SetupButton(exitButton, () =>
            {
                ExitShop();
                characterShop.onShopExit?.Invoke();
            }
            , () =>
            {
                root.Q<ScrollView>().ScrollTo(exitButton);
                exitButton.Focus();
            },
            () =>
            {

            },
            true, soundbank);

            exitButton.Focus();
            scrollView.Add(exitButton);
            scrollView.ScrollTo(exitButton);
        }

        void ExitShop()
        {
            playerManager.playerComponentManager.EnableComponents();
            cursorManager.HideCursor();
            this.gameObject.SetActive(false);
        }

        void SetupCharactersGUI(CharacterShop characterShop, bool playerIsBuying)
        {
            if (playerIsBuying)
            {
                buyerName.text = playerCharacter.name;
                buyerGold.text = playerStatsDatabase.gold.ToString();
                buyerIcon.style.backgroundImage = new StyleBackground(playerCharacter.avatar);

                sellerName.text = characterShop.character.name;
                sellerGold.text = characterShop.shopGold.ToString();
                sellerIcon.style.backgroundImage = new StyleBackground(characterShop.character.avatar);
            }
            else
            {
                buyerName.text = characterShop.character.name;
                buyerGold.text = characterShop.shopGold.ToString();
                buyerIcon.style.backgroundImage = new StyleBackground(characterShop.character.avatar);

                sellerName.text = playerCharacter.name;
                sellerGold.text = playerStatsDatabase.gold.ToString();
                sellerIcon.style.backgroundImage = new StyleBackground(playerCharacter.avatar);
            }
        }

        void DrawBuyMenu(CharacterShop characterShop)
        {
            SetupCharactersGUI(characterShop, true);

            List<Item> shopItemsToDisplay = new();

            foreach (var item in characterShop.itemsToSell)
            {
                if (item.isUnique && inventoryDatabase.HasItem(item.item))
                {
                    continue;
                }

                ShopItemEntry shopItemEntryClone = item;
                if (characterShop.requiredItemForDiscounts != null && inventoryDatabase.HasItem(characterShop.requiredItemForDiscounts))
                {
                    shopItemEntryClone.item.value *= characterShop.discountGivenByItemInInventory;
                }

                shopItemsToDisplay.Add(shopItemEntryClone.item);
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


                shopItemsToDisplay.Add(item.Key);
            }

            DrawItemsList(shopItemsToDisplay, false, characterShop);
        }

        void DrawItemsList(List<Item> itemsToSell, bool playerIsBuying, CharacterShop characterShop)
        {
            root.Q<ScrollView>().Clear();

            SetupExitButton(characterShop, root.Q<ScrollView>());

            foreach (var item in itemsToSell)
            {
                VisualElement cloneButton = buySellButton.CloneTree();
                cloneButton.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(item.sprite);
                cloneButton.Q<Label>("ItemName").text = item.name.GetEnglishText();

                bool canBuy = playerIsBuying && playerStatsDatabase.gold >= item.value;
                bool canSell = !playerIsBuying && characterShop.shopGold >= item.value;

                Button buySellItemButton = cloneButton.Q<Button>("BuySellItem");
                buySellItemButton.style.opacity = playerIsBuying && canBuy || !playerIsBuying && canSell ? 1 : 0.5f;
                buySellItemButton.text = (playerIsBuying ? "Buy " : "Sell ") + " (" + item.value + " Coins)";

                cloneButton.RegisterCallback<PointerEnterEvent>((ev) =>
                {
                    RenderItemPreview(item);
                });

                cloneButton.RegisterCallback<PointerOutEvent>((ev) =>
                {
                    HideItemPreview();
                });

                UIUtils.SetupButton(cloneButton.Q<Button>("BuySellItem"), () =>
                {
                    if (canBuy)
                    {
                        BuyItem(item, characterShop);
                    }
                    else if (canSell)
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

                root.Q<ScrollView>().Add(cloneButton);
            }

        }

        void BuyItem(Item item, CharacterShop characterShop)
        {

            if (playerStatsDatabase.gold >= item.value)
            {
                uIDocumentPlayerGold.LoseGold((int)item.value);
                characterShop.shopGold += (int)item.value;

                // Give item to player
                playerManager.playerInventory.AddItem(item, 1);

                soundbank.PlaySound(soundbank.uiItemReceived);
                notificationManager.ShowNotification("Bought " + item.name.GetEnglishText() + "", item.sprite);

                DrawBuyMenu(characterShop);
            }
        }

        void SellItem(Item item, CharacterShop characterShop)
        {
            uIDocumentPlayerGold.AddGold((int)item.value);
            characterShop.shopGold -= (int)item.value;

            // Give item to player
            playerManager.playerInventory.RemoveItem(item, 1);

            soundbank.PlaySound(soundbank.uiItemReceived);
            notificationManager.ShowNotification("Sold " + item.name.GetEnglishText() + "", item.sprite);

            DrawSellMenu(characterShop);
        }

        void RenderItemPreview(Item item)
        {
            if (item == null)
            {
                return;
            }

            itemPreviewItemIcon.style.backgroundImage = new StyleBackground(item.sprite);
            itemPreviewItemDescription.text = item.description.GetEnglishText();
            itemPreview.style.opacity = 1;
        }

        void HideItemPreview()
        {
            itemPreview.style.opacity = 0;
        }
    }
}
