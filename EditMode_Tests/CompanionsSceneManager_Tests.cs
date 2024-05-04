using System.Text.RegularExpressions;
using AF.Companions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace AF.Tests
{
    public class CompanionsSceneManagerTests
    {
        private CompanionsSceneManager companionsSceneManager;
        private CompanionsDatabase companionsDatabase;
        private PlayerManager playerManager;

        private string companionWarriorID = "companionWarrior";
        CharacterManager companionWarriorCharacterManager;
        private string companionSorcererID = "companionSorcerer";
        CharacterManager companionSorcererCharacterManager;


        [SetUp]
        public void SetUp()
        {
            companionsSceneManager = new GameObject().AddComponent<CompanionsSceneManager>();
            companionsDatabase = ScriptableObject.CreateInstance<CompanionsDatabase>();
            companionsSceneManager.companionsDatabase = companionsDatabase;
            playerManager = new GameObject().AddComponent<PlayerManager>();

            companionsSceneManager.playerManager = playerManager;

            companionWarriorCharacterManager = new GameObject().AddComponent<CharacterManager>();
            companionWarriorCharacterManager.characterController = companionWarriorCharacterManager.gameObject.AddComponent<CharacterController>();
            companionWarriorCharacterManager.agent = companionWarriorCharacterManager.gameObject.AddComponent<NavMeshAgent>();

            CompanionID companionWarrior = ScriptableObject.CreateInstance<CompanionID>();
            companionWarrior.name = companionWarriorID;
            companionWarriorCharacterManager.companionID = companionWarrior;

            companionSorcererCharacterManager = new GameObject().AddComponent<CharacterManager>();
            companionSorcererCharacterManager.characterController = companionSorcererCharacterManager.gameObject.AddComponent<CharacterController>();
            companionSorcererCharacterManager.agent = companionSorcererCharacterManager.gameObject.AddComponent<NavMeshAgent>();

            CompanionID companionSorcerer = ScriptableObject.CreateInstance<CompanionID>();
            companionSorcerer.name = companionSorcererID;
            companionSorcererCharacterManager.companionID = companionSorcerer;

            companionsSceneManager.companionPrefabs = new GameObject[] { companionWarriorCharacterManager.gameObject, companionSorcererCharacterManager.gameObject };
        }

        [Test]
        public void Test_ClearInactiveCompanions_RemovesInactiveCompanions()
        {
            companionsSceneManager.companionInstancesInScene.Add(companionWarriorID, companionWarriorCharacterManager.gameObject);
            companionsSceneManager.companionInstancesInScene.Add(companionSorcererID, companionSorcererCharacterManager.gameObject);

            companionsDatabase.AddToParty(companionWarriorID);
            companionsDatabase.RemoveFromParty(companionSorcererID);

            // Expect the specific error message in play mode
            LogAssert.Expect(LogType.Error, new Regex("Destroy may not be called from edit mode!"));

            // Act
            companionsSceneManager.ClearInactiveCompanions();

            // Assert
            Assert.That(companionsSceneManager.companionInstancesInScene.Count, Is.EqualTo(1));
            Assert.IsTrue(companionsSceneManager.companionInstancesInScene.ContainsKey(companionWarriorID));
            Assert.IsFalse(companionsSceneManager.companionInstancesInScene.ContainsKey(companionSorcererID));
        }

        [Test]
        public void Test_HandleWaitingCompanions_SpawnsAndTeleportsWaitingCompanions()
        {
            companionsSceneManager.companionInstancesInScene.Add(companionWarriorID, companionWarriorCharacterManager.gameObject);
            companionsSceneManager.companionInstancesInScene.Add(companionSorcererID, companionSorcererCharacterManager.gameObject);

            companionsDatabase.AddToParty(companionWarriorID);
            companionsDatabase.AddToParty(companionSorcererID);
            companionsDatabase.WaitForPlayer(companionWarriorID, new()
            {
                isWaitingForPlayer = true,
                waitingPosition = new Vector3(1, 1, 1),
                sceneNameWhereCompanionsIsWaitingForPlayer = "Scene"
            });

            // Act
            companionsSceneManager.HandleWaitingCompanions();

            // Assert
            Assert.That(companionsSceneManager.companionInstancesInScene.Count, Is.EqualTo(2));
            Assert.IsTrue(companionsSceneManager.companionInstancesInScene[companionWarriorID].transform.position == new Vector3(1, 1, 1));
        }
    }
}
