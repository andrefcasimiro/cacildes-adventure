using System.Linq;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Quests
{
    public class QuestObjectiveDependant : MonoBehaviour
    {
        public QuestObjective questObjective;

        public bool requireObjectiveToBeCompleted = true;

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
                EventManager.StartListening(EventMessages.ON_QUEST_OBJECTIVE_COMPLETED, Evaluate);
            }
        }

        public void Evaluate()
        {
            Utils.UpdateTransformChildren(transform, questObjective.isCompleted == requireObjectiveToBeCompleted);
        }
    }
}
