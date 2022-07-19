using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AF
{

    public class PlayerInventoryManager : MonoBehaviour
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

        public List<Item> currentItems = new List<Item>();
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

        #region Default Equipment Logic
        void GetStartingItemsFromEquipmentManager()
        {
            if (currentWeapon != null)
            {
                this.currentItems.Add(currentWeapon);
            }
            if (currentShield != null)
            {
                this.currentItems.Add(currentShield);
            }
            if (currentHelmet != null)
            {
                this.currentItems.Add(currentHelmet);
            }
            if (currentChest != null)
            {
                this.currentItems.Add(currentChest);
            }
            if (currentGauntlets != null)
            {
                this.currentItems.Add(currentGauntlets);
            }
            if (currentLegwear != null)
            {
                this.currentItems.Add(currentLegwear);
            }
            if (currentAccessory1 != null)
            {
                this.currentItems.Add(currentAccessory1);
            }
            if (currentAccessory2 != null)
            {
                this.currentItems.Add(currentAccessory2);
            }
        }
        #endregion

        public int GetCurrentItemCount(Item item)
        {
            return this.currentItems.Count(entry => entry.name == item.name);
        }

        #region Get Items By Type
        public List<Weapon> GetWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (Item i in this.currentItems)
            {
                Weapon weapon = i as Weapon;
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

            foreach (Item i in this.currentItems)
            {
                Shield shield = i as Shield;
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

            foreach (Item i in this.currentItems)
            {
                Armor armor = i as Armor;
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

            foreach (Item i in this.currentItems)
            {
                Armor armor = i as Armor;
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

            foreach (Item i in this.currentItems)
            {
                Armor armor = i as Armor;
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

            foreach (Item i in this.currentItems)
            {
                Armor armor = i as Armor;
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

            foreach (Item i in this.currentItems)
            {
                Accessory accessory = i as Accessory;
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
            PlayerInventoryManager.instance.currentItems.Clear();
            PlayerInventoryManager.instance.currentFavoriteItems.Clear();

            foreach (SerializableItem serializableItem in gameData.items)
            {
                Item itemInstance = GetItem(serializableItem.itemName);

                if (itemInstance != null)
                {
                    for (int i = 0; i < serializableItem.itemCount; i++)
                    {
                        Debug.Log(itemInstance.name);
                        this.currentItems.Add(itemInstance);
                    }
                }
            }

            foreach (string favoriteItemName in gameData.favoriteItems)
            {
                Item itemInstance = this.currentItems.Find(item => item.name == favoriteItemName);

                this.currentFavoriteItems.Add(itemInstance);
            }
        }
        #endregion

        #region Database System
        public Item GetItem(string name)
        {
            return itemsDatabase.Find(item => item.name.Equals(name));
        }
        #endregion

    }

}
