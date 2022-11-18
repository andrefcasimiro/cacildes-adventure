using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{

    public class SaveSystem : MonoBehaviour
    {

/*        public delegate void OnGameLoadEvent(GameData gameData);
        // the event itself
        public event OnGameLoadEvent OnGameLoad;*/

        public static SaveSystem instance;

        public AudioClip saveSfx;
        public AudioClip saveErrorSfx;

        public string QUICK_SAVE_FILE_NAME = "quicksave";
        public string SAVE_FILE_NAME = "save_game_";

        public int MAX_SAVE_FILES_ALLOWED = 15;

        public System.DateTime sessionStartDateTime;

        double totalPlayTimeInSeconds;

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

            sessionStartDateTime = System.DateTime.Now;
        }

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

            var menuManager = FindObjectOfType<MenuManager>(true);

            if (menuManager != null)
            {
                if (menuManager.screenshotBeforeOpeningMenu == null)
                {
                    menuManager.CaptureScreenshot();
                }

                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName + ".jpg"), menuManager.screenshotBeforeOpeningMenu.EncodeToJPG());
            }


            string jsonDataString = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, jsonDataString);
        }

        public T Load<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return default(T);
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
            var runningEventPage = eventPages.FirstOrDefault(eventPage => eventPage.IsRunning());
            if (runningEventPage)
            {
                return;
            }

            GameData gameData = new GameData();

            // Save file stuff
            gameData.isQuickSave = saveGameName.ToLower().Contains("quicksave");
            gameData.saveFileCreationDate = System.DateTime.Now.ToString();
            gameData.gameTime = totalPlayTimeInSeconds + (System.DateTime.Now - sessionStartDateTime).TotalSeconds;

            // World
            gameData.timeOfDay = Player.instance.timeOfDay;
            gameData.daysPassed = Player.instance.daysPassed;

            // Scene Name
            Scene scene = SceneManager.GetActiveScene();
            gameData.currentSceneName = scene.name;
            gameData.currentSceneIndex = scene.buildIndex;

            // Player
            GameObject player = GameObject.FindWithTag("Player");

            PlayerData playerData = new PlayerData();

            playerData.position = player.transform.position;
            playerData.rotation = player.transform.rotation;

            playerData.currentHealth = Player.instance.currentHealth;
            playerData.currentStamina = Player.instance.currentStamina;
            playerData.currentGold = Player.instance.currentGold;
            playerData.currentReputation = Player.instance.currentReputation;

            playerData.vitality = Player.instance.vitality;
            playerData.endurance = Player.instance.endurance;
            playerData.strength = Player.instance.strength;
            playerData.dexterity = Player.instance.dexterity;

            gameData.playerData = playerData;

            // Player Equipment
            EquipmentGraphicsHandler equipmentManager = FindObjectOfType<EquipmentGraphicsHandler>();
            PlayerEquipmentData playerEquipmentData = new PlayerEquipmentData();

            if (Player.instance.equippedWeapon != null)
            {
                playerEquipmentData.weaponName = Player.instance.equippedWeapon.name;
            }

            if (Player.instance.equippedShield != null)
            {
                playerEquipmentData.shieldName = Player.instance.equippedShield.name;
            }

            if (Player.instance.equippedHelmet != null)
            {
                playerEquipmentData.helmetName = Player.instance.equippedHelmet.name;
            }

            if (Player.instance.equippedArmor != null)
            {
                playerEquipmentData.chestName = Player.instance.equippedArmor.name;
            }

            if (Player.instance.equippedLegwear != null)
            {
                playerEquipmentData.legwearName = Player.instance.equippedLegwear.name;
            }

            if (Player.instance.equippedGauntlets != null)
            {
                playerEquipmentData.gauntletsName = Player.instance.equippedGauntlets.name;
            }

            if (Player.instance.equippedAccessory != null)
            {
                playerEquipmentData.accessory1Name = Player.instance.equippedAccessory.name;
            }

            gameData.playerEquipmentData = playerEquipmentData;

            // Player Inventory
            List<SerializableItem> serializableItems = new List<SerializableItem>();
            foreach (Player.ItemEntry itemEntry in Player.instance.ownedItems)
            {
                SerializableItem serializableItem = new SerializableItem();
                serializableItem.itemName = itemEntry.item.name;
                serializableItem.itemCount = itemEntry.amount;
                serializableItems.Add(serializableItem);
            }
            gameData.items = serializableItems.ToArray();

            // Player Inventory - Favorite Items
            gameData.favoriteItems = Player.instance.favoriteItems.Select(i => i.name).ToArray();

            // Switches
            List<SerializableSwitch> switches = new List<SerializableSwitch>();
            foreach (var _switch in SwitchManager.instance.switches)
            {
                SerializableSwitch serializableSwitch = new SerializableSwitch();
                serializableSwitch.uuid = _switch.ID;
                serializableSwitch.switchName = _switch.name;
                serializableSwitch.value = _switch.value;
                switches.Add(serializableSwitch);
            }
            gameData.switches = switches.ToArray();

            // Variables
            List<SerializableVariable> variables = new List<SerializableVariable>();
            foreach (var _variable in VariableManager.instance.variables)
            {
                SerializableVariable serializableVariable = new SerializableVariable();
                serializableVariable.uuid = _variable.uuid;
                serializableVariable.variableName = _variable.name;
                serializableVariable.value = _variable.value;
                variables.Add(serializableVariable);
            }
            gameData.variables = variables.ToArray();

            // Active Consumables
            List<SerializableConsumable> activeConsumables = new List<SerializableConsumable>();
            foreach (var consumable in Player.instance.appliedConsumables)
            {
                SerializableConsumable serializableConsumable = new SerializableConsumable();

                serializableConsumable.consumableName = consumable.consumableEffect.consumablePropertyName.ToString();
                serializableConsumable.displayName = consumable.consumableEffect.displayName;
                serializableConsumable.barColor = consumable.consumableEffect.barColor;
                serializableConsumable.value = consumable.consumableEffect.value;
                serializableConsumable.effectDuration = consumable.consumableEffect.effectDuration;
                serializableConsumable.currentDuration = consumable.currentDuration;
                serializableConsumable.spriteFileName = consumable.consumableEffectSprite.name;

                activeConsumables.Add(serializableConsumable);
            }
            gameData.consumables = activeConsumables.ToArray();

            // Active Status Effects
            List<SerializableStatusEffect> activeStatusEffects = new List<SerializableStatusEffect>();
            foreach (var status in Player.instance.appliedStatus)
            {
                SerializableStatusEffect serializableStatusEffect = new SerializableStatusEffect();

                serializableStatusEffect.statusEffectName = status.statusEffect.name;
                serializableStatusEffect.currentAmount = status.currentAmount;
                serializableStatusEffect.hasReachedTotalAmount = status.hasReachedTotalAmount;

                activeStatusEffects.Add(serializableStatusEffect);
            }
            gameData.statusEffects = activeStatusEffects.ToArray();

            List<string> alchemyRecipesToSave = new List<string>();
            foreach (var alchemyRecipe in Player.instance.alchemyRecipes)
            {
                alchemyRecipesToSave.Add(alchemyRecipe.name);
            }
            gameData.alchemyRecipes = alchemyRecipesToSave.ToArray();

            List<string> cookingRecipesToSave = new List<string>();
            foreach (var recipe in Player.instance.cookingRecipes)
            {
                cookingRecipesToSave.Add(recipe.name);
            }
            gameData.cookingRecipes = cookingRecipesToSave.ToArray();


            Save(gameData, saveGameName);
        }
        #endregion

        #region Load
        public void LoadGameData(string filePath)
        {
            GameData gameData = GetGameData(filePath);

            StartCoroutine(CallLoad(gameData));
        }

        public void LoadLastSavedGame()
        {
            string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.json");


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

            if (System.String.IsNullOrEmpty(targetFile) == false)
            {
                LoadGameData(targetFile);
            }

        }

        IEnumerator CallLoad(GameData gameData)
        {
            // If loading game, make sure we never show the title screen again
            if (Player.instance.hasShownTitleScreen == false)
            {
                Player.instance.hasShownTitleScreen = true;
            }

            yield return null;

            var loadingScreen = FindObjectOfType<UIDocumentLoadingScreen>(true);

            // Load scene first
            var loadingSceneAsync = SceneManager.LoadSceneAsync(gameData.currentSceneIndex);
            loadingScreen.gameObject.SetActive(true);
            loadingSceneAsync.allowSceneActivation = true;

            while (loadingSceneAsync.isDone == false)
            {
                loadingScreen.UpdateLoadingBar(loadingSceneAsync.progress * 100f);   
                yield return null;
            }

            yield return null;

            GameObject player = GameObject.FindWithTag("Player");
            player.gameObject.SetActive(true);
            yield return null;
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = gameData.playerData.position;
            player.transform.rotation = gameData.playerData.rotation;
            player.GetComponent<CharacterController>().enabled = true;
            yield return null;

            var saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();

            foreach (ISaveable saveable in saveables)
            {
                saveable.OnGameLoaded(gameData);
                yield return null;
            }

            totalPlayTimeInSeconds = gameData.gameTime;
            sessionStartDateTime = System.DateTime.Now;

            yield return new WaitForSeconds(0.1f);

            yield return loadingScreen.FadeAndDisable();

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
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

            string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.json");
            string[] screenshots = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.jpg");

            for (int i = 0; i < files.Length; i++)
            {
                var gameData = GetGameData(files[i]);

                SaveFileEntry saveFileEntry = new SaveFileEntry();
                saveFileEntry.fileFullPath = files[i];
                saveFileEntry.creationDate = gameData.saveFileCreationDate;
                saveFileEntry.gameTime = gameData.gameTime;
                saveFileEntry.level = gameData.playerData.vitality + gameData.playerData.endurance + gameData.playerData.strength + gameData.playerData.dexterity;
                saveFileEntry.sceneName = gameData.currentSceneName;
                var targetTexture = new Texture2D(2, 2);

                if (screenshots.Length > 0 && System.String.IsNullOrEmpty(screenshots[i]) == false)
                {
                    targetTexture.LoadImage(File.ReadAllBytes(screenshots[i]));
                    saveFileEntry.screenshot = targetTexture;
                }
                saveFileEntry.isQuickSave = gameData.isQuickSave;

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
        public bool isQuickSave;
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
        public string accessory1Name;
        public string accessory2Name;
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
    }

    [System.Serializable]
    public class SerializableConsumable
    {
        public string consumableName;

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