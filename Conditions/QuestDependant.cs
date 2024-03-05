using System.Linq;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class QuestDependant : MonoBehaviour
    {
        public QuestParent questParent;

        public int[] questProgresses;

        [Header("Quest Status Options")]
        public bool shouldBeWithinRange = true;
        public bool shouldBeOutsideRange = false;

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
                EventManager.StartListening(EventMessages.ON_QUESTS_PROGRESS_CHANGED, Evaluate);
            }
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (questParent != null && questProgresses != null)
            {
                if (shouldBeWithinRange)
                {
                    isActive = questProgresses.Contains(questParent.questProgress);
                }
                else if (shouldBeOutsideRange)
                {
                    isActive = !questProgresses.Contains(questParent.questProgress);
                }
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
