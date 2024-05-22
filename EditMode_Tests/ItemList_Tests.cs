using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using AF.Triggers;
using AF.UI.EquipmentMenu;
using System.Collections.Generic;
using AF.Inventory;

namespace AF.Tests
{
    public class ItemList_Tests
    {
        private ItemList itemList;
        private EquipmentDatabase equipmentDatabase;

        Weapon weapon;

        [SetUp]
        public void SetUp()
        {
            weapon = ScriptableObject.CreateInstance<Weapon>();
            itemList = new GameObject().AddComponent<ItemList>();
            equipmentDatabase = ScriptableObject.CreateInstance<EquipmentDatabase>();
            itemList.equipmentDatabase = equipmentDatabase;
        }

        [Test]
        public void IsKeyItem_ShouldReturn_FalseForNonKeyItems_AndTrueForKeyItems()
        {
            // Assert
            Assert.IsFalse(itemList.IsKeyItem(ScriptableObject.CreateInstance<Armor>()));
            Assert.IsFalse(itemList.IsKeyItem(ScriptableObject.CreateInstance<Weapon>()));
            Assert.IsFalse(itemList.IsKeyItem(ScriptableObject.CreateInstance<Shield>()));

            Assert.IsTrue(itemList.IsKeyItem(ScriptableObject.CreateInstance<Item>()));
        }

        [Test]
        public void ShouldShowItem_ItemIsNotOfTypeT_ReturnsFalse()
        {
            var item = new KeyValuePair<Item, ItemAmount>(weapon, new ItemAmount());
            bool result = itemList.ShouldShowItem<Shield>(item, 0, false);
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowItem_ItemIsNotKeyItem_ReturnsFalse()
        {
            var item = new KeyValuePair<Item, ItemAmount>(weapon, new ItemAmount());
            bool result = itemList.ShouldShowItem<Weapon>(item, 0, true);
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowItem_ItemIsKeyItemAndSlotDoesNotMatch_ReturnsFalse()
        {
            var item = new KeyValuePair<Item, ItemAmount>(weapon, new ItemAmount());
            bool result = itemList.ShouldShowItem<Weapon>(item, 0, true);
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowItem_ItemIsNotKeyItemAndSlotMatches_ReturnsTrue()
        {
            var item = new KeyValuePair<Item, ItemAmount>(weapon, new ItemAmount());

            equipmentDatabase.EquipWeapon(item.Key as Weapon, 0);

            bool result = itemList.ShouldShowItem<Weapon>(item, 0, false);

            equipmentDatabase.UnequipWeapon(0);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldNotShowEquippedWeapon_SlotDoesNotMatch()
        {
            var item = new KeyValuePair<Item, ItemAmount>(weapon, new ItemAmount());

            equipmentDatabase.EquipWeapon(item.Key as Weapon, 1);

            bool result1 = itemList.ShouldShowItem<Weapon>(item, 0, false);
            bool result2 = itemList.ShouldShowItem<Weapon>(item, 1, false);
            bool result3 = itemList.ShouldShowItem<Weapon>(item, 2, false);

            equipmentDatabase.UnequipWeapon(1);

            Assert.IsFalse(result1);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);
        }

        [Test]
        public void ShouldShowItem_ConsumableAndSlotDoesNotMatch_ReturnsFalse()
        {
            var item = new KeyValuePair<Item, ItemAmount>(new Consumable(), new ItemAmount());
            bool result = itemList.ShouldShowItem<Consumable>(item, 0, true);
            Assert.IsFalse(result);
        }
    }
}
