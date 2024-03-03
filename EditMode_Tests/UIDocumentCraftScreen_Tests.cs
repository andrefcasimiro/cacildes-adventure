using AF.Inventory;
using NUnit.Framework;
using UnityEngine;

namespace AF.Tests
{
    public class UIDocumentCraftScreenTests
    {

        UIDocumentCraftScreen uIDocumentCraftScreen;

        InventoryDatabase inventoryDatabase;
        PlayerStatsDatabase playerStatsDatabase;
        PlayerManager playerManager;

        [SetUp]
        public void SetUp()
        {
            inventoryDatabase = ScriptableObject.CreateInstance<InventoryDatabase>();
            playerStatsDatabase = ScriptableObject.CreateInstance<PlayerStatsDatabase>();


            playerManager = new GameObject().AddComponent<PlayerManager>();
            playerManager.playerStatsDatabase = playerStatsDatabase;
            playerManager.playerComponentManager = new GameObject().AddComponent<PlayerComponentManager>();
            playerManager.playerComponentManager.characterController = new GameObject().AddComponent<CharacterController>();

            uIDocumentCraftScreen = new GameObject().AddComponent<UIDocumentCraftScreen>();
            uIDocumentCraftScreen.cursorManager = uIDocumentCraftScreen.gameObject.AddComponent<CursorManager>();
            uIDocumentCraftScreen.playerManager = playerManager;
            uIDocumentCraftScreen.inventoryDatabase = inventoryDatabase;
        }

        [Test]
        public void ReturnsEmptyDescriptionForEmptyItemDescription()
        {
            // Arrange
            var recipe = ScriptableObject.CreateInstance<CraftingRecipe>();
            var resultingItem = ScriptableObject.CreateInstance<Item>();
            resultingItem.shortDescription = "";
            recipe.resultingItem = resultingItem;

            // Act
            var description = uIDocumentCraftScreen.GetItemDescription(recipe);

            // Assert
            Assert.AreEqual("", description);
        }

        [Test]
        public void ReturnsFullDescriptionForShortItemDescription()
        {
            // Arrange
            var recipe = ScriptableObject.CreateInstance<CraftingRecipe>();
            var resultingItem = ScriptableObject.CreateInstance<Item>();
            resultingItem.shortDescription = "This is a short description.";
            recipe.resultingItem = resultingItem;

            // Act
            var description = uIDocumentCraftScreen.GetItemDescription(recipe);

            // Assert
            Assert.AreEqual("This is a short description.", description);
        }

        [Test]
        public void ReturnsTruncatedDescriptionWithEllipsisForLongItemDescription()
        {
            // Arrange
            var recipe = ScriptableObject.CreateInstance<CraftingRecipe>();
            var resultingItem = ScriptableObject.CreateInstance<Item>();
            resultingItem.shortDescription = "This is a very long, long, long, long, long, long, long description that exceeds the character limit.";
            recipe.resultingItem = resultingItem;

            // Act
            var description = uIDocumentCraftScreen.GetItemDescription(recipe);

            // Assert
            Assert.AreEqual("This is a very long, long, long, long, long, long, long desc...", description);
        }
    }
}
