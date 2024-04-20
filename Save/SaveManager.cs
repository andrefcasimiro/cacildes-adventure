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

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MOMENT_START, () => { hasMomentOnGoing = true; });
            EventManager.StartListening(EventMessages.ON_MOMENT_END, () => { hasMomentOnGoing = false; });
            EventManager.StartListening(EventMessages.ON_BOSS_BATTLE_BEGINS, () => { hasBossFightOnGoing = true; });
            EventManager.StartListening(EventMessages.ON_BOSS_BATTLE_ENDS, () => { hasBossFightOnGoing = false; });
        }

        public bool CanSave()
        {
            return hasMomentOnGoing == false && hasBossFightOnGoing == false;
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

        void SaveRecipes()
        {
            var recipes = QuickSaveWriter.Create("Recipes");
            recipes.Write("craftingRecipes", recipesDatabase.craftingRecipes.Select(craftingRecipe => craftingRecipe.name));
            recipes.TryCommit();
        }

        void LoadRecipes()
        {
            var recipes = QuickSaveReader.Create("Recipes");

            recipes.TryRead("craftingRecipes", out string[] craftingRecipes);

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

        void SavePlayerStats()
        {
            var playerStats = QuickSaveWriter.Create("PlayerStats");
            playerStats.Write("currentHealth", playerStatsDatabase.currentHealth);
            playerStats.Write("currentStamina", playerStatsDatabase.currentStamina);
            playerStats.Write("currentMana", playerStatsDatabase.currentMana);
            playerStats.Write("reputation", playerStatsDatabase.reputation);
            playerStats.Write("vitality", playerStatsDatabase.vitality);
            playerStats.Write("endurance", playerStatsDatabase.endurance);
            playerStats.Write("intelligence", playerStatsDatabase.intelligence);
            playerStats.Write("strength", playerStatsDatabase.strength);
            playerStats.Write("dexterity", playerStatsDatabase.dexterity);
            playerStats.Write("gold", playerStatsDatabase.gold);
            playerStats.Write("lostGold", playerStatsDatabase.lostGold);
            playerStats.Write("sceneWhereGoldWasLost", playerStatsDatabase.sceneWhereGoldWasLost);
            playerStats.Write("positionWhereGoldWasLost", playerStatsDatabase.positionWhereGoldWasLost);
            playerStats.TryCommit();
        }

        void LoadPlayerStats(bool isFromGameOver)
        {
            var playerStats = QuickSaveReader.Create("PlayerStats");

            // Try to read currentHealth using TryRead
            playerStats.TryRead("currentHealth", out float currentHealth);
            playerStatsDatabase.currentHealth = currentHealth;

            // Try to read other stats
            playerStats.TryRead<float>("currentStamina", out float currentStamina);
            playerStatsDatabase.currentStamina = currentStamina;

            playerStats.TryRead<float>("currentMana", out float currentMana);
            playerStatsDatabase.currentMana = currentMana;

            playerStats.TryRead<int>("reputation", out int reputation);
            playerStatsDatabase.reputation = reputation;

            playerStats.TryRead<int>("vitality", out int vitality);
            playerStatsDatabase.vitality = vitality;

            playerStats.TryRead<int>("endurance", out int endurance);
            playerStatsDatabase.endurance = endurance;

            playerStats.TryRead<int>("intelligence", out int intelligence);
            playerStatsDatabase.intelligence = intelligence;

            playerStats.TryRead<int>("strength", out int strength);
            playerStatsDatabase.strength = strength;

            playerStats.TryRead<int>("dexterity", out int dexterity);
            playerStatsDatabase.dexterity = dexterity;

            // Read additional stats only if not from game over
            if (!isFromGameOver)
            {
                playerStats.TryRead<int>("gold", out int gold);
                playerStatsDatabase.gold = gold;

                playerStats.TryRead<int>("lostGold", out int lostGold);
                playerStatsDatabase.lostGold = lostGold;

                playerStats.TryRead<string>("sceneWhereGoldWasLost", out string sceneWhereGoldWasLost);
                playerStatsDatabase.sceneWhereGoldWasLost = sceneWhereGoldWasLost;

                playerStats.TryRead<Vector3>("positionWhereGoldWasLost", out Vector3 positionWhereGoldWasLost);
                playerStatsDatabase.positionWhereGoldWasLost = positionWhereGoldWasLost;
            }
        }

        void SavePlayerEquipment()
        {
            var equipment = QuickSaveWriter.Create("Equipment");
            equipment.Write("currentWeaponIndex", equipmentDatabase.currentWeaponIndex);
            equipment.Write("currentShieldIndex", equipmentDatabase.currentShieldIndex);
            equipment.Write("currentArrowIndex", equipmentDatabase.currentArrowIndex);
            equipment.Write("currentSpellIndex", equipmentDatabase.currentSpellIndex);
            equipment.Write("currentConsumableIndex", equipmentDatabase.currentConsumableIndex);
            equipment.Write("weapons", equipmentDatabase.weapons.Select(weapon => weapon != null ? weapon.name + "|" + weapon.level : ""));
            equipment.Write("shields", equipmentDatabase.shields.Select(shield => shield != null ? shield.name : ""));
            equipment.Write("arrows", equipmentDatabase.arrows.Select(arrow => arrow != null ? arrow.name : ""));
            equipment.Write("spells", equipmentDatabase.spells.Select(spell => spell != null ? spell.name : ""));
            equipment.Write("accessories", equipmentDatabase.accessories.Select(accessory => accessory != null ? accessory.name : ""));
            equipment.Write("consumables", equipmentDatabase.consumables.Select(consumable => consumable != null ? consumable.name : ""));
            equipment.Write("helmet", equipmentDatabase.helmet != null ? equipmentDatabase.helmet.name : "");
            equipment.Write("armor", equipmentDatabase.armor != null ? equipmentDatabase.armor.name : "");
            equipment.Write("gauntlet", equipmentDatabase.gauntlet != null ? equipmentDatabase.gauntlet.name : "");
            equipment.Write("legwear", equipmentDatabase.legwear != null ? equipmentDatabase.legwear.name : "");
            equipment.Write("isTwoHanding", equipmentDatabase.isTwoHanding);

            equipment.TryCommit();
        }

        void LoadPlayerEquipment()
        {
            var playerEquipment = QuickSaveReader.Create("Equipment");

            playerEquipment.TryRead<int>("currentWeaponIndex", out int currentWeaponIndex);
            equipmentDatabase.currentWeaponIndex = currentWeaponIndex;

            playerEquipment.TryRead<int>("currentShieldIndex", out int currentShieldIndex);
            equipmentDatabase.currentShieldIndex = currentShieldIndex;

            playerEquipment.TryRead<int>("currentArrowIndex", out int currentArrowIndex);
            equipmentDatabase.currentArrowIndex = currentArrowIndex;

            playerEquipment.TryRead<int>("currentSpellIndex", out int currentSpellIndex);
            equipmentDatabase.currentSpellIndex = currentSpellIndex;

            playerEquipment.TryRead<int>("currentConsumableIndex", out int currentConsumableIndex);
            equipmentDatabase.currentConsumableIndex = currentConsumableIndex;

            playerEquipment.TryRead<string[]>("weapons", out string[] weapons);
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
            playerEquipment.TryRead<string[]>("shields", out string[] shields);
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
            playerEquipment.TryRead<string[]>("arrows", out string[] arrows);
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
            playerEquipment.TryRead<string[]>("spells", out string[] spells);
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
            playerEquipment.TryRead<string[]>("accessories", out string[] accessories);
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
            playerEquipment.TryRead<string[]>("consumables", out string[] consumables);
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
            playerEquipment.TryRead<string>("helmet", out string helmetName);
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
            playerEquipment.TryRead<string>("armor", out string armorName);
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
            playerEquipment.TryRead<string>("gauntlet", out string gauntletName);
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
            playerEquipment.TryRead<string>("legwear", out string legwearName);
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

            playerEquipment.TryRead<bool>("isTwoHanding", out bool isTwoHanding);
            equipmentDatabase.isTwoHanding = isTwoHanding;
        }

        void SavePlayerInventory()
        {
            var inventory = QuickSaveWriter.Create("Inventory");

            SerializedDictionary<string, ItemAmount> keyValuePairs = new();

            foreach (var ownedItem in inventoryDatabase.ownedItems)
            {
                string path = Utils.GetItemPath(ownedItem.Key);

                if (!keyValuePairs.ContainsKey(path))
                {
                    keyValuePairs.Add(path, ownedItem.Value);
                }
            }

            inventory.Write("ownedItems", keyValuePairs);
            inventory.TryCommit();
        }

        void LoadPlayerInventory()
        {
            inventoryDatabase.ownedItems.Clear();

            var inventory = QuickSaveReader.Create("Inventory");

            inventory.TryRead("ownedItems", out SerializedDictionary<string, ItemAmount> ownedItems);

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

        void SavePickups()
        {
            var pickups = QuickSaveWriter.Create("Pickups");
            pickups.Write("pickups", pickupDatabase.pickups);
            pickups.TryCommit();
        }

        void LoadPickups()
        {
            pickupDatabase.Clear();

            var pickups = QuickSaveReader.Create("Pickups");
            pickups.TryRead("pickups", out SerializedDictionary<string, string> savedPickups);
            pickupDatabase.pickups = savedPickups;
        }

        void SaveQuests()
        {
            var quests = QuickSaveWriter.Create("Quests");

            SerializedDictionary<string, int> payload = new();

            questsDatabase.questsReceived.ForEach(questReceived =>
            {
                payload.Add("Quests/" + questReceived.name,
                    questReceived.questProgress);
            });

            quests.Write("questsReceived", payload);
            quests.Write("currentTrackedQuestIndex", questsDatabase.currentTrackedQuestIndex);

            quests.TryCommit();
        }

        void LoadQuests()
        {
            questsDatabase.questsReceived.Clear();

            var questsReceived = QuickSaveReader.Create("Quests");
            questsReceived.TryRead("questsReceived", out SerializedDictionary<string, int> savedQuestsReceived);

            foreach (var savedQuest in savedQuestsReceived)
            {
                QuestParent questParent = Resources.Load<QuestParent>(savedQuest.Key);
                questParent.questProgress = savedQuest.Value;

                questsDatabase.questsReceived.Add(questParent);
            }

            questsReceived.TryRead("currentTrackedQuestIndex", out int currentTrackedQuestIndex);
            questsDatabase.currentTrackedQuestIndex = currentTrackedQuestIndex;
        }

        void SaveFlags()
        {
            var flags = QuickSaveWriter.Create("Flags");
            flags.Write("flags", flagsDatabase.flags);
            flags.TryCommit();
        }

        void LoadFlags()
        {
            flagsDatabase.flags.Clear();

            var flags = QuickSaveReader.Create("Flags");
            flags.TryRead("flags", out SerializedDictionary<string, string> savedFlags);

            foreach (var flag in savedFlags)
            {
                flagsDatabase.flags.Add(flag.Key, flag.Value);
            }
        }

        void SaveSceneSettings()
        {
            var data = QuickSaveWriter.Create("Scene");
            data.Write("sceneIndex", SceneManager.GetActiveScene().buildIndex);
            data.Write("playerPosition", playerManager.transform.position);
            data.Write("playerRotation", playerManager.transform.rotation);
            data.TryCommit();
        }

        void LoadSceneSettings()
        {
            var data = QuickSaveReader.Create("Scene");
            data.TryRead<int>("sceneIndex", out int sceneIndex);
            SceneManager.LoadScene(sceneIndex);

            data.TryRead("playerPosition", out Vector3 playerPosition);
            gameSession.savedPlayerPosition = playerPosition;


            data.TryRead("playerRotation", out Quaternion playerRotation);
            gameSession.savedPlayerRotation = playerRotation;

            gameSession.nextMap_SpawnGameObjectName = null;
            gameSession.loadSavedPlayerPositionAndRotation = true;
        }

        void SaveGameSettings()
        {
            var data = QuickSaveWriter.Create("GameSession");
            data.Write("timeOfDay", gameSession.timeOfDay);
            data.Write("graphicsQuality", gameSession.graphicsQuality);
            data.Write("mouseSensitivity", gameSession.mouseSensitivity);
            data.TryCommit();
        }


        void LoadGameSettings()
        {
            var data = QuickSaveReader.Create("GameSession");
            data.TryRead<float>("timeOfDay", out var timeOfDay);
            gameSession.timeOfDay = timeOfDay;

            data.TryRead<int>("graphicsQuality", out var graphicsQuality);
            if (graphicsQuality != -1)
            {
                gameSession.SetGameQuality(graphicsQuality);
            }
            else
            {
                gameSession.SetGameQuality(2);
            }

            data.TryRead<float>("mouseSensitivity", out var mouseSensitivity);
            if (mouseSensitivity > 0)
            {
                gameSession.mouseSensitivity = mouseSensitivity;
            }
        }

        void SaveCompanions()
        {
            var companions = QuickSaveWriter.Create("Companions");
            companions.Write("companionsInParty", companionsDatabase.companionsInParty);
            companions.TryCommit();
        }

        void LoadCompanions()
        {
            companionsDatabase.companionsInParty.Clear();
            var companions = QuickSaveReader.Create("Companions");

            companions.TryRead("companionsInParty", out SerializedDictionary<string, CompanionState> savedCompanionsInParty);

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

        void SaveBonfires()
        {
            var bonfires = QuickSaveWriter.Create("Bonfires");
            bonfires.Write("unlockedBonfires", bonfiresDatabase.unlockedBonfires);
            bonfires.TryCommit();
        }

        void LoadBonfires()
        {
            bonfiresDatabase.unlockedBonfires.Clear();
            var bonfires = QuickSaveReader.Create("Bonfires");

            bonfires.TryRead("unlockedBonfires", out string[] unlockedBonfires);

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
            return QuickSaveReader.RootExists("Scene");
        }

        public void SaveGameData()
        {
            SaveBonfires();
            SaveCompanions();
            SavePlayerStats();
            SavePlayerEquipment();
            SavePlayerInventory();
            SavePickups();
            SaveFlags();
            SaveQuests();
            SaveRecipes();
            SaveSceneSettings();
            SaveGameSettings();

            notificationManager.ShowNotification("Game saved", notificationManager.systemSuccess);
        }

        public void LoadLastSavedGame(bool isFromGameOver)
        {
            if (HasSavedGame())
            {
                gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;

                fadeManager.FadeIn(1f, () =>
                {
                    LoadBonfires();
                    LoadCompanions();
                    LoadPlayerStats(isFromGameOver);
                    LoadPlayerInventory();
                    LoadPlayerEquipment();
                    LoadPickups();
                    LoadFlags();
                    LoadQuests();
                    LoadRecipes();
                    LoadSceneSettings();
                    LoadGameSettings();
                });
            }
            else
            {
                // Return to title screen if no save game is available
                fadeManager.FadeIn(1f, () =>
                {
                    ResetGameStateAndReturnToTitleScreen();
                });
            }
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
