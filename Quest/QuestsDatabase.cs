using System.Collections.Generic;
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


#if UNITY_EDITOR 

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif
        public void Clear()
        {
            questsReceived.Clear();
            currentTrackedQuestIndex = -1;

            foreach (var quest in Resources.LoadAll<QuestParent>("Quests"))
            {
                quest.SetProgress(-1);
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

        public string GetCurrentTrackedQuestObjective()
        {
            if (currentTrackedQuestIndex == -1)
            {
                return "";
            }

            if (questsReceived == null || questsReceived.Count <= 0)
            {
                return "";
            }

            QuestParent questParent = questsReceived[currentTrackedQuestIndex];

            if (questParent != null && questParent.questProgress >= 0 && questParent.IsCompleted() == false)
            {
                return questParent.questObjectives_LocalizedString[questParent.questProgress].GetLocalizedString();
            }

            return "";
        }

        public void AddQuest(QuestParent questParent)
        {
            if (questParent != null && !questsReceived.Contains(questParent))
            {
                this.questsReceived.Add(questParent);
            }
        }

        public bool ContainsQuest(QuestParent questParent)
        {
            return questsReceived.Contains(questParent);
        }
    }
}
