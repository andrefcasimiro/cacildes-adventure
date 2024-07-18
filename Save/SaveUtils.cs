using System;
using System.IO;
using System.Linq;
using AF.Companions;
using AF.Inventory;
using AF.Pickups;
using AYellowpaper.SerializedCollections;
using CI.QuickSave;
using UnityEngine;

namespace AF
{
    public static class SaveUtils
    {
        public static bool HasSaveFiles(string saveFilesLocation)
        {
            try
            {
                string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);
                bool filesExist = Directory.Exists(saveFolderPath) && Directory.EnumerateFiles(saveFolderPath).Any();
                return filesExist;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while checking for save files: {e.Message}");
                return false;
            }
        }

        public static string GetLastSaveFile(string saveFilesLocation)
        {
            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            if (!Directory.Exists(saveFolderPath))
            {
                return ""; // Directory doesn't exist, return empty string
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length == 0)
            {
                return ""; // No files found in the directory, return empty string
            }

            // Sort files by creation time in descending order
            FileInfo lastFile = files.Where(x => x.Name.Contains(".json")).OrderByDescending(f => f.CreationTime).First();

            return lastFile.Name.Replace(".json", ""); // Return the full path of the last file
        }

        public static string[] GetSaveFileNames(string saveFilesLocation)
        {
            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            if (!Directory.Exists(saveFolderPath))
            {
                return new string[] { }; // Directory doesn't exist, return empty string
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length == 0)
            {
                return new string[] { }; // Directory doesn't exist, return empty string
            }

            return files
                .OrderByDescending(f => f.CreationTime)
                .Where(x => x.Name.Contains(".json"))
                .Select(x => x.Name.Replace(".json", ""))
                .ToArray();
        }

        public static Texture2D GetScreenshotFilePath(string saveFilesLocation, string fileName)
        {
            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            if (!Directory.Exists(saveFolderPath))
            {

                return null;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length == 0)
            {
                return null;
            }

            string targetFilePath = files.FirstOrDefault(file => file.Name.Replace(".jpg", "") == fileName)?.FullName;

            if (string.IsNullOrEmpty(targetFilePath))
            {
                return null;
            }

            var targetTexture = new Texture2D(2, 2);
            targetTexture.LoadImage(File.ReadAllBytes(targetFilePath));
            return targetTexture;
        }

        public static void CheckAndMigrateOldSaveFiles(string saveFilesLocation)
        {
            if (!QuickSaveReader.RootExists("Scene"))
            {
                return;
            }

            string saveFileName = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}_Migrated";

            QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(saveFileName);
            MigrateBonfires(quickSaveWriter);
            MigrateCompanions(quickSaveWriter);
            MigrateStats(quickSaveWriter);
            MigratePlayerEquipment(quickSaveWriter);
            MigratePlayerInventory(quickSaveWriter);
            MigratePickups(quickSaveWriter);
            MigrateFlags(quickSaveWriter);
            MigrateQuests(quickSaveWriter);
            MigrateRecipes(quickSaveWriter);
            MigrateSceneSettings(quickSaveWriter);
            MigrateGameSettings(quickSaveWriter);

            quickSaveWriter.TryCommit();

            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            string backupFolderPath = Path.Combine(Application.persistentDataPath, "Migrated_Saves_Backup");

            // Create the backup folder if it doesn't exist
            if (!Directory.Exists(backupFolderPath))
            {
                Directory.CreateDirectory(backupFolderPath);
            }

            // Define an array of file names to loop through
            string[] fileNames = { "Bonfires.json", "Companions.json", "Equipment.json", "Flags.json", "GameSession.json", "Inventory.json", "Pickups.json", "PlayerStats.json", "Quests.json", "Recipes.json", "Scene.json" };

            foreach (string fileName in fileNames)
            {
                // Check if the file exists before proceeding
                string filePath = Path.Combine(saveFolderPath, fileName);
                if (File.Exists(filePath))
                {
                    // Define the backup file path
                    string backupFilePath = Path.Combine(backupFolderPath, $"{fileName}");

                    // Create a backup of the file
                    File.Copy(filePath, backupFilePath, true);

                    // Delete the original file
                    File.Delete(filePath);
                }
            }
        }

        static void MigrateStats(QuickSaveWriter quickSaveWriter)
        {
            var playerStats = QuickSaveReader.Create("PlayerStats");

            // Try to read currentHealth using TryRead
            playerStats.TryRead("currentHealth", out float currentHealth);
            quickSaveWriter.Write("currentHealth", currentHealth);

            // Try to read other stats
            playerStats.TryRead<float>("currentStamina", out float currentStamina);
            quickSaveWriter.Write("currentStamina", currentStamina);

            playerStats.TryRead<float>("currentMana", out float currentMana);
            quickSaveWriter.Write("currentMana", currentMana);

            playerStats.TryRead<int>("reputation", out int reputation);
            quickSaveWriter.Write("reputation", reputation);

            playerStats.TryRead<int>("vitality", out int vitality);
            quickSaveWriter.Write("vitality", vitality);

            playerStats.TryRead<int>("endurance", out int endurance);
            quickSaveWriter.Write("endurance", endurance);

            playerStats.TryRead<int>("intelligence", out int intelligence);
            quickSaveWriter.Write("intelligence", intelligence);

            playerStats.TryRead<int>("strength", out int strength);
            quickSaveWriter.Write("strength", strength);

            playerStats.TryRead<int>("dexterity", out int dexterity);
            quickSaveWriter.Write("dexterity", dexterity);

            playerStats.TryRead<int>("gold", out int gold);
            quickSaveWriter.Write("gold", gold);

            playerStats.TryRead<int>("lostGold", out int lostGold);
            quickSaveWriter.Write("lostGold", lostGold);

            playerStats.TryRead<string>("sceneWhereGoldWasLost", out string sceneWhereGoldWasLost);
            quickSaveWriter.Write("sceneWhereGoldWasLost", sceneWhereGoldWasLost);

            playerStats.TryRead<Vector3>("positionWhereGoldWasLost", out Vector3 positionWhereGoldWasLost);
            quickSaveWriter.Write("positionWhereGoldWasLost", positionWhereGoldWasLost);
        }

        static void MigratePlayerEquipment(QuickSaveWriter quickSaveWriter)
        {
            var playerEquipment = QuickSaveReader.Create("Equipment");

            playerEquipment.TryRead<int>("currentWeaponIndex", out int currentWeaponIndex);
            quickSaveWriter.Write("currentWeaponIndex", currentWeaponIndex);

            playerEquipment.TryRead<int>("currentShieldIndex", out int currentShieldIndex);
            quickSaveWriter.Write("currentShieldIndex", currentShieldIndex);

            playerEquipment.TryRead<int>("currentArrowIndex", out int currentArrowIndex);
            quickSaveWriter.Write("currentArrowIndex", currentArrowIndex);

            playerEquipment.TryRead<int>("currentSpellIndex", out int currentSpellIndex);
            quickSaveWriter.Write("currentSpellIndex", currentSpellIndex);

            playerEquipment.TryRead<int>("currentConsumableIndex", out int currentConsumableIndex);
            quickSaveWriter.Write("currentConsumableIndex", currentConsumableIndex);

            playerEquipment.TryRead<string[]>("weapons", out string[] weapons);
            quickSaveWriter.Write("weapons", weapons);

            // Try to read shields
            playerEquipment.TryRead<string[]>("shields", out string[] shields);
            quickSaveWriter.Write("shields", shields);

            // Try to read arrows
            playerEquipment.TryRead<string[]>("arrows", out string[] arrows);
            quickSaveWriter.Write("arrows", arrows);

            // Try to read spells
            playerEquipment.TryRead<string[]>("spells", out string[] spells);
            quickSaveWriter.Write("spells", spells);

            // Try to read accessories
            playerEquipment.TryRead<string[]>("accessories", out string[] accessories);
            quickSaveWriter.Write("accessories", accessories);

            // Try to read consumables
            playerEquipment.TryRead<string[]>("consumables", out string[] consumables);
            quickSaveWriter.Write("consumables", consumables);

            // Try to read helmet
            playerEquipment.TryRead<string>("helmet", out string helmetName);
            quickSaveWriter.Write("helmet", helmetName);

            // Try to read armor
            playerEquipment.TryRead<string>("armor", out string armorName);
            quickSaveWriter.Write("armor", armorName);

            // Try to read gauntlet
            playerEquipment.TryRead<string>("gauntlet", out string gauntletName);
            quickSaveWriter.Write("gauntlet", gauntletName);

            // Try to read legwear
            playerEquipment.TryRead<string>("legwear", out string legwearName);
            quickSaveWriter.Write("legwear", legwearName);

            playerEquipment.TryRead<bool>("isTwoHanding", out bool isTwoHanding);
            quickSaveWriter.Write("isTwoHanding", isTwoHanding);
        }

        static void MigratePlayerInventory(QuickSaveWriter quickSaveWriter)
        {
            var inventory = QuickSaveReader.Create("Inventory");

            inventory.TryRead("ownedItems", out SerializedDictionary<string, ItemAmount> ownedItems);
            quickSaveWriter.Write("ownedItems", ownedItems);
        }

        static void MigratePickups(QuickSaveWriter quickSaveWriter)
        {
            var pickups = QuickSaveReader.Create("Pickups");
            pickups.TryRead("pickups", out SerializedDictionary<string, string> savedPickups);
            quickSaveWriter.Write("pickups", savedPickups ?? new SerializedDictionary<string, string>());
            pickups.TryRead("replenishables", out SerializedDictionary<string, ReplenishableTime> savedReplenishables);
            quickSaveWriter.Write("replenishables", savedReplenishables ?? new SerializedDictionary<string, ReplenishableTime>());
        }

        static void MigrateQuests(QuickSaveWriter quickSaveWriter)
        {
            var questsReceived = QuickSaveReader.Create("Quests");
            questsReceived.TryRead("questsReceived", out SerializedDictionary<string, int> savedQuestsReceived);
            quickSaveWriter.Write("questsReceived", savedQuestsReceived);

            questsReceived.TryRead("currentTrackedQuestIndex", out int currentTrackedQuestIndex);
            quickSaveWriter.Write("currentTrackedQuestIndex", currentTrackedQuestIndex);
        }

        static void MigrateFlags(QuickSaveWriter quickSaveWriter)
        {
            var flags = QuickSaveReader.Create("Flags");
            flags.TryRead("flags", out SerializedDictionary<string, string> savedFlags);
            quickSaveWriter.Write("flags", savedFlags);
        }

        static void MigrateSceneSettings(QuickSaveWriter quickSaveWriter)
        {
            var data = QuickSaveReader.Create("Scene");
            data.TryRead<int>("sceneIndex", out int sceneIndex);
            quickSaveWriter.Write("sceneIndex", sceneIndex);

            data.TryRead("playerPosition", out Vector3 playerPosition);
            quickSaveWriter.Write("playerPosition", playerPosition);

            data.TryRead("playerRotation", out Quaternion playerRotation);
            quickSaveWriter.Write("playerRotation", playerRotation);
        }

        static void MigrateGameSettings(QuickSaveWriter quickSaveWriter)
        {
            var data = QuickSaveReader.Create("GameSession");

            data.TryRead<float>("timeOfDay", out var timeOfDay);
            quickSaveWriter.Write("timeOfDay", timeOfDay);
        }

        static void MigrateCompanions(QuickSaveWriter quickSaveWriter)
        {
            var companions = QuickSaveReader.Create("Companions");
            companions.TryRead("companionsInParty", out SerializedDictionary<string, CompanionState> savedCompanionsInParty);
            quickSaveWriter.Write("companionsInParty", savedCompanionsInParty);
        }

        static void MigrateBonfires(QuickSaveWriter quickSaveWriter)
        {
            var bonfires = QuickSaveReader.Create("Bonfires");

            bonfires.TryRead("unlockedBonfires", out string[] unlockedBonfires);
            quickSaveWriter.Write("unlockedBonfires", unlockedBonfires);
        }

        static void MigrateRecipes(QuickSaveWriter quickSaveWriter)
        {
            var recipes = QuickSaveReader.Create("Recipes");

            recipes.TryRead("craftingRecipes", out string[] craftingRecipes);
            quickSaveWriter.Write("craftingRecipes", craftingRecipes);
        }
    }
}
