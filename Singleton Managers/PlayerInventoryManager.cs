using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AF
{

    [System.Serializable]
    public class ItemEntry
    {
        public Item item;

        public int amount;
    }

    public class PlayerInventoryManager : InputListener, ISaveable
    {
        // Holds a database of all items. Used by the Save / Load system to have a reference of the corresponding item's scriptable object
        public List<Item> itemsDatabase = new List<Item>();

        public static PlayerInventoryManager instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            inputActions.PlayerActions.SwitchQuickItem.performed += ctx =>
            {
                Player player = FindObjectOfType<Player>(true);
                if (player.IsBusy())
                {
                    return;
                }

                SwitchFavoriteItem();
            };
            inputActions.PlayerActions.ConsumeQuickItem.performed += ctx =>
            {
                Player player = FindObjectOfType<Player>(true);
                if (player.IsBusy())
                {
                    return;
                }

                ConsumeFavoriteItem();
            };
        }

        public List<ItemEntry> currentItems = new List<ItemEntry>();
        public List<Item> currentFavoriteItems = new List<Item>();

        // EQUIPMENT
        public Weapon currentWeapon;
        public Weapon defaultUnarmedWeapon;
        public Shield currentShield;
        public Armor currentHelmet;
        public Armor currentChest;
        public Armor currentGauntlets;
        public Armor currentLegwear;
        public Accessory currentAccessory1;
        public Accessory currentAccessory2;

        private void Start()
        {
            // Get starting items from equipment manager
            GetStartingItemsFromEquipmentManager();

        }

        public void AddItem(Item item, int amount)
        {
            int itemEntryIndex = this.currentItems.FindIndex(_itemEntry => _itemEntry.item == item);

            if (itemEntryIndex != -1)
            {
                this.currentItems[itemEntryIndex].amount += amount;
            }
            else
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = item;
                itemEntry.amount = amount;

                this.currentItems.Add(itemEntry);
            }

            UpdateQuickItemsGUI();
        }

        public void RemoveItem(Item item, int amount)
        {
            int itemEntryIndex = this.currentItems.FindIndex(_itemEntry => _itemEntry.item == item);

            if (itemEntryIndex == -1)
            {
                return;
            }

            if (this.currentItems[itemEntryIndex].amount <= 1)
            {
                // Remove item 
                this.currentItems.RemoveAt(itemEntryIndex);

                // Remove item from favorite
                var idxOfThisItemInFavorites = this.currentFavoriteItems.IndexOf(item);
                if (idxOfThisItemInFavorites != -1)
                {
                    this.currentFavoriteItems.Remove(item);

                    if (idxOfThisItemInFavorites == 0)
                    {
                        SwitchFavoriteItem();
                    }
                }
            }
            else
            {
                this.currentItems[itemEntryIndex].amount -= amount;
            }

            UpdateQuickItemsGUI();
        }

        #region Favorite Items Logic
        public void ToggleFavoriteItem(Item item)
        {
            var idx = this.currentFavoriteItems.IndexOf(item);

            if (idx == -1)
            {
                this.currentFavoriteItems.Add(item);
            }
            else
            {
                this.currentFavoriteItems.Remove(item);
            }

            UpdateQuickItemsGUI();
        }

        public void FavoriteItemAndSetItAsCurrent(Item item)
        {
            var idx = this.currentFavoriteItems.IndexOf(item);

            if (idx != -1)
            {
                this.currentFavoriteItems.RemoveAt(idx);
            }

            // Reorder items
            Item[] cachedFavoriteItems = new Item[this.currentFavoriteItems.Count];
            this.currentFavoriteItems.CopyTo(cachedFavoriteItems);

            this.currentFavoriteItems.Clear();
            this.currentFavoriteItems.Add(item);
            foreach (var cachedFavoriteItem in cachedFavoriteItems)
            {
                this.currentFavoriteItems.Add(cachedFavoriteItem);
            }

            UpdateQuickItemsGUI();
        }

        public void SwitchFavoriteItem()
        {
            if (this.currentFavoriteItems.Count <= 0)
            {
                return;
            }

            Item firstItem = this.currentFavoriteItems.First();
            this.currentFavoriteItems.Remove(firstItem);
            this.currentFavoriteItems.Add(firstItem);

            UpdateQuickItemsGUI();
        }

        public void ConsumeFavoriteItem()
        {
            if (currentFavoriteItems.Count > 0)
            {
                Item currentItem = currentFavoriteItems[0];

                ItemEntry itemEntry = this.currentItems.Find(item => item.item.name == currentItem.name);

                if (itemEntry.amount <= 1)
                {
                    currentFavoriteItems.Remove(currentItem);                
                }

                Consumable consumableItem = (Consumable)currentItem;

                consumableItem.OnConsume();
            }

            UpdateQuickItemsGUI();
        }

        public void UpdateQuickItemsGUI()
        {
            UIDocumentPlayerHUD uiDocumentPlayerHud = FindObjectOfType<UIDocumentPlayerHUD>(true);
            if (uiDocumentPlayerHud != null)
            {
                uiDocumentPlayerHud.UpdateQuickItems();
            }
        }

        #endregion

        #region Default Equipment Logic
        void GetStartingItemsFromEquipmentManager()
        {
            if (currentWeapon != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentWeapon;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
            if (currentShield != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentShield;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
            if (currentHelmet != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentHelmet;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
            if (currentChest != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentChest;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
            if (currentGauntlets != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentGauntlets;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
            if (currentLegwear != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentLegwear;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
            if (currentAccessory1 != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentAccessory1;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
            if (currentAccessory2 != null)
            {
                ItemEntry itemEntry = new ItemEntry();
                itemEntry.item = currentAccessory2;
                itemEntry.amount = 1;

                this.currentItems.Add(itemEntry);
            }
        }
        #endregion

        #region Get Items By Type
        public List<Weapon> GetWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (ItemEntry i in this.currentItems)
            {
                Weapon weapon = i.item as Weapon;
                if (weapon != null)
                {
                    weapons.Add(weapon);
                }
            }

            return weapons;
        }

        public List<Shield> GetShields()
        {
            List<Shield> shields = new List<Shield>();

            foreach (ItemEntry i in this.currentItems)
            {
                Shield shield = i.item as Shield;
                if (shield != null)
                {
                    shields.Add(shield);
                }
            }

            return shields;
        }

        public List<Armor> GetHelmets()
        {
            List<Armor> helmets = new List<Armor>();

            foreach (ItemEntry i in this.currentItems)
            {
                Armor armor = i.item as Armor;
                if (armor != null)
                {
                    if (armor.armorType == ArmorSlot.Head)
                    {
                        helmets.Add(armor);
                    }
                }
            }

            return helmets;
        }

        public List<Armor> GetChestpieces()
        {
            List<Armor> chestpieces = new List<Armor>();

            foreach (ItemEntry i in this.currentItems)
            {
                Armor armor = i.item as Armor;
                if (armor != null)
                {
                    if (armor.armorType == ArmorSlot.Chest)
                    {
                        chestpieces.Add(armor);
                    }
                }
            }

            return chestpieces;
        }

        public List<Armor> GetGauntlets()
        {
            List<Armor> gauntlets = new List<Armor>();

            foreach (ItemEntry i in this.currentItems)
            {
                Armor armor = i.item as Armor;
                if (armor != null)
                {
                    if (armor.armorType == ArmorSlot.Arms)
                    {
                        gauntlets.Add(armor);
                    }
                }
            }

            return gauntlets;
        }

        public List<Armor> GetLegwear()
        {
            List<Armor> legwear = new List<Armor>();

            foreach (ItemEntry i in this.currentItems)
            {
                Armor armor = i.item as Armor;
                if (armor != null)
                {
                    if (armor.armorType == ArmorSlot.Legs)
                    {
                        legwear.Add(armor);
                    }
                }
            }

            return legwear;
        }

        public List<Accessory> GetAccessories()
        {
            List<Accessory> accessories = new List<Accessory>();

            foreach (ItemEntry i in this.currentItems)
            {
                Accessory accessory = i.item as Accessory;
                if (accessory != null)
                {
                    accessories.Add(accessory);
                }
            }

            return accessories;
        }
        #endregion

        #region Serialization
        public void OnGameLoaded(GameData gameData)
        {
            currentItems.Clear();
            currentFavoriteItems.Clear();

            foreach (SerializableItem serializableItem in gameData.items)
            {
                Item itemInstance = GetItem(serializableItem.itemName);

                if (itemInstance != null)
                {
                    ItemEntry itemEntry = new ItemEntry();
                    itemEntry.item = itemInstance;
                    itemEntry.amount = serializableItem.itemCount;
                    this.currentItems.Add(itemEntry);
                }
            }

            foreach (string favoriteItemName in gameData.favoriteItems)
            {
                Item itemInstance = GetItem(favoriteItemName);

                this.currentFavoriteItems.Add(itemInstance);
            }
        }
        #endregion

        public bool HasItem(Item item)
        {
            return this.currentItems.Exists(x => x.item == item);
        }

        #region Database System
        public Item GetItem(string name)
        {
            return itemsDatabase.Find(item => item.name.Equals(name));
        }
        #endregion


    }

}
