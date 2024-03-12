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
            inventoryDatabase.Clear();
            pickupDatabase.Clear();
            questsDatabase.Clear();
            companionsDatabase.Clear();
            bonfiresDatabase.Clear();
            flagsDatabase.Clear();
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

            playerStats.Read<float>("currentHealth", (value) =>
            {
                playerStatsDatabase.currentHealth = value;
            });
            playerStats.Read<float>("currentStamina", (value) =>
            {
                playerStatsDatabase.currentStamina = value;
            });
            playerStats.Read<float>("currentMana", (value) =>
            {
                playerStatsDatabase.currentMana = value;
            });
            playerStats.Read<int>("reputation", (value) =>
            {
                playerStatsDatabase.reputation = value;
            });
            playerStats.Read<int>("vitality", (value) =>
            {
                playerStatsDatabase.vitality = value;
            });
            playerStats.Read<int>("endurance", (value) =>
            {
                playerStatsDatabase.endurance = value;
            });
            playerStats.Read<int>("intelligence", (value) =>
            {
                playerStatsDatabase.intelligence = value;
            });
            playerStats.Read<int>("strength", (value) =>
            {
                playerStatsDatabase.strength = value;
            });
            playerStats.Read<int>("dexterity", (value) =>
            {
                playerStatsDatabase.dexterity = value;
            });

            if (!isFromGameOver)
            {
                playerStats.Read<int>("gold", (value) =>
                {
                    playerStatsDatabase.gold = value;
                });

                playerStats.Read<int>("lostGold", (value) =>
                {
                    playerStatsDatabase.lostGold = value;
                });
                playerStats.Read<string>("sceneWhereGoldWasLost", (value) =>
                {
                    playerStatsDatabase.sceneWhereGoldWasLost = value;
                });
                playerStats.Read<Vector3>("positionWhereGoldWasLost", (value) =>
                {
                    playerStatsDatabase.positionWhereGoldWasLost = value;
                });
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

            playerEquipment.Read<int>("currentWeaponIndex", (value) =>
            {
                equipmentDatabase.currentWeaponIndex = value;
            });
            playerEquipment.Read<int>("currentShieldIndex", (value) =>
            {
                equipmentDatabase.currentShieldIndex = value;
            });
            playerEquipment.Read<int>("currentArrowIndex", (value) =>
            {
                equipmentDatabase.currentArrowIndex = value;
            });
            playerEquipment.Read<int>("currentSpellIndex", (value) =>
            {
                equipmentDatabase.currentSpellIndex = value;
            });
            playerEquipment.Read<int>("currentConsumableIndex", (value) =>
            {
                equipmentDatabase.currentConsumableIndex = value;
            });
            playerEquipment.Read<string[]>("weapons", (value) =>
            {
                for (int idx = 0; idx < value.Length; idx++)
                {
                    string weaponNameAndLevel = value[idx];

                    if (!string.IsNullOrEmpty(weaponNameAndLevel))
                    {
                        Weapon weaponInstance = Resources.Load<Weapon>("Items/Weapons/" + weaponNameAndLevel.Split("|")[0]);

                        if (weaponInstance != null)
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
            });
            playerEquipment.Read<string[]>("shields", (value) =>
            {
                for (int idx = 0; idx < value.Length; idx++)
                {
                    string shieldName = value[idx];

                    if (!string.IsNullOrEmpty(shieldName))
                    {
                        Shield shieldInstance = Resources.Load<Shield>("Items/Shields/" + shieldName);

                        if (shieldInstance != null)
                        {
                            equipmentDatabase.shields[idx] = shieldInstance;
                        }
                    }
                }
            });
            playerEquipment.Read<string[]>("arrows", (value) =>
            {
                for (int idx = 0; idx < value.Length; idx++)
                {
                    string arrowName = value[idx];

                    if (!string.IsNullOrEmpty(arrowName))
                    {
                        Arrow arrowInstance = Resources.Load<Arrow>("Items/Arrows/" + arrowName);

                        if (arrowInstance != null)
                        {
                            equipmentDatabase.arrows[idx] = arrowInstance;
                        }
                    }
                }
            });
            playerEquipment.Read<string[]>("spells", (value) =>
            {
                for (int idx = 0; idx < value.Length; idx++)
                {
                    string spellName = value[idx];

                    if (!string.IsNullOrEmpty(spellName))
                    {
                        Spell spellInstance = Resources.Load<Spell>("Items/Spells/" + spellName);

                        if (spellInstance != null)
                        {
                            equipmentDatabase.spells[idx] = spellInstance;
                        }
                    }
                }
            });
            playerEquipment.Read<string[]>("accessories", (value) =>
            {
                for (int idx = 0; idx < value.Length; idx++)
                {
                    string accessoryName = value[idx];

                    if (!string.IsNullOrEmpty(accessoryName))
                    {
                        Accessory accessoryInstance = Resources.Load<Accessory>("Items/Accessories/" + accessoryName);

                        if (accessoryInstance != null)
                        {
                            equipmentDatabase.accessories[idx] = accessoryInstance;
                        }
                    }
                }
            });
            playerEquipment.Read<string[]>("consumables", (value) =>
            {
                for (int idx = 0; idx < value.Length; idx++)
                {
                    string consumableName = value[idx];

                    if (!string.IsNullOrEmpty(consumableName))
                    {
                        Consumable consumableInstance = Resources.Load<Consumable>("Items/Consumables/" + consumableName);

                        if (consumableInstance != null)
                        {
                            equipmentDatabase.consumables[idx] = consumableInstance;
                        }
                    }
                }
            });
            playerEquipment.Read<string>("helmet", (value) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Helmet helmetInstance = Resources.Load<Helmet>("Items/Helmets/" + value);

                    if (helmetInstance != null)
                    {
                        equipmentDatabase.helmet = helmetInstance;
                    }
                }
            });
            playerEquipment.Read<string>("armor", (value) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Armor armorInstance = Resources.Load<Armor>("Items/Armors/" + value);

                    if (armorInstance != null)
                    {
                        equipmentDatabase.armor = armorInstance;
                    }
                }
            });
            playerEquipment.Read<string>("gauntlet", (value) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Gauntlet gauntletInstance = Resources.Load<Gauntlet>("Items/Gauntlets/" + value);

                    if (gauntletInstance != null)
                    {
                        equipmentDatabase.gauntlet = gauntletInstance;
                    }
                }
            });
            playerEquipment.Read<string>("legwear", (value) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Legwear legwearInstance = Resources.Load<Legwear>("Items/Legwears/" + value);

                    if (legwearInstance != null)
                    {
                        equipmentDatabase.legwear = legwearInstance;
                    }
                }
            });
            playerEquipment.Read<bool>("isTwoHanding", (value) =>
            {
                equipmentDatabase.isTwoHanding = value;
            });
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
            inventory.Read<SerializedDictionary<string, ItemAmount>>("ownedItems", (value) =>
            {
                for (int idx = 0; idx < value.Count; idx++)
                {
                    var itemEntry = value.ElementAt(idx);

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
            });
        }

        void SavePickups()
        {
            var pickups = QuickSaveWriter.Create("Pickups");
            pickups.Write("pickups", pickupDatabase.pickups);
            pickups.TryCommit();
        }

        void LoadPickups()
        {
            if (!HasSavedGame())
            {
                return;
            }

            pickupDatabase.Clear();

            var pickups = QuickSaveReader.Create("Pickups");
            pickups.Read<SerializedDictionary<string, string>>("pickups", (pickups) =>
            {
                pickupDatabase.pickups = pickups;
            });
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

            if (!HasSavedGame())
            {
                return;
            }

            var questsReceived = QuickSaveReader.Create("Quests");
            questsReceived.Read<SerializedDictionary<string, int>>("questsReceived", (payload) =>
            {
                foreach (var savedQuest in payload)
                {
                    QuestParent questParent = Resources.Load<QuestParent>(savedQuest.Key);
                    questParent.questProgress = savedQuest.Value;

                    questsDatabase.questsReceived.Add(questParent);
                }
            });

            questsReceived.Read<int>("currentTrackedQuestIndex", (value) =>
            {
                questsDatabase.currentTrackedQuestIndex = value;
            });
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
            flags.Read<SerializedDictionary<string, string>>("flags", (payload) =>
            {
                foreach (var flag in payload)
                {
                    flagsDatabase.flags.Add(flag.Key, flag.Value);
                }
            });
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
            if (!HasSavedGame())
            {
                return;
            }

            var data = QuickSaveReader.Create("Scene");
            data.Read<int>("sceneIndex", (sceneIndex) =>
            {
                SceneManager.LoadScene(sceneIndex);
            });

            data.Read<Vector3>("playerPosition", (value) =>
            {
                gameSession.savedPlayerPosition = value;
            });
            data.Read<Quaternion>("playerRotation", (value) =>
            {
                gameSession.savedPlayerRotation = value;
            });

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
            data.TryRead<int>("graphicsQuality", out var graphicsQuality);
            data.TryRead<float>("mouseSensitivity", out var mouseSensitivity);

            gameSession.timeOfDay = timeOfDay;

            if (graphicsQuality != -1)
            {
                gameSession.SetGameQuality(graphicsQuality);
            }

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

            companions.Read<SerializedDictionary<string, CompanionState>>("companionsInParty", (value) =>
            {
                for (int idx = 0; idx < value.Count; idx++)
                {
                    var itemEntry = value.ElementAt(idx);

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
            });
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

            bonfires.Read<string[]>("unlockedBonfires", (value) =>
            {
                for (int idx = 0; idx < value.Length; idx++)
                {
                    bonfiresDatabase.unlockedBonfires.Add(value[idx]);
                }
            });
        }

        public bool HasSavedGame()
        {
            return QuickSaveReader.Create("Scene").Exists("sceneIndex");
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
            SaveSceneSettings();
            SaveGameSettings();

            notificationManager.ShowNotification("Game saved", notificationManager.systemSuccess);
        }

        public void LoadLastSavedGame(bool isFromGameOver)
        {
            gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;

            fadeManager.FadeIn(1f, () =>
            {
                LoadBonfires();
                LoadCompanions();
                LoadPlayerStats(isFromGameOver);
                LoadPlayerEquipment();
                LoadPlayerInventory();
                LoadPickups();
                LoadFlags();
                LoadQuests();
                LoadSceneSettings();
                LoadGameSettings();
            });
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
