using NUnit.Framework;
using UnityEngine;

namespace AF
{

    public class Weapon_Tests
    {
        private Weapon weapon;
        private UpgradeMaterial iron;

        [SetUp]
        public void Setup()
        {
            // Initialize a mock weapon instance for testing
            weapon = ScriptableObject.CreateInstance<Weapon>();
            iron = ScriptableObject.CreateInstance<UpgradeMaterial>();
            iron.name = "Iron";

            weapon.level = 1; // Assuming the weapon is at level 1

            WeaponUpgradeLevel weaponUpgradeLevel1 = new()
            {
                newDamage = new() { physical = 20 },
                goldCostForUpgrade = 200,
                upgradeMaterials = new()
            };
            weaponUpgradeLevel1.upgradeMaterials.Add(iron, 1);

            WeaponUpgradeLevel weaponUpgradeLevel2 = new()
            {
                newDamage = new() { physical = 50 },
                goldCostForUpgrade = 500,
                upgradeMaterials = new()
            };
            weaponUpgradeLevel2.upgradeMaterials.Add(iron, 3);

            weapon.weaponUpgrades = new[] { weaponUpgradeLevel1, weaponUpgradeLevel2 };
        }

        [Test]
        public void GetMaterialCostForNextLevel_CanBeUpgraded_ReturnsMaterialCost()
        {
            // Arrange
            string expectedOutput = "Next Weapon Level: 2\n" +
                                    "+20 Physical ATK\n" +
                                    "Required Gold: 200 Coins\n" +
                                    "Required Items:\n" +
                                    "- Iron: x1\n";

            // Act
            string actualOutput = weapon.GetMaterialCostForNextLevel();

            // Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void GetMaterialCostForNextNextLevel_CanBeUpgraded_ReturnsMaterialCost()
        {
            weapon.level = 2;
            // Arrange
            string expectedOutput = "Next Weapon Level: 3\n" +
                                    "+50 Physical ATK\n" +
                                    "Required Gold: 500 Coins\n" +
                                    "Required Items:\n" +
                                    "- Iron: x3\n";

            // Act
            string actualOutput = weapon.GetMaterialCostForNextLevel();

            // Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void GetMaterialCostForNextLevel_CannotBeUpgraded_ReturnsEmptyString()
        {
            // Arrange
            weapon.level = 2; // Assuming the weapon is already at max level
            weapon.canBeUpgraded = false;
            // Act
            string result = weapon.GetMaterialCostForNextLevel();

            // Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void GetMaterialCostForNextLevel_UpgradeMaterialsIsNull_ReturnsEmptyString()
        {
            // Arrange
            weapon.canBeUpgraded = true;
            weapon.weaponUpgrades[0].upgradeMaterials = null;

            // Act
            string result = weapon.GetMaterialCostForNextLevel();

            // Assert
            Assert.AreEqual("", result);
        }
    }

}
