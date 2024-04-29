using UnityEngine;
using Input = UnityEngine.Input;
using CI.QuickSave;
using UnityEngine.SceneManagement;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using AF.Inventory;
using System.Linq;
using AF.Companions;
using AF.Flags;
using AF.Bonfires;
using TigerForge;
using AF.Events;
using AF.Pickups;
using System;
using System.IO;

namespace AF
{
    public class SaveManager : MonoBehaviour
    {

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public PickupDatabase pickupDatabase;
        public QuestsDatabase questsDatabase;
        public CompanionsDatabase companionsDatabase;
        public BonfiresDatabase bonfiresDatabase;
        public GameSession gameSession;
        public FlagsDatabase flagsDatabase;
        public RecipesDatabase recipesDatabase;

        [Header("Components")]
        public FadeManager fadeManager;
        public PlayerManager playerManager;
        public NotificationManager notificationManager;

        // Flags that allow us to save the game
        bool hasMomentOnGoing = false;
        bool hasBossFightOnGoing = false;

        public string SAVE_FILES_FOLDER = "QuickSave";

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MOMENT_START, () => { hasMomentOnGoing = true; });
            EventManager.StartListening(EventMessages.ON_MOMENT_END, () => { hasMomentOnGoing = false; });
            EventManager.StartListening(EventMessages.ON_BOSS_BATTLE_BEGINS, () => { hasBossFightOnGoing = true; });
            EventManager.StartListening(EventMessages.ON_BOSS_BATTLE_ENDS, () => { hasBossFightOnGoing = false; });

            SaveUtils.CheckAndMigrateOldSaveFiles(SAVE_FILES_FOLDER);
        }

        public bool CanSave()
        {
            if (hasMomentOnGoing)
            {
                return false;
            }

            if (hasBossFightOnGoing)
            {
                return false;
            }

            if (playerManager.thirdPersonController.Grounded == false)
            {
                return false;
            }

            return true;
        }

        public void ResetGameState()
        {
            playerStatsDatabase.Clear();
            equipmentDatabase.Clear();
            inventoryDatabase.SetDefaultItems();
            pickupDatabase.Clear();
            questsDatabase.Clear();
            companionsDatabase.Clear();
            bonfiresDatabase.Clear();
            flagsDatabase.Clear();
            recipesDatabase.Clear();
        }

        void SaveRecipes(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("craftingRecipes", recipesDatabase.craftingRecipes.Select(craftingRecipe => craftingRecipe.name));
        }
        void SavePlayerStats(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("currentHealth", playerStatsDatabase.currentHealth);
            quickSaveWriter.Write("currentStamina", playerStatsDatabase.currentStamina);
            quickSaveWriter.Write("currentMana", playerStatsDatabase.currentMana);
            quickSaveWriter.Write("reputation", playerStatsDatabase.reputation);
            quickSaveWriter.Write("vitality", playerStatsDatabase.vitality);
            quickSaveWriter.Write("endurance", playerStatsDatabase.endurance);
            quickSaveWriter.Write("intelligence", playerStatsDatabase.intelligence);
            quickSaveWriter.Write("strength", playerStatsDatabase.strength);
            quickSaveWriter.Write("dexterity", playerStatsDatabase.dexterity);
            quickSaveWriter.Write("gold", playerStatsDatabase.gold);
            quickSaveWriter.Write("lostGold", playerStatsDatabase.lostGold);
            quickSaveWriter.Write("sceneWhereGoldWasLost", playerStatsDatabase.sceneWhereGoldWasLost);
            quickSaveWriter.Write("positionWhereGoldWasLost", playerStatsDatabase.positionWhereGoldWasLost);
        }

        void SavePlayerEquipment(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("currentWeaponIndex", equipmentDatabase.currentWeaponIndex);
            quickSaveWriter.Write("currentShieldIndex", equipmentDatabase.currentShieldIndex);
            quickSaveWriter.Write("currentArrowIndex", equipmentDatabase.currentArrowIndex);
            quickSaveWriter.Write("currentSpellIndex", equipmentDatabase.currentSpellIndex);
            quickSaveWriter.Write("currentConsumableIndex", equipmentDatabase.currentConsumableIndex);
            quickSaveWriter.Write("weapons", equipmentDatabase.weapons.Select(weapon => weapon != null ? weapon.name + "|" + weapon.level : ""));
            quickSaveWriter.Write("shields", equipmentDatabase.shields.Select(shield => shield != null ? shield.name : ""));
            quickSaveWriter.Write("arrows", equipmentDatabase.arrows.Select(arrow => arrow != null ? arrow.name : ""));
            quickSaveWriter.Write("spells", equipmentDatabase.spells.Select(spell => spell != null ? spell.name : ""));
            quickSaveWriter.Write("accessories", equipmentDatabase.accessories.Select(accessory => accessory != null ? accessory.name : ""));
            quickSaveWriter.Write("consumables", equipmentDatabase.consumables.Select(consumable => consumable != null ? consumable.name : ""));
            quickSaveWriter.Write("helmet", equipmentDatabase.helmet != null ? equipmentDatabase.helmet.name : "");
            quickSaveWriter.Write("armor", equipmentDatabase.armor != null ? equipmentDatabase.armor.name : "");
            quickSaveWriter.Write("gauntlet", equipmentDatabase.gauntlet != null ? equipmentDatabase.gauntlet.name : "");
            quickSaveWriter.Write("legwear", equipmentDatabase.legwear != null ? equipmentDatabase.legwear.name : "");
            quickSaveWriter.Write("isTwoHanding", equipmentDatabase.isTwoHanding);
        }


        void SavePlayerInventory(QuickSaveWriter quickSaveWriter)
        {
            SerializedDictionary<string, ItemAmount> keyValuePairs = new();

            foreach (var ownedItem in inventoryDatabase.ownedItems)
            {
                string path = Utils.GetItemPath(ownedItem.Key);

                if (!keyValuePairs.ContainsKey(path))
                {
                    keyValuePairs.Add(path, ownedItem.Value);
                }
            }

            quickSaveWriter.Write("ownedItems", keyValuePairs);
        }
        void SavePickups(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("pickups", pickupDatabase.pickups);
            quickSaveWriter.Write("replenishables", pickupDatabase.replenishables);
        }

        void SaveQuests(QuickSaveWriter quickSaveWriter)
        {
            SerializedDictionary<string, int> payload = new();

            questsDatabase.questsReceived.ForEach(questReceived =>
            {
                payload.Add("Quests/" + questReceived.name,
                    questReceived.questProgress);
            });

            quickSaveWriter.Write("questsReceived", payload);
            quickSaveWriter.Write("currentTrackedQuestIndex", questsDatabase.currentTrackedQuestIndex);
        }

        void SaveFlags(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("flags", flagsDatabase.flags);
        }

        void SaveSceneSettings(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("sceneIndex", SceneManager.GetActiveScene().buildIndex);
            quickSaveWriter.Write("playerPosition", playerManager.transform.position);
            quickSaveWriter.Write("playerRotation", playerManager.transform.rotation);
        }
        void SaveGameSettings(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("timeOfDay", gameSession.timeOfDay);
            quickSaveWriter.Write("graphicsQuality", gameSession.graphicsQuality);
            quickSaveWriter.Write("mouseSensitivity", gameSession.mouseSensitivity);
            quickSaveWriter.Write("musicVolume", gameSession.musicVolume);
        }
        void SaveCompanions(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("companionsInParty", companionsDatabase.companionsInParty);
        }

        void SaveBonfires(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("unlockedBonfires", bonfiresDatabase.unlockedBonfires);
        }

        void LoadRecipes(QuickSaveReader quickSaveReader)
        {
            quickSaveReader.TryRead("craftingRecipes", out string[] craftingRecipes);

            if (craftingRecipes != null && craftingRecipes.Count() > 0)
            {
                foreach (var recipeName in craftingRecipes)
                {
                    CraftingRecipe craftingRecipe = Resources.Load<CraftingRecipe>("Recipes/" + recipeName);
                    if (craftingRecipe != null)
                    {
                        recipesDatabase.AddCraftingRecipe(craftingRecipe);
                    }
                }
            }
        }

        void LoadPlayerStats(QuickSaveReader quickSaveReader, bool isFromGameOver)
        {
            // Try to read currentHealth using TryRead
            quickSaveReader.TryRead("currentHealth", out float currentHealth);
            playerStatsDatabase.currentHealth = currentHealth;

            // Try to read other stats
            quickSaveReader.TryRead<float>("currentStamina", out float currentStamina);
            playerStatsDatabase.currentStamina = currentStamina;

            quickSaveReader.TryRead<float>("currentMana", out float currentMana);
            playerStatsDatabase.currentMana = currentMana;

            quickSaveReader.TryRead<int>("reputation", out int reputation);
            playerStatsDatabase.reputation = reputation;

            quickSaveReader.TryRead<int>("vitality", out int vitality);
            playerStatsDatabase.vitality = vitality;

            quickSaveReader.TryRead<int>("endurance", out int endurance);
            playerStatsDatabase.endurance = endurance;

            quickSaveReader.TryRead<int>("intelligence", out int intelligence);
            playerStatsDatabase.intelligence = intelligence;

            quickSaveReader.TryRead<int>("strength", out int strength);
            playerStatsDatabase.strength = strength;

            quickSaveReader.TryRead<int>("dexterity", out int dexterity);
            playerStatsDatabase.dexterity = dexterity;

            // Read additional stats only if not from game over
            if (!isFromGameOver)
            {
                quickSaveReader.TryRead<int>("gold", out int gold);
                playerStatsDatabase.gold = gold;

                quickSaveReader.TryRead<int>("lostGold", out int lostGold);
                playerStatsDatabase.lostGold = lostGold;

                quickSaveReader.TryRead<string>("sceneWhereGoldWasLost", out string sceneWhereGoldWasLost);
                playerStatsDatabase.sceneWhereGoldWasLost = sceneWhereGoldWasLost;

                quickSaveReader.TryRead<Vector3>("positionWhereGoldWasLost", out Vector3 positionWhereGoldWasLost);
                playerStatsDatabase.positionWhereGoldWasLost = positionWhereGoldWasLost;
            }
        }

        void LoadPlayerEquipment(QuickSaveReader quickSaveReader)
        {
            quickSaveReader.TryRead<int>("currentWeaponIndex", out int currentWeaponIndex);
            equipmentDatabase.currentWeaponIndex = currentWeaponIndex;

            quickSaveReader.TryRead<int>("currentShieldIndex", out int currentShieldIndex);
            equipmentDatabase.currentShieldIndex = currentShieldIndex;

            quickSaveReader.TryRead<int>("currentArrowIndex", out int currentArrowIndex);
            equipmentDatabase.currentArrowIndex = currentArrowIndex;

            quickSaveReader.TryRead<int>("currentSpellIndex", out int currentSpellIndex);
            equipmentDatabase.currentSpellIndex = currentSpellIndex;

            quickSaveReader.TryRead<int>("currentConsumableIndex", out int currentConsumableIndex);
            equipmentDatabase.currentConsumableIndex = currentConsumableIndex;

            quickSaveReader.TryRead<string[]>("weapons", out string[] weapons);
            if (weapons != null && weapons.Length > 0)
            {
                for (int idx = 0; idx < weapons.Length; idx++)
                {
                    string weaponNameAndLevel = weapons[idx];

                    if (!string.IsNullOrEmpty(weaponNameAndLevel))
                    {
                        Weapon weaponInstance = Resources.Load<Weapon>("Items/Weapons/" + weaponNameAndLevel.Split("|")[0]);

                        if (weaponInstance != null && inventoryDatabase.HasItem(weaponInstance))
                        {
                            equipmentDatabase.weapons[idx] = weaponInstance;

                            string level = weaponNameAndLevel.Split("|")[1];
                            if (int.TryParse(level, out int levelValue))
                            {
                                equipmentDatabase.weapons[idx].level = levelValue;
                            }
                        }
                    }
                }
            }

            // Try to read shields
            quickSaveReader.TryRead<string[]>("shields", out string[] shields);
            if (shields != null && shields.Length > 0)
            {
                for (int idx = 0; idx < shields.Length; idx++)
                {
                    string shieldName = shields[idx];

                    if (!string.IsNullOrEmpty(shieldName))
                    {
                        Shield shieldInstance = Resources.Load<Shield>("Items/Shields/" + shieldName);

                        if (shieldInstance != null)
                        {
                            equipmentDatabase.shields[idx] = shieldInstance;
                        }
                    }
                }
            }

            // Try to read arrows
            quickSaveReader.TryRead<string[]>("arrows", out string[] arrows);
            if (arrows != null && arrows.Length > 0)
            {
                for (int idx = 0; idx < arrows.Length; idx++)
                {
                    string arrowName = arrows[idx];

                    if (!string.IsNullOrEmpty(arrowName))
                    {
                        Arrow arrowInstance = Resources.Load<Arrow>("Items/Arrows/" + arrowName);

                        if (arrowInstance != null)
                        {
                            equipmentDatabase.arrows[idx] = arrowInstance;
                        }
                    }
                }
            }

            // Try to read spells
            quickSaveReader.TryRead<string[]>("spells", out string[] spells);
            if (spells != null && spells.Length > 0)
            {
                for (int idx = 0; idx < spells.Length; idx++)
                {
                    string spellName = spells[idx];

                    if (!string.IsNullOrEmpty(spellName))
                    {
                        Spell spellInstance = Resources.Load<Spell>("Items/Spells/" + spellName);

                        if (spellInstance != null)
                        {
                            equipmentDatabase.spells[idx] = spellInstance;
                        }
                    }
                }
            }

            // Try to read accessories
            quickSaveReader.TryRead<string[]>("accessories", out string[] accessories);
            if (accessories != null && accessories.Length > 0)
            {
                for (int idx = 0; idx < accessories.Length; idx++)
                {
                    string accessoryName = accessories[idx];

                    if (!string.IsNullOrEmpty(accessoryName))
                    {
                        Accessory accessoryInstance = Resources.Load<Accessory>("Items/Accessories/" + accessoryName);

                        if (accessoryInstance != null)
                        {
                            equipmentDatabase.accessories[idx] = accessoryInstance;
                        }
                    }
                }
            }

            // Try to read consumables
            quickSaveReader.TryRead<string[]>("consumables", out string[] consumables);
            if (consumables != null && consumables.Length > 0)
            {
                for (int idx = 0; idx < consumables.Length; idx++)
                {
                    string consumableName = consumables[idx];

                    if (!string.IsNullOrEmpty(consumableName))
                    {
                        Consumable consumableInstance = Resources.Load<Consumable>("Items/Consumables/" + consumableName);

                        if (consumableInstance != null)
                        {
                            equipmentDatabase.consumables[idx] = consumableInstance;
                        }
                    }
                }
            }

            // Try to read helmet
            quickSaveReader.TryRead<string>("helmet", out string helmetName);
            if (!string.IsNullOrEmpty(helmetName))
            {
                Helmet helmetInstance = Resources.Load<Helmet>("Items/Helmets/" + helmetName);

                if (helmetInstance != null)
                {
                    equipmentDatabase.helmet = helmetInstance;
                }
            }
            else
            {
                equipmentDatabase.UnequipHelmet();
            }

            // Try to read armor
            quickSaveReader.TryRead<string>("armor", out string armorName);
            if (!string.IsNullOrEmpty(armorName))
            {
                Armor armorInstance = Resources.Load<Armor>("Items/Armors/" + armorName);

                if (armorInstance != null)
                {
                    equipmentDatabase.armor = armorInstance;
                }
            }
            else
            {
                equipmentDatabase.UnequipArmor();
            }

            // Try to read gauntlet
            quickSaveReader.TryRead<string>("gauntlet", out string gauntletName);
            if (!string.IsNullOrEmpty(gauntletName))
            {
                Gauntlet gauntletInstance = Resources.Load<Gauntlet>("Items/Gauntlets/" + gauntletName);

                if (gauntletInstance != null)
                {
                    equipmentDatabase.gauntlet = gauntletInstance;
                }
            }
            else
            {
                equipmentDatabase.UnequipGauntlet();
            }

            // Try to read legwear
            quickSaveReader.TryRead<string>("legwear", out string legwearName);
            if (!string.IsNullOrEmpty(legwearName))
            {
                Legwear legwearInstance = Resources.Load<Legwear>("Items/Legwears/" + legwearName);

                if (legwearInstance != null)
                {
                    equipmentDatabase.legwear = legwearInstance;
                }
            }
            else
            {
                equipmentDatabase.UnequipLegwear();
            }

            quickSaveReader.TryRead<bool>("isTwoHanding", out bool isTwoHanding);
            equipmentDatabase.isTwoHanding = isTwoHanding;
        }


        void LoadPlayerInventory(QuickSaveReader quickSaveReader)
        {
            inventoryDatabase.ownedItems.Clear();

            quickSaveReader.TryRead("ownedItems", out SerializedDictionary<string, ItemAmount> ownedItems);

            if (ownedItems != null && ownedItems.Count > 0)
            {
                for (int idx = 0; idx < ownedItems.Count; idx++)
                {
                    var itemEntry = ownedItems.ElementAt(idx);

                    if (!string.IsNullOrEmpty(itemEntry.Key))
                    {
                        Item itemInstance = Resources.Load<Item>(itemEntry.Key);

                        if (itemInstance != null)
                        {
                            inventoryDatabase.ownedItems.Add(itemInstance, new()
                            {
                                amount = itemEntry.Value.amount,
                                chanceToGet = itemEntry.Value.chanceToGet,
                                usages = itemEntry.Value.usages
                            });
                        }
                    }
                }
            }
        }

        void LoadPickups(QuickSaveReader quickSaveReader)
        {
            pickupDatabase.Clear();

            quickSaveReader.TryRead("pickups", out SerializedDictionary<string, string> savedPickups);
            pickupDatabase.pickups = savedPickups;
            quickSaveReader.TryRead("replenishables", out SerializedDictionary<string, ReplenishableTime> savedReplenishables);
            pickupDatabase.replenishables = savedReplenishables;
        }

        void LoadQuests(QuickSaveReader quickSaveReader)
        {
            questsDatabase.questsReceived.Clear();

            quickSaveReader.TryRead("questsReceived", out SerializedDictionary<string, int> savedQuestsReceived);

            foreach (var savedQuest in savedQuestsReceived)
            {
                QuestParent questParent = Resources.Load<QuestParent>(savedQuest.Key);
                questParent.questProgress = savedQuest.Value;

                questsDatabase.questsReceived.Add(questParent);
            }

            quickSaveReader.TryRead("currentTrackedQuestIndex", out int currentTrackedQuestIndex);
            questsDatabase.currentTrackedQuestIndex = currentTrackedQuestIndex;
        }

        void LoadFlags(QuickSaveReader quickSaveReader)
        {
            flagsDatabase.flags.Clear();
            quickSaveReader.TryRead("flags", out SerializedDictionary<string, string> savedFlags);

            foreach (var flag in savedFlags)
            {
                flagsDatabase.flags.Add(flag.Key, flag.Value);
            }
        }

        void LoadSceneSettings(QuickSaveReader quickSaveReader)
        {
            gameSession.nextMap_SpawnGameObjectName = null;
            gameSession.loadSavedPlayerPositionAndRotation = true;

            quickSaveReader.TryRead("playerPosition", out Vector3 playerPosition);
            gameSession.savedPlayerPosition = playerPosition;

            quickSaveReader.TryRead("playerRotation", out Quaternion playerRotation);
            gameSession.savedPlayerRotation = playerRotation;

            quickSaveReader.TryRead<int>("sceneIndex", out int sceneIndex);
            SceneManager.LoadScene(sceneIndex);
        }

        void LoadGameSettings(QuickSaveReader quickSaveReader)
        {
            quickSaveReader.TryRead<float>("timeOfDay", out var timeOfDay);
            gameSession.timeOfDay = timeOfDay;

            quickSaveReader.TryRead<int>("graphicsQuality", out var graphicsQuality);
            if (graphicsQuality != -1)
            {
                gameSession.SetGameQuality(graphicsQuality);
            }
            else
            {
                gameSession.SetGameQuality(2);
            }

            quickSaveReader.TryRead<float>("mouseSensitivity", out var mouseSensitivity);
            if (mouseSensitivity > 0)
            {
                gameSession.SetCameraSensitivity(mouseSensitivity);
            }

            quickSaveReader.TryRead<float>("musicVolume", out var musicVolume);
            if (musicVolume != -1)
            {
                gameSession.SetMusicVolume(musicVolume);
            }
        }

        void LoadCompanions(QuickSaveReader quickSaveReader)
        {
            companionsDatabase.companionsInParty.Clear();

            quickSaveReader.TryRead("companionsInParty", out SerializedDictionary<string, CompanionState> savedCompanionsInParty);

            if (savedCompanionsInParty != null && savedCompanionsInParty.Count > 0)
            {
                for (int idx = 0; idx < savedCompanionsInParty.Count; idx++)
                {
                    var itemEntry = savedCompanionsInParty.ElementAt(idx);

                    if (!string.IsNullOrEmpty(itemEntry.Key))
                    {
                        companionsDatabase.companionsInParty.Add(itemEntry.Key, new()
                        {
                            isWaitingForPlayer = itemEntry.Value.isWaitingForPlayer,
                            waitingPosition = itemEntry.Value.waitingPosition,
                            sceneNameWhereCompanionsIsWaitingForPlayer = itemEntry.Value.sceneNameWhereCompanionsIsWaitingForPlayer
                        });
                    }
                }

            }
        }

        void LoadBonfires(QuickSaveReader quickSaveReader)
        {
            bonfiresDatabase.unlockedBonfires.Clear();

            quickSaveReader.TryRead("unlockedBonfires", out string[] unlockedBonfires);

            if (unlockedBonfires != null && unlockedBonfires.Length > 0)
            {
                for (int idx = 0; idx < unlockedBonfires.Length; idx++)
                {
                    bonfiresDatabase.unlockedBonfires.Add(unlockedBonfires[idx]);
                }
            }
        }

        public bool HasSavedGame()
        {
            return SaveUtils.HasSaveFiles(SAVE_FILES_FOLDER);
        }

        public void SaveGameData()
        {
            if (!CanSave())
            {
                notificationManager.ShowNotification("Can not save at this time", null);
                return;
            }

            string saveFileName = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}";

            QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(saveFileName);
            SaveBonfires(quickSaveWriter);
            SaveCompanions(quickSaveWriter);
            SavePlayerStats(quickSaveWriter);
            SavePlayerEquipment(quickSaveWriter);
            SavePlayerInventory(quickSaveWriter);
            SavePickups(quickSaveWriter);
            SaveFlags(quickSaveWriter);
            SaveQuests(quickSaveWriter);
            SaveRecipes(quickSaveWriter);
            SaveSceneSettings(quickSaveWriter);
            SaveGameSettings(quickSaveWriter);
            quickSaveWriter.TryCommit();

            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            if (screenshot != null)
            {
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath + "/" + SAVE_FILES_FOLDER, saveFileName + ".jpg"), screenshot.EncodeToJPG());
            }

            notificationManager.ShowNotification("Game saved", notificationManager.systemSuccess);
        }

        public void LoadLastSavedGame(bool isFromGameOver)
        {
            string lastSave = SaveUtils.GetLastSaveFile(SAVE_FILES_FOLDER);

            LoadSaveFile(lastSave, isFromGameOver);
        }

        public void LoadSaveFile(string saveFileName)
        {
            LoadSaveFile(saveFileName, false);
        }

        void LoadSaveFile(string saveFileName, bool isFromGameOver)
        {
            if (string.IsNullOrEmpty(saveFileName) || !QuickSaveBase.RootExists(saveFileName))
            {
                // Return to title screen if no save game is available
                fadeManager.FadeIn(1f, () =>
                {
                    ResetGameStateAndReturnToTitleScreen();
                });
                return;
            }

            QuickSaveReader quickSaveReader = QuickSaveReader.Create(saveFileName);

            gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;
            fadeManager.FadeIn(1f, () =>
            {
                LoadBonfires(quickSaveReader);
                LoadCompanions(quickSaveReader);
                LoadPlayerStats(quickSaveReader, isFromGameOver);
                LoadPlayerInventory(quickSaveReader);
                LoadPlayerEquipment(quickSaveReader);
                LoadPickups(quickSaveReader);
                LoadFlags(quickSaveReader);
                LoadQuests(quickSaveReader);
                LoadRecipes(quickSaveReader);
                LoadGameSettings(quickSaveReader);
                LoadSceneSettings(quickSaveReader);
            });
        }

        public void ResetGameStateAndReturnToTitleScreen()
        {
            ResetGameState();
            gameSession.gameState = GameSession.GameState.INITIALIZED;
            SceneManager.LoadScene(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGameData();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadLastSavedGame(false);
            }
        }
    }
}
