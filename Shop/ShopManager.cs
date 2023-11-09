using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class ShopEntryInstance
    {
        public string name;

        public int dayThatTradingBegan = -1; // -1 means the store has not been activated by the player (it is activated once the player begins trading)

        public List<ShopItem> itemStock = new();

        // The items collected here are never restocked. These are items sold by the player and he can buy them back at any time from the shop keeper
        public List<ShopItem> boughtItemsFromPlayer = new();

        public List<ShopItem> boughtItemsByPlayerThatDoNotRestock = new();


        public Item requiredItemForDiscounts;
        public float discountGivenByItemInInventory = 0.3f;
    }

    public class ShopManager : MonoBehaviour, ISaveable
    {
        public List<ShopEntryInstance> characterShopInstances = new();

        public static ShopManager instance;

        public float REPUTATION_MODIFIER = 2.25f;

        PlayerInventory playerInventory;

        public void Awake()
        {

            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

            LoadCharacterShops();
        }

        private void Start()
        {
            playerInventory = FindAnyObjectByType<PlayerInventory>(FindObjectsInactive.Include);
        }

        public void LoadCharacterShops()
        {
            characterShopInstances.Clear();
            foreach (var shopEntry in Resources.LoadAll<ShopEntry>("Shops"))
            {
                List<ShopItem> newDefaultStocksItems = new();
                foreach (var s in shopEntry.itemStock)
                {
                    newDefaultStocksItems.Add(new ShopItem()
                    {
                        item = s.item,
                        quantity = s.quantity,
                        priceModifier = s.priceModifier,
                        isRestockable = s.isRestockable,
                    });
                }

                characterShopInstances.Add(new ShopEntryInstance()
                {
                    name = shopEntry.name,
                    dayThatTradingBegan = -1,
                    itemStock = newDefaultStocksItems,
                    boughtItemsFromPlayer = new List<ShopItem>(),
                    boughtItemsByPlayerThatDoNotRestock = new(),
                    requiredItemForDiscounts = shopEntry.requiredItemForDiscounts,
                    discountGivenByItemInInventory = shopEntry.discountGivenByItemInInventory,
                }); ;
            }
        }

        public ShopEntryInstance GetShopInstanceByName(string shopEntryName)
        {
            return this.characterShopInstances.FirstOrDefault(x => x.name == shopEntryName);
        }

        public void ReplenishShopStock(string shopEntryName, List<ShopItem> defaultStockItems)
        {
            var idx = this.characterShopInstances.FindIndex(x => x.name == shopEntryName);

            this.characterShopInstances[idx].itemStock.Clear();

            List<ShopItem> newDefaultStocksItems = new();
            foreach (var defaultStockItem in defaultStockItems)
            {
                // Is it a non restockable item and has player bought it?
                if (!HasPlayerBoughtNonRestockableItem(this.characterShopInstances[idx], defaultStockItem))
                {
                    newDefaultStocksItems.Add(new ShopItem()
                    {
                        item = defaultStockItem.item,
                        quantity = defaultStockItem.quantity,
                        priceModifier = defaultStockItem.priceModifier,
                        isRestockable = defaultStockItem.isRestockable,
                    });
                }
            }

            this.characterShopInstances[idx].itemStock = newDefaultStocksItems;
            this.characterShopInstances[idx].dayThatTradingBegan = -1;
        }

        public bool HasPlayerBoughtNonRestockableItem(ShopEntryInstance characterShop, ShopItem item)
        {
            if (item.isRestockable)
            {
                return false;
            }

            return characterShop.boughtItemsByPlayerThatDoNotRestock.FirstOrDefault(
                        x => x.item.name.GetEnglishText() == item.item.name.GetEnglishText()) != null;
        }

        public bool HasStock(string shopName, ShopItem shopItem)
        {
            var shop = GetShopInstanceByName(shopName);

            if (shop == null)
            {
                return false;
            }

            var itemInStock = shop.itemStock.FirstOrDefault(x => x.item.name.GetEnglishText() == shopItem.item.name.GetEnglishText());
            if (itemInStock != null && itemInStock.quantity > 0)
            {
                return true;
            }

            var itemInBoughItems = shop.boughtItemsFromPlayer.FirstOrDefault(x => x.item.name.GetEnglishText() == shopItem.item.name.GetEnglishText());
            if (itemInBoughItems != null && itemInBoughItems.quantity > 0)
            {
                return true;
            }

            return false;
        }

        public bool BuyFromShop(string shopEntryName, ShopItem itemToBuy)
        {
            var idx = characterShopInstances.FindIndex(characterShopInstance => characterShopInstance.name == shopEntryName);

            var indexOfItemInStock = characterShopInstances[idx].itemStock.FindIndex(
                itemToBuyInStock => itemToBuyInStock.item.name.GetEnglishText() == itemToBuy.item.name.GetEnglishText());
            var indexOfItemBoughtItemsFromPlayer = characterShopInstances[idx].boughtItemsFromPlayer.FindIndex(
                itemToBuyInStock => itemToBuyInStock.item.name.GetEnglishText() == itemToBuy.item.name.GetEnglishText());

            if (indexOfItemInStock == -1 && indexOfItemBoughtItemsFromPlayer == -1)
            {
                return false;
            }

            if (indexOfItemInStock != -1)
            {
                this.characterShopInstances[idx].itemStock[indexOfItemInStock].quantity--;
            }
            else if (indexOfItemBoughtItemsFromPlayer != -1)
            {
                this.characterShopInstances[idx].boughtItemsFromPlayer[indexOfItemBoughtItemsFromPlayer].quantity--;
            }

            if (!itemToBuy.isRestockable)
            {
                this.characterShopInstances[idx].boughtItemsByPlayerThatDoNotRestock.Add(itemToBuy);
            }

            if (this.characterShopInstances[idx].dayThatTradingBegan == -1)
            {
                this.characterShopInstances[idx].dayThatTradingBegan = Player.instance.daysPassed;
            }

            // Retrieve coin to player
            var finalPrice = GetItemToBuyPrice(itemToBuy, this.characterShopInstances[idx]);

            FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGoldLost(finalPrice);
            Player.instance.currentGold -= finalPrice;

            // Give item to player
            playerInventory.AddItem(itemToBuy.item, 1);

            return true;
        }


        public bool SellToShop(string shopEntryName, Item itemToSell)
        {
            var idx = characterShopInstances.FindIndex(characterShopInstance => characterShopInstance.name == shopEntryName);

            // Update merchant inventory
            var idxOfItemBoughtFromPlayer = this.characterShopInstances[idx].boughtItemsFromPlayer.FindIndex(
                shopItem => shopItem.item.name.GetEnglishText() == itemToSell.name.GetEnglishText());
            if (idxOfItemBoughtFromPlayer != -1)
            {
                this.characterShopInstances[idx].boughtItemsFromPlayer[idxOfItemBoughtFromPlayer].quantity++;
            }
            else
            {
                this.characterShopInstances[idx].boughtItemsFromPlayer.Add(new()
                {
                    item = itemToSell,
                    quantity = 1,
                    priceModifier = 0,
                    isRestockable = false
                });
            }

            // Give coin to player
            var finalPrice = GetItemToSellPrice(itemToSell);

            FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGold(finalPrice);
            Player.instance.currentGold += finalPrice;

            return true;
        }

        public int GetItemToBuyPrice(ShopItem shopItem, ShopEntryInstance shopEntryInstance)
        {
            var finalPrice = (int)Mathf.Abs(Mathf.Round((shopItem.item.value + shopItem.priceModifier) - (Player.instance.GetCurrentReputation() * REPUTATION_MODIFIER)));


            if (finalPrice <= 0)
            {
                return 1;
            }


            if (shopEntryInstance.requiredItemForDiscounts != null)
            {
                if (shopEntryInstance.requiredItemForDiscounts is Helmet && Player.instance.equippedHelmet != null && Player.instance.equippedHelmet.name.GetEnglishText() == shopEntryInstance.requiredItemForDiscounts.name.GetEnglishText())
                {
                    finalPrice = (int)(finalPrice / shopEntryInstance.discountGivenByItemInInventory);
                }
            }

            return finalPrice;
        }

        public int GetItemToSellPrice(Item itemToSell)
        {
            var finalPrice = (int)Mathf.Abs(Mathf.Round((itemToSell.value) + (Player.instance.GetCurrentReputation() * REPUTATION_MODIFIER)));

            if (finalPrice <= 0)
            {
                return 1;
            }

            return finalPrice;
        }

        public void OnGameLoaded(object gameData)
        {
            Dictionary<string, Item> itemsTable = new();

            foreach (var item in Resources.LoadAll<Item>("Items"))
            {
                itemsTable.Add(item.name.GetEnglishText(), item);
            }

            // For v0.5 compatibility
            if (gameData.GetType().GetProperty("shopData") == null)
            {
                return;
            }

            /*
            foreach (var savedShop in gameData.shops)
            {
                var idx = this.characterShopInstances.FindIndex(shop => shop.name == savedShop.shopName);

                if (idx == -1)
                {
                    Debug.LogError("Could not find shop with name: " + savedShop.shopName);
                    return;
                }

                this.characterShopInstances[idx].dayThatTradingBegan = savedShop.dayThatTradingBegan;

                List<ShopItem> itemStock = new();
                foreach (var savedItemStock in savedShop.stockItems)
                {
                    var itemInStockIndex = itemStock.FindIndex(x => x.item.name.GetEnglishText() == savedItemStock.itemName);
                    if (itemInStockIndex != -1)
                    {
                        itemStock[itemInStockIndex].quantity++;
                    }
                    else
                    {
                        itemStock.Add(new ShopItem()
                        {
                            item = itemsTable[savedItemStock.itemName],
                            quantity = savedItemStock.itemCount,
                            priceModifier = savedItemStock.priceModifier,
                            isRestockable = savedItemStock.isRestockable,
                        });
                    }
                }
                this.characterShopInstances[idx].itemStock = itemStock;

                List<ShopItem> itemsBoughtFromPlayer = new();
                foreach (var savedItemBoughtFromPlayer in savedShop.itemsBoughtFromPlayer)
                {
                    var itemIndex = itemStock.FindIndex(x => x.item.name.GetEnglishText() == savedItemBoughtFromPlayer.itemName);
                    if (itemIndex != -1)
                    {
                        itemsBoughtFromPlayer[itemIndex].quantity++;
                    }
                    else
                    {
                        itemsBoughtFromPlayer.Add(new ShopItem()
                        {
                            item = itemsTable[savedItemBoughtFromPlayer.itemName],
                            quantity = savedItemBoughtFromPlayer.itemCount,
                            priceModifier = savedItemBoughtFromPlayer.priceModifier,
                            isRestockable = savedItemBoughtFromPlayer.isRestockable,
                        });
                    }
                }
                this.characterShopInstances[idx].boughtItemsFromPlayer = itemsBoughtFromPlayer;


                List<ShopItem> boughtItemsByPlayerThatDoNotRestock = new();
                foreach (var boughtItemByPlayerThatDoesNotRestock in savedShop.boughtItemsByPlayerThatDoNotRestock)
                {
                    var itemIndex = itemStock.FindIndex(x => x.item.name.GetEnglishText() == boughtItemByPlayerThatDoesNotRestock.itemName);
                    if (itemIndex != -1)
                    {
                        boughtItemsByPlayerThatDoNotRestock[itemIndex].quantity++;
                    }
                    else
                    {
                        boughtItemsByPlayerThatDoNotRestock.Add(new ShopItem()
                        {
                            item = itemsTable[boughtItemByPlayerThatDoesNotRestock.itemName],
                            quantity = boughtItemByPlayerThatDoesNotRestock.itemCount,
                            priceModifier = boughtItemByPlayerThatDoesNotRestock.priceModifier,
                            isRestockable = boughtItemByPlayerThatDoesNotRestock.isRestockable,
                        });
                    }
                }
                this.characterShopInstances[idx].boughtItemsByPlayerThatDoNotRestock = boughtItemsByPlayerThatDoNotRestock;

            }*/
        }
    }
}
