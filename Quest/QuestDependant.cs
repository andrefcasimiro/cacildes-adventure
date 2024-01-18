using System.Linq;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Quests
{
    public class QuestDependant : MonoBehaviour
    {
        public QuestParent questParent;

        public QuestStatus[] questStatuses;

        [Header("Quest Status Options")]
        public bool shouldContainAny = true;
        public bool shouldNotContainAny = false;

        [Header("Settings")]
        public bool listenForQuestChanges = true;

        private void Awake()
        {
            Utils.UpdateTransformChildren(transform, false);
        }

        private void Start()
        {
            Evaluate();

            if (listenForQuestChanges)
            {
                EventManager.StartListening(EventMessages.ON_QUEST_STATUS_CHANGED, Evaluate);
            }
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (shouldContainAny)
            {
                isActive = questStatuses.Contains(questParent.currentQuestStatus);
            }
            else if (shouldNotContainAny)
            {
                isActive = !questStatuses.Contains(questParent.currentQuestStatus);
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
