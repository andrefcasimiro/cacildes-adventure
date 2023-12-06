using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using AF.Triggers;
using AF.UI.EquipmentMenu;

namespace AF.Tests
{
    public class ItemList_Tests
    {
        private ItemList itemList;

        [SetUp]
        public void SetUp()
        {
            itemList = new GameObject().AddComponent<ItemList>();
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
    }

}