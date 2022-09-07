using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{

    [System.Serializable]
    public class GameData
    {
        public string currentSceneName;
        public int currentSceneIndex;

        public PlayerData playerData;

        public PlayerEquipmentData playerEquipmentData;

        public SerializableSwitch[] switches;

        public SerializableLocalSwitch[] localSwitches;

        public SerializableItem[] items;
        public string[] favoriteItems;

        public string lastActiveCameraName;

        public SerializableConsumable[] consumables;
        public SerializableStatusEffect[] statusEffects;
    }

    public class SaveSystem : MonoBehaviour
    {

        public delegate void OnGameLoadEvent(GameData gameData);
        // the event itself
        public event OnGameLoadEvent OnGameLoad;

        public static SaveSystem instance;

        public AudioClip saveSfx;

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

        public void Save<T>(T data, string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName + ".json");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string jsonDataString = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, jsonDataString);
        }

        public T Load<T>(string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName + ".json");

            if (!File.Exists(path))
            {
                return default(T);
            }

            string loadedJsonString = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(loadedJsonString);
        }

        public void SaveGameData()
        {
            GameData gameData = new GameData();

            // Scene Name
            Scene scene = SceneManager.GetActiveScene();
            gameData.currentSceneName = scene.name;
            gameData.currentSceneIndex = scene.buildIndex;

            // Player
            Player player = FindObjectOfType<Player>();

            PlayerData playerData = new PlayerData();
            playerData.position = player.transform.position;
            playerData.rotation = player.transform.rotation;

            playerData.currentHealth = PlayerStatsManager.instance.GetCurrentHealth();
            playerData.currentMagic = PlayerStatsManager.instance.currentMagic;
            playerData.currentStamina = PlayerStatsManager.instance.currentStamina;
            playerData.currentExperience = PlayerStatsManager.instance.currentExperience;
            playerData.currentReputation = PlayerStatsManager.instance.currentReputation;

            playerData.vitality = PlayerStatsManager.instance.vitality;
            playerData.endurance = PlayerStatsManager.instance.endurance;
            playerData.intelligence = PlayerStatsManager.instance.intelligence;
            playerData.strength = PlayerStatsManager.instance.strength;
            playerData.dexterity = PlayerStatsManager.instance.dexterity;
            playerData.arcane = PlayerStatsManager.instance.arcane;
            playerData.occult = PlayerStatsManager.instance.occult;
            playerData.charisma = PlayerStatsManager.instance.charisma;
            playerData.luck = PlayerStatsManager.instance.luck;

            gameData.playerData = playerData;

            // Player Equipment
            EquipmentGraphicsHandler equipmentManager = FindObjectOfType<EquipmentGraphicsHandler>();
            PlayerEquipmentData playerEquipmentData = new PlayerEquipmentData();

            if (PlayerInventoryManager.instance.currentWeapon != null)
            {
                playerEquipmentData.weaponName = PlayerInventoryManager.instance.currentWeapon.name;
            }

            if (PlayerInventoryManager.instance.currentShield != null)
            {
                playerEquipmentData.shieldName = PlayerInventoryManager.instance.currentShield.name;
            }

            if (PlayerInventoryManager.instance.currentHelmet != null)
            {
                playerEquipmentData.helmetName = PlayerInventoryManager.instance.currentHelmet.name;
            }

            if (PlayerInventoryManager.instance.currentChest != null)
            {
                playerEquipmentData.chestName = PlayerInventoryManager.instance.currentChest.name;
            }

            if (PlayerInventoryManager.instance.currentLegwear != null)
            {
                playerEquipmentData.legwearName = PlayerInventoryManager.instance.currentLegwear.name;
            }

            if (PlayerInventoryManager.instance.currentGauntlets != null)
            {
                playerEquipmentData.gauntletsName = PlayerInventoryManager.instance.currentGauntlets.name;
            }

            if (PlayerInventoryManager.instance.currentAccessory1 != null)
            {
                playerEquipmentData.accessory1Name = PlayerInventoryManager.instance.currentAccessory1.name;
            }

            if (PlayerInventoryManager.instance.currentAccessory2 != null)
            {
                playerEquipmentData.accessory2Name = PlayerInventoryManager.instance.currentAccessory2.name;
            }

            gameData.playerEquipmentData = playerEquipmentData;

            // Player Inventory
            List<SerializableItem> serializableItems = new List<SerializableItem>();
            foreach (ItemEntry itemEntry in PlayerInventoryManager.instance.currentItems)
            {
                SerializableItem serializableItem = new SerializableItem();
                serializableItem.itemName = itemEntry.item.name;
                serializableItem.itemCount = itemEntry.amount;
                serializableItems.Add(serializableItem);
            }
            gameData.items = serializableItems.ToArray();

            // Player Inventory - Favorite Items
            gameData.favoriteItems = PlayerInventoryManager.instance.currentFavoriteItems.Select(i => i.name).ToArray();

            // Switches
            List<SerializableSwitch> switches = new List<SerializableSwitch>();
            foreach (Switch s in SwitchManager.instance.switches)
            {
                SerializableSwitch serializableSwitch = new SerializableSwitch();

                serializableSwitch.switchName = s.switchName.ToString();
                serializableSwitch.value = s.value;

                switches.Add(serializableSwitch);
            }
            gameData.switches = switches.ToArray();

            // Local Switches
            GameData savedGameData = Load<GameData>("gameData");
            List<SerializableLocalSwitch> alreadySavedLocalSwitches = (savedGameData != null && savedGameData.localSwitches != null) ? savedGameData.localSwitches.ToList() : new List<SerializableLocalSwitch>();
            LocalSwitch[] localSwitchesInScene = FindObjectsOfType<LocalSwitch>(true);

            foreach (LocalSwitch localSwitchInScene in localSwitchesInScene)
            {
                int idx = alreadySavedLocalSwitches.FindIndex(savedLocalSwitch => savedLocalSwitch.uuid == localSwitchInScene.ID);
                if (idx != -1)
                {
                    alreadySavedLocalSwitches[idx].localSwitchName = localSwitchInScene.localSwitchName.ToString();
                }
                else
                {
                    SerializableLocalSwitch serializableLocalSwitch = new SerializableLocalSwitch();
                    serializableLocalSwitch.uuid = localSwitchInScene.ID;
                    serializableLocalSwitch.localSwitchName = localSwitchInScene.localSwitchName.ToString();
                    alreadySavedLocalSwitches.Add(serializableLocalSwitch);
                }
            }
            gameData.localSwitches = alreadySavedLocalSwitches.ToArray();

            // Activate game camera
            var sceneCameras = FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>(true);

            Cinemachine.CinemachineVirtualCamera activeCamera = sceneCameras.Count() > 0 ? sceneCameras.First(camera => camera.gameObject.activeSelf) : null;
            if (activeCamera != null)
            {
                gameData.lastActiveCameraName = activeCamera.gameObject.name;
            }
            else
            {
                gameData.lastActiveCameraName = "Player Camera";
            }

            // Active Consumables
            List<SerializableConsumable> activeConsumables = new List<SerializableConsumable>();
            foreach (var consumable in PlayerStatsManager.instance.appliedConsumables)
            {
                SerializableConsumable serializableConsumable = new SerializableConsumable();

                serializableConsumable.consumableName = consumable.consumable.name;
                serializableConsumable.currentDuration = consumable.currentDuration;

                activeConsumables.Add(serializableConsumable);
            }
            gameData.consumables = activeConsumables.ToArray();

            // Active Status Effects
            List<SerializableStatusEffect> activeStatusEffects = new List<SerializableStatusEffect>();
            foreach (var status in PlayerStatsManager.instance.appliedStatus)
            {
                SerializableStatusEffect serializableStatusEffect = new SerializableStatusEffect();

                serializableStatusEffect.statusEffectName = status.statusEffect.name;
                serializableStatusEffect.currentAmount = status.currentAmount;
                serializableStatusEffect.hasReachedTotalAmount = status.hasReachedTotalAmount;

                activeStatusEffects.Add(serializableStatusEffect);
            }
            gameData.statusEffects = activeStatusEffects.ToArray();

            Save(gameData, "gameData");

            // MenuManager.instance.CloseAll();
        }

        public void LoadGameData()
        {
            GameData gameData = Load<GameData>("gameData");

            // Load scene first
            SceneManager.LoadScene(gameData.currentSceneIndex);

            StartCoroutine(CallLoad(gameData));
        }

        IEnumerator CallLoad(GameData gameData)
        {
            yield return null;

            var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

            foreach (ISaveable saveable in saveables)
            {
                saveable.OnGameLoaded(gameData);
            }

            // Activate game camera
            var sceneCameras = FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>(true);

            if (sceneCameras.Length > 0)
            {
                var targetCamera = sceneCameras.First(camera => camera.gameObject.name == gameData.lastActiveCameraName);

                if (targetCamera == null)
                {
                    targetCamera = sceneCameras.First(camera => camera.gameObject.name == "Player Camera");
                }

                targetCamera.gameObject.SetActive(true);
                foreach (var sceneCam in sceneCameras)
                {
                    if (sceneCam != targetCamera)
                    {
                        sceneCam.gameObject.SetActive(false);
                    }
                }
            }

        }
    }


    [System.Serializable]
    public class PlayerData
    {
        public Vector3 position;
        public Quaternion rotation;

        public float currentExperience;

        public float currentHealth;
        public float currentMagic;
        public float currentStamina;

        public int currentReputation;

        public int gold;

        public int vitality;
        public int intelligence;
        public int endurance;
        public int strength;
        public int dexterity;
        public int arcane;
        public int occult;
        public int charisma;
        public int luck;
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
        public string switchName;
        public bool value;
    }

    [System.Serializable]
    public class SerializableLocalSwitch
    {
        public string uuid;
        public string localSwitchName;
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