using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

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

        public string consumableItemName;

        public float currentDuration;

    }

    public class Player : MonoBehaviour, ISaveable
    {
        [System.Serializable]
        public class ItemEntry
        {
            public Item item;

            public int amount;

            // only applicable to items that are not lost upon use
            public int usages = 0;
        }

        public static Player instance;

        public bool isDemo = false;

        [Header("World")]
        [Range(0, 24)] public float timeOfDay;
        public int daysPassed = 0;
        public float daySpeed = 0.005f;

        [Header("Stats")]
        public float currentHealth;
        public int currentReputation;
        public int currentGold;


        [Header("Inventory")]
        public List<ItemEntry> ownedItems = new List<ItemEntry>();
        public List<Item> favoriteItems = new List<Item>();
        public Weapon equippedWeapon;
        public Shield equippedShield;
        public Helmet equippedHelmet;
        public ArmorBase equippedArmor;
        public Gauntlet equippedGauntlets;
        public Legwear equippedLegwear;
        public List<Accessory> equippedAccessories = new();

        public ArmorBase defaultArmor;
        public Legwear defaultLegwear;

        [Header("Recipes")]
        public List<AlchemyRecipe> alchemyRecipes = new List<AlchemyRecipe>();
        public List<CookingRecipe> cookingRecipes = new List<CookingRecipe>();

        [Header("Status Effects")]
        public List<AppliedStatus> appliedStatus = new List<AppliedStatus>();

        [Header("Consumables Effects")]
        public List<AppliedConsumable> appliedConsumables = new List<AppliedConsumable>();

        [Header("Enemies Scale")]
        public float enemiesHealthMultiplierPerLevel = 0.25f;
        public float enemiesAttackMultiplierPerLevel = 0.15f;
        public float enemiesPostureMultiplierPerLevel = 0.15f;

        [Header("Unlocked Bonfires")]
        public List<string> unlockedBonfires = new();
        public bool unlockAllBonfires = false;

        [Header("In Game Stats")]
        public bool playerHasDied = false;
        public bool playerWasHit = false;
        public bool memorizePlayerDeath = false;
        public bool memorizePlayerWasHit = false;

        public string currentObjective = "";

        public List<object> companions = new List<object>();

        // For companion comments
        public Enemy lastEnemyKilled;

        // Scene References
        EquipmentGraphicsHandler equipmentGraphicsHandler;

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
            var copiedOwnedItems = ownedItems.ToArray();
            ownedItems.Clear();
            foreach (var ownedItem in copiedOwnedItems)
            {
                int baseAmount = ownedItem.amount;

                ownedItems.Add(new ItemEntry()
                {
                    item = Instantiate(ownedItem.item),
                    amount = baseAmount,
                    usages = ownedItem.usages,
                });
            }

            defaultArmor = equippedArmor;
            defaultLegwear = equippedLegwear;

            // Health & stamina logic, playern needs to be present in every game scene now because of this:
            currentHealth = FindObjectOfType<HealthStatManager>(true).GetMaxHealth();

            // Default Equipment

            if (equippedWeapon != null)
            {
                ItemEntry i = new ItemEntry();
                i.item = Instantiate(equippedWeapon);
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
            if (equippedAccessories.Count > 0)
            {
                foreach (var equippedAcc in equippedAccessories)
                {
                    ItemEntry i = new ItemEntry();
                    i.item = equippedAcc;
                    i.amount = 1;
                    ownedItems.Add(i);
                }
            }
        }

        // TODO: Remove on build
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
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                FindObjectOfType<DayNightManager>(true).SetTimeOfDay(Mathf.RoundToInt(Player.instance.timeOfDay) + 12, 0);
                foreach (var iClockListener in FindObjectsOfType<MonoBehaviour>(true).OfType<IClockListener>())
                {
                    iClockListener.OnHourChanged();
                }

            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                unlockAllBonfires = !unlockAllBonfires;
            }
        }

        public void UnlockBonfire(string bonfireName)
        {
            if (this.unlockedBonfires.Contains(bonfireName))
            {
                return;
            }

            this.unlockedBonfires.Add(bonfireName);
        }

        public void OnGameLoaded(object gameData)
        {
            /*
            playerHasDied = playerHasDied || gameData.playerHasDied;

            playerWasHit = memorizePlayerWasHit || gameData.playerWasHit;

            PlayerData playerData = gameData.playerData;

            // Bonfires
            unlockedBonfires.Clear();
            unlockedBonfires = gameData.unlockedBonfires.ToList();

            // Companions
            //var companions = Resources.LoadAll<Companion>("Companions");
            companions.Clear();
            companions = gameData.companions.ToList();

            // Items
            ownedItems.Clear();
            favoriteItems.Clear();

            if (gameData.items.Length > 0)
            {
                var accessories = Resources.LoadAll<Accessory>("Items/Accessories");
                var alchemy = Resources.LoadAll<AlchemyIngredient>("Items/Alchemy");
                var cookingIngredients = Resources.LoadAll<CookingIngredient>("Items/Cooking");
                var armors = Resources.LoadAll<Armor>("Items/Armors");
                var consumables = Resources.LoadAll<Consumable>("Items/Consumables");
                var gauntlets = Resources.LoadAll<Gauntlet>("Items/Gauntlets");
                var helmets = Resources.LoadAll<Helmet>("Items/Helmets");
                var legwears = Resources.LoadAll<Legwear>("Items/Legwears");
                var shields = Resources.LoadAll<Shield>("Items/Shields");
                var spells = Resources.LoadAll<Spell>("Items/Spells");
                var weapons = Resources.LoadAll<Weapon>("Items/Weapons");
                var keyItems = Resources.LoadAll<Item>("Items/Key Items");
                var upgradeMaterials = Resources.LoadAll<UpgradeMaterial>("Upgrade Materials");

                foreach (var serializedItem in gameData.items)
                {
                    object itemInstance = null;
                    if (itemInstance == null)
                    {
                        itemInstance = accessories.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = alchemy.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = cookingIngredients.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = armors.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = consumables.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = gauntlets.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = helmets.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = legwears.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = shields.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = spells.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        var originalItemInstance = weapons.FirstOrDefault(i => serializedItem.itemName.StartsWith(i.name.GetEnglishText()));

                        if (originalItemInstance != null)
                        {
                            itemInstance = Instantiate(originalItemInstance);
                            var idx = serializedItem.itemName.IndexOf("+");
                            var weaponLevel = idx != -1 ? serializedItem.itemName.Substring(idx) : null;
                            if (!string.IsNullOrEmpty(weaponLevel))
                            {
                                if (itemInstance != null)
                                {
                                    var itemEntry = new ItemEntry();

                                    var weapon = (Weapon)itemInstance;
                                    int.TryParse(weaponLevel.Replace("+", "").Trim(), out int result);

                                    if (result > 0)
                                    {
                                        weapon.level = result;
                                    }

                                    itemEntry.item = weapon;

                                    int amount = itemEntry.amount;
                                    itemEntry.amount = amount;
                                    itemEntry.usages = serializedItem.itemUsage;
                                    this.ownedItems.Add(itemEntry);

                                    continue;
                                }
                            }

                        }
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = keyItems.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }
                    if (itemInstance == null)
                    {
                        itemInstance = upgradeMaterials.FirstOrDefault(i => i.name.GetEnglishText() == serializedItem.itemName);
                    }

                    if (itemInstance != null)
                    {
                        var itemEntry = new ItemEntry();
                        itemEntry.item = (Item)itemInstance;
                        itemEntry.amount = serializedItem.itemCount;
                        itemEntry.usages = serializedItem.itemUsage;
                        this.ownedItems.Add(itemEntry);
                    }
                }

                if (gameData.favoriteItems.Length > 0)
                {
                    foreach (var serializedFavoriteItem in gameData.favoriteItems)
                    {
                        var favoriteItem = this.ownedItems.Find(x => x.item.name.GetEnglishText() == serializedFavoriteItem);

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
                    Enum.TryParse(savedConsumable.displayName, out Consumable.ConsumablePropertyName consumableName);

                    consumableEffect.consumablePropertyName = consumableName;
                    Consumable consumableScriptableObject = Instantiate(Resources.Load<Consumable>("Items/Consumables/" + savedConsumable.consumableItemName));

                    var targetConsumableEffect = consumableScriptableObject.consumableEffects.FirstOrDefault(x => x.consumablePropertyName == consumableName);

                    targetConsumableEffect.barColor = new Color(savedConsumable.barColor.r, savedConsumable.barColor.g, savedConsumable.barColor.b, savedConsumable.barColor.a);
                    targetConsumableEffect.value = savedConsumable.value;
                    targetConsumableEffect.effectDuration = savedConsumable.effectDuration;
                    appliedConsumable.currentDuration = savedConsumable.currentDuration;

                    appliedConsumable.consumableEffect = targetConsumableEffect;

                    appliedConsumable.consumableEffectSprite = targetConsumableEffect.sprite;
                    appliedConsumable.consumableItemName = savedConsumable.consumableItemName;
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
            intelligence = playerData.intelligence;

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

            this.currentObjective = gameData.currentObjective;

            GamePreferences.instance.UpdateGraphics();
            GamePreferences.instance.UpdateAudio();

            */
        }

        #region Accessories Logic
        public bool IsAccessoryEquiped(Accessory accessory)
        {
            return GetEquippedAccessoryIndex(accessory) != -1;
        }

        public int GetEquippedAccessoryIndex(Accessory accessory)
        {
            return Player.instance.equippedAccessories.FindIndex(x => x.name.GetEnglishText() == accessory.name.GetEnglishText());
        }
        #endregion

        #region Recipe
        public void LearnRecipe(CraftingRecipe recipe)
        {
            if (this.cookingRecipes.Contains(recipe) || this.alchemyRecipes.Contains(recipe))
            {
                return;
            }

            var cookingRecipe = (CookingRecipe)recipe;
            if (cookingRecipe != null)
            {
                this.cookingRecipes.Add(cookingRecipe);
                return;
            }

            var alchemyRecipe = (AlchemyRecipe)recipe;
            if (alchemyRecipe != null)
            {
                this.alchemyRecipes.Add(alchemyRecipe);
                return;
            }
        }
        #endregion

        #region Player Data Logic
        public void ResetPlayerData()
        {
            /* this.vitality = 1;
             this.endurance = 1;
             this.dexterity = 1;
             this.strength = 1;
             this.intelligence = 1;

             this.currentGold = 0;

             this.currentObjective = "";

             this.currentReputation = 1;

             this.ownedItems.Clear();
             this.favoriteItems.Clear();

             this.equippedHelmet = null;
             this.equippedArmor = defaultArmor;
             this.equippedGauntlets = null;
             this.equippedLegwear = defaultLegwear;
             this.equippedAccessories = new();
             this.equippedShield = null;
             this.equippedWeapon = null;

             this.appliedConsumables.Clear();
             this.appliedStatus.Clear();

             this.alchemyRecipes.Clear();
             this.cookingRecipes.Clear();

             this.companions.Clear();

             // Reset all switches
             foreach (var _switch in SwitchManager.instance.switchEntryInstances)
             {
                 _switch.currentValue = false;
             }

             // Reset all variables
             foreach (var _variable in VariableManager.instance.variableEntryInstances)
             {
                 _variable.currentValue = _variable.initialValue;
             }*/
        }
        #endregion

        #region Load Scene Logic
        public void LoadScene(int sceneIndex, bool deactivateLoadingScreenWhenFinished)
        {
            FindObjectOfType<UIDocumentLoadingScreen>(true).gameObject.SetActive(true);
            FindObjectOfType<UIDocumentLoadingScreen>(true).UpdateLoadingBar(0f);

            StartCoroutine(HandleLoadScene(sceneIndex, deactivateLoadingScreenWhenFinished, null));
        }

        public IEnumerator HandleLoadScene(int sceneIndex, bool deactivateLoadingScreenWhenFinished, object gameData)
        {
            var loadingScreen = FindAnyObjectByType<UIDocumentLoadingScreen>(FindObjectsInactive.Include);

            loadingScreen.gameObject.SetActive(true);
            loadingScreen.UpdateLoadingBar(0f);

            // Async load scene first
            var loadingSceneAsync = SceneManager.LoadSceneAsync(sceneIndex);
            loadingSceneAsync.allowSceneActivation = true;

            while (loadingSceneAsync.isDone == false)
            {
                float progress = Mathf.Clamp01(loadingSceneAsync.progress / 0.9f) * 100;

                // activate loading screen between scenes
                if (loadingScreen == null)
                {
                    loadingScreen = FindAnyObjectByType<UIDocumentLoadingScreen>(FindObjectsInactive.Include);
                    loadingScreen.gameObject.SetActive(true);
                }

                loadingScreen.UpdateLoadingBar(progress);

                yield return null;
            }


            yield return null;

            if (deactivateLoadingScreenWhenFinished)
            {
                yield return HideLoadingScreen();
            }

            yield return null;

            if (gameData != null)
            {
                //SaveSystem.instance.FinishLoad(gameData);
            }
        }

        public IEnumerator HideLoadingScreen()
        {
            var loadingScreen = FindObjectOfType<UIDocumentLoadingScreen>(true);
            yield return loadingScreen.FadeAndDisable();
        }
        #endregion

        #region AI Formulas

        public int CalculateAIHealth(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 4) * Mathf.Pow(1.025f, currentLevel));
        }

        public int CalculateAIAttack(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 2) * Mathf.Pow(1.15f, currentLevel));
        }

        public int CalculateAIPosture(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 4) * Mathf.Pow(1.05f, currentLevel));
        }

        public int CalculateAIGenericValue(int baseValue, int currentLevel)
        {
            return baseValue + Mathf.RoundToInt((baseValue / 2) * Mathf.Pow(1.05f, currentLevel));
        }

        public int CalculateSpellValue(int baseValue, int currentIntelligence)
        {
            if (currentIntelligence <= 1)
            {
                currentIntelligence = 0;
            }

            return baseValue + Mathf.RoundToInt((baseValue / 4) * Mathf.Pow(1.04f, currentIntelligence));

        }
        #endregion

        public int CalculateIncomingElementalAttack(int damageToReceive, WeaponElementType weaponElementType, DefenseStatManager defenseStatManager)
        {
            // Apply elemental defense reduction based on weaponElementType
            float elementalDefense = 0f;
            switch (weaponElementType)
            {
                case WeaponElementType.Fire:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetFireDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
                case WeaponElementType.Frost:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetFrostDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
                case WeaponElementType.Lightning:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetLightningDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
                case WeaponElementType.Magic:
                    elementalDefense = Mathf.Clamp(defenseStatManager.GetMagicDefense() / 100, 0f, 1f); // Convert to percentage and cap at 100%
                    break;
            }

            // Calculate the final damage to receive, considering elemental defense
            if (elementalDefense > 0)
            {
                damageToReceive = (int)(damageToReceive * (1 - elementalDefense)); // Subtract elemental defense as a percentage
            }

            return damageToReceive;
        }
    }

}
