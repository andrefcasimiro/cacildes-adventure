using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using UnityEngine.Events;

namespace AF
{
    public static class CraftingUtils
    {
        public static bool CanCraftItem(InventoryDatabase inventoryDatabase, CraftingRecipe recipe)
        {
            bool hasEnoughMaterial = true;

            foreach (var ingredient in recipe.ingredients)
            {
                var itemEntry = inventoryDatabase.HasItem(ingredient.ingredient)
                    ? inventoryDatabase.ownedItems[ingredient.ingredient]
                    : null;

                if (itemEntry == null)
                {
                    hasEnoughMaterial = false;
                    break;
                }

                if (itemEntry.amount >= ingredient.amount)
                {
                    hasEnoughMaterial = true;
                }
                else
                {
                    hasEnoughMaterial = false;
                    break;
                }
            }

            return hasEnoughMaterial;
        }

        public static bool CanImproveWeapon(InventoryDatabase inventoryDatabase, Weapon weapon, int ownedGold)
        {
            WeaponUpgradeLevel nextWeaponUpgradeLevel = weapon.weaponUpgrades.ElementAtOrDefault(weapon.level - 1);

            if (nextWeaponUpgradeLevel == null)
            {
                return false;
            }

            bool hasAllMaterialsRequired = true;

            foreach (var upgradeMaterial in nextWeaponUpgradeLevel.upgradeMaterials)
            {
                if (!inventoryDatabase.HasItem(upgradeMaterial.Key) || inventoryDatabase.GetItemAmount(upgradeMaterial.Key) < upgradeMaterial.Value)
                {
                    hasAllMaterialsRequired = false;
                    break;
                }
            }

            if (!hasAllMaterialsRequired)
            {
                return false;
            }

            if (ownedGold < nextWeaponUpgradeLevel.goldCostForUpgrade)
            {
                return false;
            }

            return true;
        }

        public static void UpgradeWeapon(
            Weapon weapon,
            UnityAction<int> onUpgrade,
            UnityAction<KeyValuePair<UpgradeMaterial, int>> onUpgradeMaterialUsed
        )
        {
            var currentWeaponLevel = weapon.level;

            WeaponUpgradeLevel weaponUpgradeForNextLevel = weapon.weaponUpgrades.ElementAtOrDefault(currentWeaponLevel - 1);

            onUpgrade(weaponUpgradeForNextLevel.goldCostForUpgrade);

            foreach (var upgradeMaterial in weaponUpgradeForNextLevel.upgradeMaterials)
            {
                onUpgradeMaterialUsed(upgradeMaterial);
            }

            weapon.level++;
        }
    }
}
