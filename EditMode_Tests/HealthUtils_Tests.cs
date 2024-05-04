using System;
using NUnit.Framework;
using AF;

namespace AF.Tests
{
    [TestFixture]
    public class HealthUtilsTests
    {
        [TestCase(0, 0)]  // No companions, should return 0 extra health
        [TestCase(1, 1000)]  // One companion, should return 1000 extra health
        [TestCase(2, 2000)]  // Two companions, should return 3000 extra health
        [TestCase(3, 6000)]  // Three companions, should return 6000 extra health
        [TestCase(5, 15000)] // Five companions, should return 15000 extra health
        public void GetExtraHealthBasedOnCompanionsInParty_ReturnsCorrectValue(int numberOfCompanions, int expectedExtraHealth)
        {
            // Arrange

            // Act
            int extraHealth = HealthUtils.GetExtraHealthBasedOnCompanionsInParty(numberOfCompanions);

            // Assert
            Assert.AreEqual(expectedExtraHealth, extraHealth);
        }
    }
}
