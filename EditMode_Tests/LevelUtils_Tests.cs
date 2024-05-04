using System;
using NUnit.Framework;
using AF;

namespace AF.Tests
{
    [TestFixture]
    public class LevelUtils_Tests
    {
        [TestCase(1, 125)]
        [TestCase(3, 625)]
        [TestCase(5, 1125)]
        [TestCase(10, 2375)]
        [TestCase(15, 3625)]
        [TestCase(20, 4875)]
        [TestCase(30, 7375)]
        [TestCase(50, 12375)]
        public void GetRequiredCoinsForGivenLevel(int level, int expectedCoins)
        {
            // Act
            int extraCoins = LevelUtils.GetRequiredExperienceForLevel(level);

            // Assert
            Assert.AreEqual(expectedCoins, extraCoins);
        }
    }
}
