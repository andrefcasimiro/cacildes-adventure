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
        public void ShouldActivateChildren_IfFromObjectiveIsCompleted_AndUntilObjectiveIsNotCompleted()
        {
            QuestParent questParent = ScriptableObject.CreateInstance<QuestParent>();

            // Arrange
            var fromObjective = ScriptableObject.CreateInstance<QuestObjective>();
            fromObjective.isCompleted = false;
            var untilObjective = ScriptableObject.CreateInstance<QuestObjective>();
            untilObjective.isCompleted = false;

            questParent.questObjectives = new QuestObjective[] { fromObjective, untilObjective };

            questDependant.From = fromObjective;
            questDependant.Until = untilObjective;

            QuestsDatabase questsDatabase = ScriptableObject.CreateInstance<QuestsDatabase>();
            questsDatabase.AddQuest(questParent);

            questDependant.questsDatabase = questsDatabase;

            // Quest Not Started: Objectives not started, don't show children
            questDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            // Quest Is On Going: Objective started but not finished, show children
            questDependant.questsDatabase.CompleteObjective(fromObjective);
            questDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            // Quest Is Finished: Both objectives completed, dont show children
            questDependant.questsDatabase.CompleteObjective(untilObjective);
            questDependant.Evaluate();
            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

        }
    }

}