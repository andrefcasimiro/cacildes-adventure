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
        }

        public void CompleteObjective(QuestObjective questObjectiveToComplete)
        {
            var targetQuestIndex = this.questsReceived.FindIndex(quest => quest.questObjectives.Contains(questObjectiveToComplete));

            if (targetQuestIndex == -1)
            {
                return;
            }

            this.questsReceived[targetQuestIndex].questObjectives.FirstOrDefault(x => x == questObjectiveToComplete).isCompleted = true;

            if (this.questsReceived[targetQuestIndex].IsCompleted())
            {
                SetQuestToTrack(null);
            }
            else
            {
                EventManager.EmitEvent(EventMessages.ON_QUEST_TRACKED);
            }
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

    }
}
