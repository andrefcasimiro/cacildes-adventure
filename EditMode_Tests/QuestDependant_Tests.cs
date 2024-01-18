using UnityEngine;
using NUnit.Framework;
using AF.Quests;

namespace AF.Tests
{

    public class QuestDependant_Tests : MonoBehaviour
    {
        QuestDependant questDependant;
        GameObject child1;
        GameObject child2;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new();
            questDependant = go.AddComponent<QuestDependant>();

            // Add children
            child1 = Instantiate(new GameObject(), questDependant.transform);
            child2 = Instantiate(new GameObject(), questDependant.transform);
        }

        [Test]
        public void ShouldActivateChildren_IfRequiredQuestStatusIs_Given()
        {
            QuestParent questParent = ScriptableObject.CreateInstance<QuestParent>();
            QuestStatus notGiven = new QuestStatus();
            QuestStatus given = new QuestStatus();

            questParent.currentQuestStatus = notGiven;

            // Arrange
            var fromObjective = ScriptableObject.CreateInstance<QuestObjective>();
            fromObjective.isCompleted = false;
            var untilObjective = ScriptableObject.CreateInstance<QuestObjective>();
            untilObjective.isCompleted = false;

            questParent.questObjectives = new QuestObjective[] { fromObjective, untilObjective };

            questDependant.questParent = questParent;

            QuestsDatabase questsDatabase = ScriptableObject.CreateInstance<QuestsDatabase>();
            questsDatabase.questStatusWhenQuestStarts = given;

            questDependant.questStatuses = new[] { given };
            questDependant.shouldContainAny = true;

            // Quest Not Started: Objectives not started, don't show children
            questDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            // Receive quest
            questsDatabase.AddQuest(questParent);

            questDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

        }
    }
}
