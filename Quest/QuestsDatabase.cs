using System.Collections.Generic;
using System.Linq;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(fileName = "Quests Database", menuName = "System/New Quests Database", order = 0)]
    public class QuestsDatabase : ScriptableObject
    {

        [Header("Quests")]
        public List<QuestParent> questsReceived = new();

        public int currentTrackedQuestIndex = -1;

        [Header("Settings")]
        public bool shouldClear = false;

        [Header("Quest Statuses")]
        public QuestStatus questStatusWhenQuestStarts;
        public QuestStatus questStatusWhenAllObjectivesAreCompleted;

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode && shouldClear)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }

        public void Clear()
        {
            questsReceived.Clear();
            currentTrackedQuestIndex = -1;
        }

        public bool IsObjectiveCompleted(QuestObjective questObjective)
        {
            var targetQuestIndex = this.questsReceived.FindIndex(quest => quest.questObjectives.Contains(questObjective));

            if (targetQuestIndex == -1)
            {
                return false;
            }

            return this.questsReceived[targetQuestIndex].questObjectives.FirstOrDefault(x => x == questObjective).isCompleted;
        }

        public void CompleteObjective(QuestObjective questObjectiveToComplete)
        {
            AddQuest(questObjectiveToComplete.questParent);

            var targetQuestIndex = this.questsReceived.FindIndex(quest => quest.questObjectives.Contains(questObjectiveToComplete));

            this.questsReceived[targetQuestIndex].questObjectives.FirstOrDefault(x => x == questObjectiveToComplete).isCompleted = true;
            if (this.questsReceived[targetQuestIndex].AllObjectivesAreCompleted())
            {
                SetQuestToTrack(null);

                this.questsReceived[targetQuestIndex].SetQuestStatus(questStatusWhenAllObjectivesAreCompleted);
            }

            EventManager.EmitEvent(EventMessages.ON_QUEST_OBJECTIVE_COMPLETED);
        }

        public bool IsQuestTracked(QuestParent questParent)
        {
            if (currentTrackedQuestIndex == -1)
            {
                return false;
            }

            return questsReceived[currentTrackedQuestIndex] == questParent;
        }

        public void SetQuestToTrack(QuestParent questParent)
        {
            AddQuest(questParent);

            if (IsQuestTracked(questParent))
            {
                currentTrackedQuestIndex = -1;
            }
            else
            {
                currentTrackedQuestIndex = questsReceived.IndexOf(questParent);
            }

            EventManager.EmitEvent(EventMessages.ON_QUEST_TRACKED);
        }

        public QuestObjective GetCurrentTrackedQuestObjective()
        {
            if (currentTrackedQuestIndex == -1)
            {
                return null;
            }

            return questsReceived[currentTrackedQuestIndex].questObjectives.FirstOrDefault(x => x.isCompleted == false);
        }

        public void AddQuest(QuestParent questParent)
        {
            if (questParent != null && !questsReceived.Contains(questParent))
            {
                questParent.currentQuestStatus = questStatusWhenQuestStarts;
                this.questsReceived.Add(questParent);
                EventManager.EmitEvent(EventMessages.ON_QUEST_STATUS_CHANGED);
            }
        }

        public bool ContainsQuest(QuestParent questParent)
        {
            return questsReceived.Contains(questParent);
        }

        public bool AreQuestObjectivesCompleted(QuestParent questParent)
        {
            if (!ContainsQuest(questParent))
            {
                return false;
            }

            int idx = questsReceived.FindIndex(questReceived => questReceived == questParent);
            return questsReceived[idx].questObjectives.All(IsObjectiveCompleted);
        }
    }
}
