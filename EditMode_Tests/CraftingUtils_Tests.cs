using AF.Inventory;
using NUnit.Framework;
using UnityEngine;

namespace AF.Tests
{
    public class CraftingUtils_Tests
    {

        InventoryDatabase inventoryDatabase;

        [SetUp]
        public void SetUp()
        {
            inventoryDatabase = ScriptableObject.CreateInstance<InventoryDatabase>();
        }


        [Test]
        public void CanCraftItem_ReturnsTrueWhenPlayerHasRequiredMaterials()
        {
            Item resultingItem = ScriptableObject.CreateInstance<Item>();

            CraftingMaterial requiredIngredient = ScriptableObject.CreateInstance<CraftingMaterial>();

            // Arrange
            var recipe = ScriptableObject.CreateInstance<CraftingRecipe>();
            recipe.resultingItem = resultingItem;
            recipe.ingredients = new System.Collections.Generic.List<CraftingIngredientEntry>
            {
                new() { ingredient = requiredIngredient, amount = 3 }
            };

            inventoryDatabase.ownedItems.Clear();
            inventoryDatabase.AddItem(requiredIngredient, 3);

            // Assert
            Assert.IsTrue(CraftingUtils.CanCraftItem(inventoryDatabase, recipe));
        }

        [Test]
        public void CanCraftItem_ReturnsFalseWhenPlayerLacksMaterials()
        {
            Item resultingItem = ScriptableObject.CreateInstance<Item>();

            CraftingMaterial requiredIngredient = ScriptableObject.CreateInstance<CraftingMaterial>();

            // Arrange
            var recipe = ScriptableObject.CreateInstance<CraftingRecipe>();
            recipe.resultingItem = resultingItem;
            recipe.ingredients = new System.Collections.Generic.List<CraftingIngredientEntry>
            {
                new() { ingredient = requiredIngredient, amount = 3 }
            };

            inventoryDatabase.ownedItems.Clear();
            inventoryDatabase.AddItem(requiredIngredient, 2);

            // Assert
            Assert.IsFalse(CraftingUtils.CanCraftItem(inventoryDatabase, recipe));
        }
    }
}
