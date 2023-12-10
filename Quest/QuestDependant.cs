using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Quests
{
    public class QuestDependant : MonoBehaviour
    {
        [Header("Databases")]
        public QuestsDatabase questsDatabase;

        [Header("Conditions")]
        public QuestObjective From;

        public QuestObjective Until;

        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        private void Start()
        {
            Evaluate();

            EventManager.StartListening(EventMessages.ON_QUEST_OBJECTIVE_COMPLETED, Evaluate);
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (From != null && Until != null && questsDatabase.IsObjectiveCompleted(From) == true && questsDatabase.IsObjectiveCompleted(Until) == false)
            {
                isActive = true;
            }
            else if (From == null && Until != null && questsDatabase.IsObjectiveCompleted(Until) == false)
            {
                isActive = true;
            }
            else if (From != null && Until == null && questsDatabase.IsObjectiveCompleted(From))
            {
                isActive = true;
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
