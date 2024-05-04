using AF.Health;
using AF.Music;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF.Tests
{

    public class CharacterBossControllerTests
    {
        private GameObject gameObject;
        private CharacterBossController _bossController;
        private CharacterManager _mockCharacterManager;
        private BGMManager _mockBGMManager;
        private SceneSettings _mockSceneSettings;
        private UIDocument uIDocument;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();
            uIDocument = gameObject.AddComponent<UIDocument>();

            _bossController = gameObject.AddComponent<CharacterBossController>();
            _bossController.bossHud = uIDocument;
            _bossController.bossHud.enabled = false;
            _mockCharacterManager = gameObject.AddComponent<CharacterManager>();
            _mockCharacterManager.health = gameObject.AddComponent<CharacterHealth>();

            _mockBGMManager = gameObject.AddComponent<BGMManager>();
            _mockSceneSettings = gameObject.AddComponent<SceneSettings>();
            _mockSceneSettings.bgmManager = _mockBGMManager;

            _bossController.characterManager = _mockCharacterManager;
            _mockCharacterManager.characterBossController = _bossController;
        }

        [Test]
        public void Test_Awake_HidesBossHud()
        {
            // Act
            _bossController.Start();

            // Assert
            Assert.That(_bossController.IsBossHUDEnabled(), Is.False);
        }

    }
}
