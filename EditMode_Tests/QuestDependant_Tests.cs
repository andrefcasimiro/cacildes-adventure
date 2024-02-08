using UnityEngine;
using NUnit.Framework;
using AF.Conditions;

namespace AF.Tests
{
    public class QuestDependant_Tests : MonoBehaviour
    {
        QuestsDatabase questsDatabase;
        QuestParent questParent;

        QuestDependant questDependant;
        GameObject child1;
        GameObject child2;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new();
            questDependant = go.AddComponent<QuestDependant>();

            questsDatabase = ScriptableObject.CreateInstance<QuestsDatabase>();

            questParent = ScriptableObject.CreateInstance<QuestParent>();
            questDependant.questParent = questParent;

            questParent.questObjectives = new[] { "First Objective", "Second Objective" };
            questParent.questsDatabase = questsDatabase;

            // Add children
            child1 = Instantiate(new GameObject(), questDependant.transform);
            child2 = Instantiate(new GameObject(), questDependant.transform);
        }

        [Test]
        public void ShouldManageChildrenBasedOnCorrectQuestProgressWithinRange()
        {
            questParent.SetProgress(-1);

            questDependant.shouldBeWithinRange = true;
            questDependant.shouldBeOutsideRange = false;
            questDependant.questProgresses = new[] { 0 };

            questDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            questParent.SetProgress(0);

            questDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            questParent.SetProgress(1);

            questDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);
        }

        [Test]
        public void ShouldManageChildrenBasedOnCorrectQuestProgressOutsideRange()
        {
            questDependant.shouldBeWithinRange = false;
            questDependant.shouldBeOutsideRange = true;
            questDependant.questProgresses = new[] { 0 };

            questParent.SetProgress(-1);

            questDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            questParent.SetProgress(0);

            questDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            questParent.SetProgress(1);

            questDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            questParent.SetProgress(2);

            questDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);
        }
    }
}
