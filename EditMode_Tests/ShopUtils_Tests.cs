using NUnit.Framework;
using AF.Shops;
using UnityEngine;

namespace AF.Tests
{

    public class ShopUtilsTests
    {
        [Test]
        public void GetItemFinalPrice_NoDiscount_PlayerIsBuying_ReturnsOriginalPrice()
        {
            // Arrange
            Item item = ScriptableObject.CreateInstance<Item>();
            int itemOriginalValue = 100;
            item.value = itemOriginalValue;

            bool playerIsBuying = true;
            float discountPercentage = 0f;

            // Act
            int finalPrice = ShopUtils.GetItemFinalPrice(item, playerIsBuying, discountPercentage);

            // Assert
            Assert.AreEqual(itemOriginalValue, finalPrice);
        }

        [Test]
        public void GetItemFinalPrice_NoDiscount_PlayerIsSelling_ReturnsOriginalPrice()
        {
            // Arrange
            Item item = ScriptableObject.CreateInstance<Item>();
            int itemOriginalValue = 100;
            item.value = itemOriginalValue;

            bool playerIsBuying = false;
            float discountPercentage = 0f;

            // Act
            int finalPrice = ShopUtils.GetItemFinalPrice(item, playerIsBuying, discountPercentage);

            // Assert
            Assert.AreEqual(itemOriginalValue, finalPrice);
        }

        [Test]
        public void GetItemFinalPrice_WithDiscount_PlayerIsBuying_ReturnsDiscountedPrice()
        {
            // Arrange
            Item item = ScriptableObject.CreateInstance<Item>();
            int itemOriginalValue = 100;
            item.value = itemOriginalValue;

            bool playerIsBuying = true;
            float discountPercentage = 0.9f;

            // Act
            int finalPrice = ShopUtils.GetItemFinalPrice(item, playerIsBuying, discountPercentage);

            // Assert
            Assert.AreEqual(11, finalPrice);
        }

        [Test]
        public void GetItemFinalPrice_WithDiscount_PlayerIsSelling_ReturnsIncreasedPrice()
        {
            // Arrange
            Item item = ScriptableObject.CreateInstance<Item>();
            int itemOriginalValue = 100;
            item.value = itemOriginalValue;

            bool playerIsBuying = false;
            float discountPercentage = 0.9f;

            // Act
            int finalPrice = ShopUtils.GetItemFinalPrice(item, playerIsBuying, discountPercentage);

            // Assert
            Assert.AreEqual(189, finalPrice);
        }
    }
}
