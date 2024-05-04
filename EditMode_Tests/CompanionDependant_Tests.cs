using UnityEngine;
using NUnit.Framework;
using AF.Companions;
using AF.Conditions;

namespace AF.Tests
{
    public class CompanionDependant_Tests : MonoBehaviour
    {
        CompanionDependant companionDependant;
        GameObject child1;
        GameObject child2;
        CompanionsDatabase companionsDatabase;

        string companionId = "companion1";
        CompanionID companionID = ScriptableObject.CreateInstance<CompanionID>();

        [SetUp]
        public void SetUp()
        {
            GameObject go = new();

            companionDependant = go.AddComponent<CompanionDependant>();
            companionID.name = companionId;
            companionDependant.companionID = companionID;

            companionsDatabase = ScriptableObject.CreateInstance<CompanionsDatabase>();
            companionDependant.companionsDatabase = companionsDatabase;

            // Add children
            child1 = Instantiate(new GameObject(), companionDependant.transform);
            child2 = Instantiate(new GameObject(), companionDependant.transform);
        }

        [Test]
        public void ShouldActivateChildren_IfCompanionIsInParty_AndIsRequiredToBeInParty()
        {
            companionDependant.requireInParty = true;

            companionDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            // Companion is added
            companionsDatabase.AddToParty(companionId);

            companionDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            // Companion is removed
            companionsDatabase.RemoveFromParty(companionId);

            companionDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);
        }

        [Test]
        public void ShouldActivateChildren_IfCompanionIsNotInParty_AndIsNotRequiredToBeInParty()
        {
            companionDependant.requireInParty = false;

            companionDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            // Companion is added
            companionsDatabase.AddToParty(companionId);

            companionDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            // Companion is removed
            companionsDatabase.RemoveFromParty(companionId);

            companionDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);
        }
    }
}
