using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AF
{
    [System.Serializable]
    public class AppliedStatus
    {
        public StatusEffect statusEffect;

        public bool hasReachedTotalAmount;

        public float currentAmount;
    }

    public class Player : MonoBehaviour, ISaveable
    {
        [System.Serializable]
        public class ItemEntry
        {
            public Item item;

            public int amount;
        }

        public static Player instance;

        [HideInInspector] public bool hasShownTitleScreen = false;

        [Header("World")]
        [Range(0, 24)] public float timeOfDay;
        public int daysPassed = 0;
        public float daySpeed = 0.005f;

        [Header("Stats")]
        public float currentHealth;
        public float currentStamina;
        public int currentReputation;
        public int currentGold;

        [Header("Attributes")]
        public int vitality = 0;
        public int endurance = 0;
        public int strength = 0;
        public int dexterity = 0;

        [Header("Inventory")]
        public List<ItemEntry> ownedItems = new List<ItemEntry>();
        public List<Item> favoriteItems = new List<Item>();
        public Weapon equippedWeapon;
        public Shield equippedShield;
        public Helmet equippedHelmet;
        public ArmorBase equippedArmor;
        public Gauntlet equippedGauntlets;
        public Legwear equippedLegwear;
        public Accessory equippedAccessory;

        [Header("Recipes")]
        public List<AlchemyRecipe> alchemyRecipes = new List<AlchemyRecipe>();

        [Header("Status Effects")]
        public List<AppliedStatus> appliedStatus = new List<AppliedStatus>();


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

        private void Start()
        {
            // Health & stamina logic, playern needs to be present in every game scene now because of this:
            currentHealth = FindObjectOfType<HealthStatManager>(true).GetMaxHealth();
            currentStamina = FindObjectOfType<StaminaStatManager>(true).GetMaxStamina();

            // Default Equipment

            if (equippedWeapon != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = equippedWeapon;
                i.amount = 1;
                ownedItems.Add(i);
            }
            if (equippedShield != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = equippedShield;
                i.amount = 1;
                ownedItems.Add(i);
            }
            if (equippedHelmet != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = equippedHelmet;
                i.amount = 1;
                ownedItems.Add(i);
            }
            if (equippedArmor != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = equippedArmor;
                i.amount = 1;
                ownedItems.Add(i);
            }
            if (equippedGauntlets != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = equippedGauntlets;
                i.amount = 1;
                ownedItems.Add(i);
            }
            if (equippedLegwear != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = equippedLegwear;
                i.amount = 1;
                ownedItems.Add(i);
            }
            if (equippedAccessory != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = equippedAccessory;
                i.amount = 1;
                ownedItems.Add(i);
            }
        }

        public void OnGameLoaded(GameData gameData)
        {
            PlayerData playerData = gameData.playerData;

            // Items
            ownedItems.Clear();
            favoriteItems.Clear();
            if (gameData.items.Length > 0)
            {
                var items = Resources.FindObjectsOfTypeAll<Item>();

                foreach (var serializedItem in gameData.items)
                {
                    var itemInstance = items.FirstOrDefault(i => i.name == serializedItem.itemName);

                    if (itemInstance != null)
                    {
                        var itemEntry = new ItemEntry();
                        itemEntry.item = itemInstance;
                        itemEntry.amount = serializedItem.itemCount;
                        this.ownedItems.Add(itemEntry);
                    }
                }

                if (gameData.favoriteItems.Length > 0)
                {
                    foreach (var serializedFavoriteItem in gameData.favoriteItems)
                    {
                        var itemInstance = items.FirstOrDefault(i => i.name == serializedFavoriteItem);

                        if (itemInstance != null)
                        {
                            this.favoriteItems.Add(itemInstance);
                        }
                    }
                }

                FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();
            }

            // Status Effects
            this.appliedStatus.Clear();

            if (gameData.statusEffects.Length > 0)
            {
                PlayerStatusManager playerStatusManager = FindObjectOfType<PlayerStatusManager>(true);
                foreach (var savedStatus in gameData.statusEffects)
                {
                    var statusEffect = Resources.Load<StatusEffect>("Status/" + savedStatus.statusEffectName);

                    playerStatusManager.InflictStatusEffect(statusEffect, savedStatus.currentAmount, savedStatus.hasReachedTotalAmount);
                }
            }

            // Stats
            currentGold = playerData.currentGold;
            currentHealth = playerData.currentHealth;
            currentStamina = playerData.currentStamina;
            currentReputation = playerData.currentReputation;
            vitality = playerData.vitality;
            endurance = playerData.endurance;
            strength = playerData.strength;
            dexterity = playerData.dexterity;

            StaminaStatManager staminaStatManager = FindObjectOfType<StaminaStatManager>(true);
            if (currentStamina < staminaStatManager.GetMaxStamina())
            {
                staminaStatManager.DecreaseStamina(0);
            }

            timeOfDay = gameData.timeOfDay;
            FindObjectOfType<DayNightManager>(true).timeOfDay = gameData.timeOfDay;
            daysPassed = gameData.daysPassed;


            // Alchemy Recipes
            this.alchemyRecipes.Clear();

            if (gameData.alchemyRecipes.Length > 0)
            {
                foreach (var alchemyRecipeName in gameData.alchemyRecipes)
                {
                    var alchemyRecipe = Resources.Load<AlchemyRecipe>("Recipes/Alchemy/" + alchemyRecipeName);
                    this.alchemyRecipes.Add(alchemyRecipe);
                }
            }

            /*
            this.appliedConsumables.Clear();
            foreach (var serializedConsumable in gameData.consumables)
            {
                var consumable = PlayerInventoryManager.instance.GetItem(serializedConsumable.consumableName);

                AppliedConsumable appliedConsumable = new AppliedConsumable();
                appliedConsumable.consumable = (Consumable)consumable;
                appliedConsumable.currentDuration = serializedConsumable.currentDuration;

                this.appliedConsumables.Add(appliedConsumable);
            }*/


        }
    }

}
