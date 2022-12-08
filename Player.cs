using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AF
{
    [System.Serializable]
    public class AppliedStatus
    {
        public StatusEffect statusEffect;

        public bool hasReachedTotalAmount;

        public float currentAmount;
    }

    [System.Serializable]
    public class AppliedConsumable
    {
        public Consumable.ConsumableEffect consumableEffect;

        public Sprite consumableEffectSprite;

        public float currentDuration;
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
        public List<CookingRecipe> cookingRecipes = new List<CookingRecipe>();

        [Header("Status Effects")]
        public List<AppliedStatus> appliedStatus = new List<AppliedStatus>();

        [Header("Consumables Effects")]
        public List<AppliedConsumable> appliedConsumables = new List<AppliedConsumable>();

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FindObjectOfType<DayNightManager>(true).SetTimeOfDay(Mathf.RoundToInt(Player.instance.timeOfDay) - 1, 0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                FindObjectOfType<DayNightManager>(true).SetTimeOfDay(Mathf.RoundToInt(Player.instance.timeOfDay) + 1, 0);
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
                var accessories = Resources.LoadAll<Accessory>("Items/Accessories");
                var alchemy = Resources.LoadAll<AlchemyIngredient>("Items/Alchemy");
                var armors = Resources.LoadAll<Armor>("Items/Armors");
                var consumables = Resources.LoadAll<Consumable>("Items/Consumables");
                var craftables = Resources.LoadAll<CraftingMaterial>("Items/Craftables");
                var gauntlets = Resources.LoadAll<Gauntlet>("Items/Gauntlets");
                var helmets = Resources.LoadAll<Helmet>("Items/Helmets");
                var legwears = Resources.LoadAll<Legwear>("Items/Legwears");
                var shields = Resources.LoadAll<Shield>("Items/Shields");
                var weapons = Resources.LoadAll<Weapon>("Items/Weapons");

                foreach (var serializedItem in gameData.items)
                {
                    object itemInstance = null;
                    if (itemInstance == null)
                    {
                        itemInstance = accessories.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = alchemy.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = armors.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = consumables.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = craftables.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = gauntlets.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = helmets.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = legwears.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = shields.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = weapons.FirstOrDefault(i => i.name == serializedItem.itemName);
                    }

                    if (itemInstance != null)
                    {
                        var itemEntry = new ItemEntry();
                        itemEntry.item = (Item)itemInstance;
                        itemEntry.amount = serializedItem.itemCount;
                        this.ownedItems.Add(itemEntry);
                    }
                }

                if (gameData.favoriteItems.Length > 0)
                {
                    foreach (var serializedFavoriteItem in gameData.favoriteItems)
                    {
                        var favoriteItem = this.ownedItems.Find(x => x.item.name == serializedFavoriteItem);

                        if (favoriteItem != null)
                        {
                            this.favoriteItems.Add(favoriteItem.item);
                        }
                    }
                }

                FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(true);
                FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();
            }

            // Status Effects
            this.appliedStatus.Clear();
            FindObjectOfType<UIDocumentStatusEffectV2>(true).ClearAllNegativeStatusEntry();

            if (gameData.statusEffects.Length > 0)
            {
                PlayerStatusManager playerStatusManager = FindObjectOfType<PlayerStatusManager>(true);
                foreach (var savedStatus in gameData.statusEffects)
                {
                    var statusEffect = Resources.Load<StatusEffect>("Status/" + savedStatus.statusEffectName);

                    playerStatusManager.InflictStatusEffect(statusEffect, savedStatus.currentAmount, savedStatus.hasReachedTotalAmount);
                }
            }

            // Status Effects
            this.appliedConsumables.Clear();
            FindObjectOfType<UIDocumentStatusEffectV2>(true).ClearAllConsumableEntries();

            if (gameData.consumables.Length > 0)
            {
                var playerConsumableManager = FindObjectOfType<PlayerConsumablesManager>(true);
                foreach (var savedConsumable in gameData.consumables)
                {
                    AppliedConsumable appliedConsumable = new AppliedConsumable();
                    Consumable.ConsumableEffect consumableEffect = new Consumable.ConsumableEffect();
                    Enum.TryParse(savedConsumable.consumableName, out Consumable.ConsumablePropertyName consumableName);

                    consumableEffect.consumablePropertyName = consumableName;
                    consumableEffect.displayName = savedConsumable.displayName;
                    consumableEffect.barColor = new Color(savedConsumable.barColor.r, savedConsumable.barColor.g, savedConsumable.barColor.b, savedConsumable.barColor.a);
                    consumableEffect.value = savedConsumable.value;
                    consumableEffect.effectDuration = savedConsumable.effectDuration;


                    appliedConsumable.currentDuration = savedConsumable.currentDuration;

                    var sprite = Resources.Load<Sprite>("Items/Consumables/" + savedConsumable.spriteFileName);

                    consumableEffect.sprite = sprite;

                    appliedConsumable.consumableEffect = consumableEffect;
                    appliedConsumable.consumableEffectSprite = sprite;

                    playerConsumableManager.AddConsumableEffect(appliedConsumable);
                }
            }

            // Stats
            currentGold = SaveSystem.instance.loadingFromGameOver ? 0 : playerData.currentGold;

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

            // Cooking Recipes
            this.cookingRecipes.Clear();

            if (gameData.cookingRecipes.Length > 0)
            {
                foreach (var recipeName in gameData.cookingRecipes)
                {
                    var recipe = Resources.Load<CookingRecipe>("Recipes/Cooking/" + recipeName);
                    this.cookingRecipes.Add(recipe);
                }
            }

        }
    }

}
