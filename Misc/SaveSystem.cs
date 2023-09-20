using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using static AF.Player;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Input = UnityEngine.Input;

namespace AF
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem instance;

        public AudioClip saveSfx;
        public AudioClip saveErrorSfx;

        public string QUICK_SAVE_FILE_NAME = "quicksave";
        public string SAVE_FILE_NAME = "save_game_";

        public int MAX_SAVE_FILES_ALLOWED = 15;

        public DateTime sessionStartDateTime;

        double totalPlayTimeInSeconds;

        public bool loadingFromGameOver = false;

        ScreenshotCaptureManager screenshotCaptureManager;

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

            sessionStartDateTime = DateTime.Now;
        }

        #region Screenshot
        public Texture2D GetSaveFileScreenshot()
        {
            if (screenshotCaptureManager == null)
            {
                screenshotCaptureManager = FindObjectOfType<ScreenshotCaptureManager>(true);
            }

            return screenshotCaptureManager.CaptureScreenshot();
        }
        #endregion

        #region System

        public void Save<T>(T data, string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
            string screenshotPath = Path.Combine(Application.persistentDataPath, fileName + ".jpg");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (File.Exists(screenshotPath))
            {
                File.Delete(screenshotPath);
            }

            Texture2D screenshot = GetSaveFileScreenshot();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName + ".jpg"), screenshot.EncodeToJPG());


            string jsonDataString = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, jsonDataString);
        }

        public T Load<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return default;
            }

            string loadedJsonString = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(loadedJsonString);
        }
        #endregion

        #region #Save
        public void SaveGameData(string saveGameName)
        {
            // Check if any event is running first
            EventPage[] eventPages = FindObjectsOfType<EventPage>(); // Only active ones
            var runningEventPage = eventPages.FirstOrDefault(eventPage => eventPage.isRunning);
            if (runningEventPage)
            {
                return;
            }

            if (FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include).isColliseum)
            {
                return;
            }

            GameData gameData = new()
            {
                // Save file stuff
                isQuickSave = saveGameName.ToLower().Contains("quicksave"),
                saveFileCreationDate = DateTime.Now.ToString(),
                gameTime = totalPlayTimeInSeconds + (DateTime.Now - sessionStartDateTime).TotalSeconds,

                // World
                timeOfDay = Player.instance.timeOfDay,
                daysPassed = Player.instance.daysPassed,

                currentObjective = Player.instance.currentObjective
            };

            // Scene Name
            Scene scene = SceneManager.GetActiveScene();
            gameData.currentSceneName = scene.name;
            gameData.currentSceneIndex = scene.buildIndex;

            // Player
            GameObject player = GameObject.FindWithTag("Player");

            PlayerData playerData = new()
            {
                position = player.transform.position,
                rotation = player.transform.rotation,

                currentHealth = Player.instance.currentHealth,
                currentStamina = Player.instance.currentStamina,
                currentGold = Player.instance.currentGold,
                currentReputation = Player.instance.currentReputation,

                vitality = Player.instance.vitality,
                endurance = Player.instance.endurance,
                strength = Player.instance.strength,
                dexterity = Player.instance.dexterity,
                intelligence = Player.instance.intelligence,

          }; 

            gameData.playerData = playerData;

            // Player Equipment
            EquipmentGraphicsHandler equipmentManager = FindObjectOfType<EquipmentGraphicsHandler>();
            PlayerEquipmentData playerEquipmentData = new();

            if (Player.instance.equippedWeapon != null)
            {
                playerEquipmentData.weaponName = Player.instance.equippedWeapon.name.GetEnglishText() + (Player.instance.equippedWeapon.level > 1 ? " +" + Player.instance.equippedWeapon.level : "");
            }

            if (Player.instance.equippedShield != null)
            {
                playerEquipmentData.shieldName = Player.instance.equippedShield.name.GetEnglishText();
            }

            if (Player.instance.equippedHelmet != null)
            {
                playerEquipmentData.helmetName = Player.instance.equippedHelmet.name.GetEnglishText();
            }

            if (Player.instance.equippedArmor != null)
            {
                playerEquipmentData.chestName = Player.instance.equippedArmor.name.GetEnglishText();
            }

            if (Player.instance.equippedLegwear != null)
            {
                playerEquipmentData.legwearName = Player.instance.equippedLegwear.name.GetEnglishText();
            }

            if (Player.instance.equippedGauntlets != null)
            {
                playerEquipmentData.gauntletsName = Player.instance.equippedGauntlets.name.GetEnglishText();
            }

            if (Player.instance.equippedAccessories.Count > 0)
            {
                List<string> accessoryNames = new();
                foreach (var equippedAcc in Player.instance.equippedAccessories)
                {
                    accessoryNames.Add(equippedAcc.name.GetEnglishText());
                }

                playerEquipmentData.acessoryNames = accessoryNames.ToArray();
            }

            gameData.playerEquipmentData = playerEquipmentData;

            // Player Inventory
            List<SerializableItem> serializableItems = new List<SerializableItem>();
            foreach (Player.ItemEntry itemEntry in Player.instance.ownedItems)
            {
                int amountToSave = itemEntry.amount;

                SerializableItem serializableItem = new SerializableItem
                {
                    itemName = GetSerializableItemName(itemEntry),
                    itemCount = amountToSave,
                    itemUsage = itemEntry.usages
                };
                serializableItems.Add(serializableItem);
            }
            gameData.items = serializableItems.ToArray();

            // Player Inventory - Favorite Items
            gameData.favoriteItems = Player.instance.favoriteItems.Select(i => i.name.GetEnglishText()).ToArray();

            // Switches
            List<SerializableSwitch> switches = new();
            foreach (var _switch in SwitchManager.instance.switchEntryInstances)
            {
                SerializableSwitch serializableSwitch = new SerializableSwitch
                {
                    uuid = _switch.switchEntry.name,
                    switchName = _switch.switchEntry.name,
                    value = _switch.currentValue
                };
                switches.Add(serializableSwitch);
            }
            gameData.switches = switches.ToArray();

            // Variables
            List<SerializableVariable> variables = new();
            foreach (var _variable in VariableManager.instance.variableEntryInstances)
            {
                SerializableVariable serializableVariable = new SerializableVariable
                {
                    uuid = _variable.variableEntry.name,
                    variableName = _variable.variableEntry.name,
                    value = _variable.variableEntry.value
                };
                variables.Add(serializableVariable);
            }
            gameData.variables = variables.ToArray();

            // Active Consumables
            List<SerializableConsumable> activeConsumables = new();
            foreach (var consumable in Player.instance.appliedConsumables)
            {
                SerializableConsumable serializableConsumable = new()
                {
                    consumableItemName = consumable.consumableItemName,
                    displayName = consumable.consumableEffect.consumablePropertyName.ToString(),
                    barColor = consumable.consumableEffect.barColor,
                    value = consumable.consumableEffect.value,
                    effectDuration = consumable.consumableEffect.effectDuration,
                    currentDuration = consumable.currentDuration,
                    spriteFileName = consumable.consumableEffectSprite.name
                };

                activeConsumables.Add(serializableConsumable);
            }
            gameData.consumables = activeConsumables.ToArray();

            // Active Status Effects
            List<SerializableStatusEffect> activeStatusEffects = new();
            foreach (var status in Player.instance.appliedStatus)
            {
                SerializableStatusEffect serializableStatusEffect = new()
                {
                    statusEffectName = status.statusEffect.name.GetEnglishText(),
                    currentAmount = status.currentAmount,
                    hasReachedTotalAmount = status.hasReachedTotalAmount
                };

                activeStatusEffects.Add(serializableStatusEffect);
            }
            gameData.statusEffects = activeStatusEffects.ToArray();

            List<string> alchemyRecipesToSave = new();
            foreach (var alchemyRecipe in Player.instance.alchemyRecipes)
            {
                alchemyRecipesToSave.Add(alchemyRecipe.name.GetEnglishText());
            }
            gameData.alchemyRecipes = alchemyRecipesToSave.ToArray();

            List<string> cookingRecipesToSave = new();
            foreach (var recipe in Player.instance.cookingRecipes)
            {
                cookingRecipesToSave.Add(recipe.name.GetEnglishText());
            }
            gameData.cookingRecipes = cookingRecipesToSave.ToArray();

            // Lost coins
            LostCoins lostCoins = new();
            var activateLostCoinsPickupInstance = FindObjectOfType<ActivateLostCoinsPickup>(true);

            gameData.lostCoins = LostCoinsManager.instance.lostCoins;

            // Save companions
            gameData.companions = Player.instance.companions.ToArray();

            // Save factions
            List<SerializableFaction> factionsToSave = new();
            foreach (var factionEntry in FactionManager.instance.factionEntriesDictionary)
            {
                factionsToSave.Add(new(){  factionName = factionEntry.Key, playerReputationWithinFaction = factionEntry.Value.currentPlayerAffinityWithFaction });
            }
            gameData.factions = factionsToSave.ToArray();


            // Save shops
            List<SerializableShops> shopsToSave = new();
            foreach (var shopEntry in ShopManager.instance.characterShopInstances)
            {
                SerializableShopItem[] itemsBoughtFromPlayer = shopEntry.boughtItemsFromPlayer.Select(item => {
                    return new SerializableShopItem() {
                        itemName = item.item.name.GetEnglishText(),
                        itemCount = item.quantity,
                        priceModifier = item.priceModifier,
                        isRestockable = item.isRestockable,
                    };
                }).ToArray();

                SerializableShopItem[] boughtItemsByPlayerThatDoNotRestock = shopEntry.boughtItemsByPlayerThatDoNotRestock.Select(item => {
                    return new SerializableShopItem() {
                        itemName = item.item.name.GetEnglishText(),
                        itemCount = item.quantity,
                        priceModifier = item.priceModifier,
                        isRestockable = item.isRestockable,
                    };
                }).ToArray();

                SerializableShopItem[] stockItems = shopEntry.itemStock.Select(item => {
                    return new SerializableShopItem() {
                        itemName = item.item.name.GetEnglishText(),
                        itemCount = item.quantity,
                        priceModifier = item.priceModifier,
                        isRestockable = item.isRestockable,
                    };
                }).ToArray();

                shopsToSave.Add(new()
                {
                    shopName = shopEntry.name,
                    dayThatTradingBegan = shopEntry.dayThatTradingBegan,
                    itemsBoughtFromPlayer = itemsBoughtFromPlayer,
                    boughtItemsByPlayerThatDoNotRestock = boughtItemsByPlayerThatDoNotRestock,
                    stockItems = stockItems,
                });
            }
            gameData.shops = shopsToSave.ToArray();

            // Bonfires
            gameData.unlockedBonfires = Player.instance.unlockedBonfires.ToArray();

            Save(gameData, saveGameName);


            var notificationManager = FindObjectOfType<NotificationManager>(true);
            notificationManager.ShowNotification(LocalizedTerms.ProgressSaved(), notificationManager.systemSuccess);
        }
        #endregion

        string GetSerializableItemName(ItemEntry itemEntry)
        {
            Weapon wp = itemEntry.item as Weapon;
            if (wp != null)
            {
                return wp.name.GetEnglishText() + (wp.level > 1 ? " +" + wp.level : "");
            }

            return itemEntry.item.name.GetEnglishText();

        }

        #region Load
        public void LoadGameData(string filePath)
        {
            GameData gameData = GetGameData(filePath);

            StartCoroutine(CallLoad(gameData));
        }

        public void LoadLastSavedGame()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");


            string targetFile = "";
            System.DateTime targetFileDateTime = new System.DateTime();
            foreach (var file in files)
            {
                var thisFileWriteTime = File.GetLastWriteTime(file);
                
                if (targetFileDateTime.CompareTo(thisFileWriteTime) < 0)
                {
                    targetFile = file;
                    targetFileDateTime = thisFileWriteTime;
                }
            }

            if (string.IsNullOrEmpty(targetFile) == false)
            {
                LoadGameData(targetFile);
            }

        }

        // TODO Remove for builds
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (screenshotCaptureManager == null)
                {
                    screenshotCaptureManager = FindObjectOfType<ScreenshotCaptureManager>(true);

                    if (screenshotCaptureManager != null)
                    {
                        screenshotCaptureManager.CaptureScreenshot();
                    }
                }

                SaveSystem.instance.SaveGameData(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                SaveSystem.instance.LoadLastSavedGame();
            }
        }

        IEnumerator CallLoad(GameData gameData)
        {
            // If loading game, make sure we never show the title screen again
            if (Player.instance.hasShownTitleScreen == false)
            {
                Player.instance.hasShownTitleScreen = true;
            }

            yield return Player.instance.HandleLoadScene(gameData.currentSceneIndex, false);

            yield return null;

            GameObject player = FindAnyObjectByType<PlayerCombatController>(FindObjectsInactive.Include).gameObject;
            player.gameObject.SetActive(true);

            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<PlayerComponentManager>().UpdatePosition(gameData.playerData.position, gameData.playerData.rotation);
            player.GetComponent<CharacterController>().enabled = true;
            yield return null;

            if (loadingFromGameOver == false)
            {
                LostCoinsManager.instance.lostCoins = gameData.lostCoins;
            }

            // Update Switch Manager first
            SwitchManager.instance.OnGameLoaded(gameData);

            var saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();

            foreach (ISaveable saveable in saveables)
            {
                saveable.OnGameLoaded(gameData);
                yield return null;
            }

            totalPlayTimeInSeconds = gameData.gameTime;
            sessionStartDateTime = DateTime.Now;

            yield return new WaitForSeconds(0.1f);

            yield return Player.instance.HideLoadingScreen();

            loadingFromGameOver = false;
        }
        #endregion

        #region Delete
        public void DeleteSaveFile(string saveFilePath, string screenshotFilePath)
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }

            if (File.Exists(screenshotFilePath))
            {
                File.Delete(screenshotFilePath);
            }
        }
        #endregion

        #region Public Methods (Get saves, can save files, etc.)
        public List<SaveFileEntry> GetSaveFiles()
        {
            List<SaveFileEntry> saveList = new List<SaveFileEntry>();

            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
            string[] screenshots = Directory.GetFiles(Application.persistentDataPath, "*.jpg");

            for (int i = 0; i < files.Length; i++)
            {
                var gameData = GetGameData(files[i]);

                SaveFileEntry saveFileEntry = new SaveFileEntry
                {
                    fileFullPath = files[i],
                    creationDate = gameData.saveFileCreationDate,
                    gameTime = gameData.gameTime,
                    level = gameData.playerData.vitality + gameData.playerData.endurance + gameData.playerData.strength + gameData.playerData.dexterity + gameData.playerData.intelligence,
                    sceneName = gameData.currentSceneName
                };
                var targetTexture = new Texture2D(2, 2);
                try
                {
                    if (screenshots.Length > 0 && screenshots[i] != null && string.IsNullOrEmpty(screenshots[i]) == false)
                    {
                        targetTexture.LoadImage(File.ReadAllBytes(screenshots[i]));
                        saveFileEntry.screenshot = targetTexture;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    if (screenshotCaptureManager == null)
                    {
                        screenshotCaptureManager = FindObjectOfType<ScreenshotCaptureManager>(true);
                    }

                    saveFileEntry.screenshot = screenshotCaptureManager.fallbackSaveScreenshot;
                }

                saveFileEntry.currentObjective = "";
                var currentObjective = gameData.currentObjective;
                if (currentObjective != null)
                {
                    saveFileEntry.currentObjective = currentObjective;
                }

                saveList.Add(saveFileEntry);
            }

            return saveList;
        }

        public GameData GetGameData(string saveFilePath)
        {
            return Load<GameData>(saveFilePath);
        }

        public bool HasSaveGame(string saveFileName)
        {
            string path = Path.Combine(Application.persistentDataPath, saveFileName + ".json");

            return File.Exists(path);
        }

        public string GetTotalPlayTime()
        {
            var secondsPlayed = totalPlayTimeInSeconds + (DateTime.Now - sessionStartDateTime).TotalSeconds;
            var date = TimeSpan.FromSeconds(secondsPlayed);

            return "" + date.Hours + "h:" + date.Minutes + "m:" + date.Seconds + "s";
        }

        #endregion
    }

    [System.Serializable]
    public class SaveFileEntry
    {
        public string fileFullPath;
        public string sceneName;
        public int level;
        public string creationDate;
        public double gameTime;
        public Texture2D screenshot;
        public string currentObjective;
    }
    
    [System.Serializable]
    public class LostCoins
    {
        public Vector3 position;

        public int amount;

        public int sceneIndex;
    }

    [System.Serializable]
    public class SerializedCompanion {
        public string companionId;

        public bool isWaitingForPlayer;

        public int waitingForPlayerSceneIndex;

        public Vector3 waitingForPlayerPosition;
    }

    [System.Serializable]
    public class SerializableFaction
    {
        public string factionName;
        public int playerReputationWithinFaction;
    }

    [System.Serializable]
    public class SerializableShops
    {
        public string shopName;
        public int dayThatTradingBegan;
        public SerializableShopItem[] stockItems;
        public SerializableShopItem[] itemsBoughtFromPlayer;
        public SerializableShopItem[] boughtItemsByPlayerThatDoNotRestock;
    }

    [System.Serializable]
    public class GameData
    {
        public bool isQuickSave;

        public double gameTime;

        public string saveFileCreationDate;

        public string currentSceneName;

        public int currentSceneIndex;

        // Day Night System
        public float timeOfDay;
        public int daysPassed;

        public PlayerData playerData;

        public PlayerEquipmentData playerEquipmentData;

        public SerializableSwitch[] switches;

        public SerializableVariable[] variables;

        public SerializableItem[] items;

        public string[] favoriteItems;

        public SerializableConsumable[] consumables;

        public SerializableStatusEffect[] statusEffects;

        public string[] alchemyRecipes;

        public string[] cookingRecipes;

        public LostCoins lostCoins;

        public string currentObjective;

        public SerializedCompanion[] companions;

        public SerializableFaction[] factions;

        public SerializableShops[] shops;

        public string[] unlockedBonfires;
    }

    [System.Serializable]
    public class PlayerData
    {
        public Vector3 position;
        public Quaternion rotation;

        public float currentHealth;
        public float currentStamina;

        public int currentReputation;

        public int currentGold;

        public int vitality;
        public int endurance;
        public int strength;
        public int dexterity;
        public int intelligence;

    }

    [System.Serializable]
    public class PlayerEquipmentData
    {
        public string weaponName;
        public string shieldName;
        public string helmetName;
        public string chestName;
        public string gauntletsName;
        public string legwearName;
        public string[] acessoryNames;
    }

    [System.Serializable]
    public class SerializableSwitch
    {
        public string uuid;
        public string switchName;
        public bool value;
    }

    [System.Serializable]
    public class SerializableVariable
    {
        public string uuid;
        public string variableName;
        public int value;
    }

    [System.Serializable]
    public class SerializableItem
    {
        public string itemName;
        public int itemCount;
        public int itemUsage;
    }

    [System.Serializable]
    public class SerializableShopItem
    {
        public string itemName;
        public int itemCount;
        public int priceModifier;
        public bool isRestockable;
    }

    [System.Serializable]
    public class SerializableConsumable
    {
        public string consumableItemName;

        public string displayName;

        public Color barColor;

        public float value;

        public float effectDuration;

        public string spriteFileName;

        public float currentDuration;
    }

    [System.Serializable]
    public class SerializableStatusEffect
    {
        public string statusEffectName;
        public float currentAmount;
        public bool hasReachedTotalAmount;
    }
}
