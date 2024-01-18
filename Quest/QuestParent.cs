using System.Collections;
using System.Linq;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Quest")]

    public class QuestParent : ScriptableObject
    {
        [TextArea]
        public new string name;

        public Texture questIcon;

        public QuestObjective[] questObjectives;

        public QuestStatus currentQuestStatus;

        public QuestStatus defaultQuestStatus;

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                this.currentQuestStatus = defaultQuestStatus;
            }
        }

        public bool AllObjectivesAreCompleted()
        {
            return questObjectives.All(questObjective => questObjective.isCompleted);
        }

        public void SetQuestStatus(QuestStatus questStatus)
        {
            this.currentQuestStatus = questStatus;
            EventManager.EmitEvent(EventMessages.ON_QUEST_STATUS_CHANGED);
        }
    }
}
