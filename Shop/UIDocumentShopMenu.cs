using StarterAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentShopMenu : MonoBehaviour
    {

        [Header("Day / Night Manager")]
        public int daysToRestock = 3;
        [Range(0, 23)] public int hourToReplenish = 6;

        public ShopEntry shopEntry;

        [Header("Localization")]
        public LocalizedText shopName;

        public bool isBuying = true;

        UIDocument uiDocument => GetComponent<UIDocument>();
        VisualElement root;


        public VisualTreeAsset buySellButton;

        StarterAssetsInputs inputs => FindObjectOfType<StarterAssetsInputs>(true);

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void Open()
        {
            Utils.ShowCursor();

            if (HasReplenished())
            {
                Replenish();
            }

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                ExitShop();
            }
        }

        void ExitShop()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uiDocument.rootVisualElement;

            root.Q<Button>("ButtonExit").text = LocalizedTerms.ExitShop();
            root.Q<Button>("ButtonExit").RegisterCallback<ClickEvent>((ev) => { ExitShop(); });


            if (isBuying)
            {
                DrawBuyMenu();
            }
            else
            {
                DrawSellMenu();
            }
        }

        void DrawBuyMenu()
        {
            root.Q<Label>("ShopName").text = shopName.GetText();

            List<ShopItem> itemsToShow = new();

            var shopEntry = ShopManager.instance.GetShopInstanceByName(this.shopEntry.name);

            foreach (var stockItem in shopEntry.itemStock)
            {
                itemsToShow.Add(stockItem);
            }
            foreach (var boughtItemFromPlayer in shopEntry.boughtItemsFromPlayer)
            {
                itemsToShow.Add(boughtItemFromPlayer);
            }

            root.Q<Label>("PlayerGold").text = LocalizedTerms.YourCurrentGold() + ": " + Player.instance.currentGold + " " + LocalizedTerms.Coins();

            UpdateItemsList(itemsToShow);
        }

        void DrawSellMenu()
        {

        }

        void UpdateItemsList(List<ShopItem> itemsToShow)
        {
            root.Q<ScrollView>().Clear();

            foreach (var item in itemsToShow)
            {
                VisualElement cloneButton = buySellButton.CloneTree();
                cloneButton.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(item.item.sprite);
                cloneButton.Q<Label>("ItemName").text = item.item.name.GetText() + " ( " + item.quantity + " )";

                bool canBuy = item.quantity > 0 && Player.instance.currentGold >= ShopManager.instance.GetItemToBuyPrice(item);

                cloneButton.Q<Button>("BuySellItem").SetEnabled(canBuy);
                cloneButton.Q<Button>("BuySellItem").style.opacity = (canBuy ? 1 : 0.5f);
                cloneButton.Q<Button>("BuySellItem").text = LocalizedTerms.Buy() + " (" + ShopManager.instance.GetItemToBuyPrice(item) + " " + LocalizedTerms.Coins() + ")";

                cloneButton.RegisterCallback<PointerOverEvent>(ev =>
                {
                    RenderItemPreview(root, item.item);
                });

                cloneButton.RegisterCallback<PointerOutEvent>(ev =>
                {
                    HideItemPreview( root);
                });

                cloneButton.RegisterCallback<FocusInEvent>(ev =>
                {
                    RenderItemPreview(root, item.item);

                    root.Q<ScrollView>().ScrollTo(cloneButton);
                });

                cloneButton.RegisterCallback<FocusOutEvent>(ev =>
                {
                    HideItemPreview(root);
                });

                cloneButton.RegisterCallback<ClickEvent>(ev =>
                {
                    BuyItem(item);
                });

                root.Q<ScrollView>().Add(cloneButton);
            }

        }

        void BuyItem(ShopItem item)
        {
            if (Player.instance.currentGold >= ShopManager.instance.GetItemToBuyPrice(item))
            {
                if (!ShopManager.instance.HasStock(shopEntry.name, item))
                {
                    return;
                }

                ShopManager.instance.BuyFromShop(shopEntry.name, item);
                DrawBuyMenu();
            }
        }

        void RenderItemPreview(VisualElement root, Item item)
        {
            var itemPreview = root.Q<VisualElement>("ItemPreview");
            itemPreview.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(item.sprite);
            itemPreview.Q<Label>("Title").text = item.name.GetText();
            itemPreview.Q<Label>("Description").text = item.description.GetText();
            itemPreview.style.opacity = 1;
        }

        void HideItemPreview(VisualElement root)
        {
            root.Q<VisualElement>("ItemPreview").style.opacity = 0;
        }

        public void OnHourChanged()
        {
            if (HasReplenished())
            {
                // If is only one day passed, only enable after passing the hour threshold
                var shopRefreshValue = ShopManager.instance.GetShopInstanceByName(shopEntry.name).dayThatTradingBegan;

                if ((Player.instance.daysPassed - 1) == shopRefreshValue)
                {
                    if (Player.instance.timeOfDay >= hourToReplenish)
                    {
                        Replenish();
                    }
                }
                else
                {
                    Replenish();
                }
            }
        }

        void Replenish()
        {
            ShopManager.instance.ReplenishShopStock(shopEntry.name, shopEntry.itemStock);
        }

        bool HasReplenished()
        {
            var shopRefreshValue = ShopManager.instance.GetShopInstanceByName(shopEntry.name).dayThatTradingBegan;

            if (Player.instance.daysPassed > shopRefreshValue + daysToRestock)
            {
                return true;
            }

            return false;
        }

    }

}
