using AF.Health;
using AF.Stats;
using NUnit.Framework;
using UnityEngine;

namespace AF.Tests
{
    public class PlayerDamageResistancesTests
    {
        PlayerDamageResistances playerDamageResistances;
        PlayerManager playerManager;
        DefenseStatManager defenseStatManager;

        PlayerStatsDatabase playerStatsDatabase;
        StatsBonusController statsBonusController;

        [SetUp]
        public void Setup()
        {
            playerDamageResistances = new GameObject().AddComponent<PlayerDamageResistances>();
            playerDamageResistances.damageReductionFactor = 2;

            playerManager = new GameObject().AddComponent<PlayerManager>();
            playerDamageResistances.playerManager = playerManager;
            defenseStatManager = new GameObject().AddComponent<DefenseStatManager>();
            playerManager.defenseStatManager = defenseStatManager;

            statsBonusController = new GameObject().AddComponent<StatsBonusController>();
            playerManager.defenseStatManager.playerStatsBonusController = statsBonusController;

            playerStatsDatabase = ScriptableObject.CreateInstance<PlayerStatsDatabase>();
            playerManager.defenseStatManager.playerStatsDatabase = playerStatsDatabase;
        }

        [Test]
        public void Test_NoDamageReduction_WhenDefenseAbsorptionIsZero()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 0;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 50);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionIs5()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 5;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 48);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionIs50()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 50;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 32);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionIs75()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 75;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 26);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionIs100()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 100;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 22);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionIs150()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 150;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 16);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionIs200()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 200;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 12);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionIs300()
        {
            var incomingDamage = new Damage();
            int initialDamage = 50;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 300;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 8);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionBonusIs30()
        {
            var incomingDamage = new Damage();
            int initialDamage = 100;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 0;
            defenseStatManager.physicalDefenseAbsorption = 30;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 70);
        }

        [Test]
        public void Test_DamageReduction_WhenDefenseAbsorptionBonusIs100()
        {
            var incomingDamage = new Damage();
            int initialDamage = 100;
            incomingDamage.physical = initialDamage;

            playerStatsDatabase.endurance = 0;
            defenseStatManager.basePhysicalDefense = 0;
            defenseStatManager.physicalDefenseAbsorption = 100;

            var filteredDamage = playerDamageResistances.FilterIncomingDamage(incomingDamage);

            Assert.AreNotEqual(initialDamage, filteredDamage.physical);
            Assert.AreEqual(filteredDamage.physical, 0);
        }

    }
}
