using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class QuestManager : MonoBehaviour
    {
        public List<Quest> quests = new();

        public static QuestManager instance;

        public const string MAIN_QUEST_NAME = "Main Story";

        public SwitchEntry switchToFailGame;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

            LoadQuests();
        }

        void LoadQuests()
        {
            var loadedAssets = Resources.LoadAll<Quest>("Quests").ToList();

            this.quests = loadedAssets.ToList();
        }

        public List<QuestEntry> GetMainQuestCurrentAndPastObjectives()
        {
            var mainQuest = this.quests.Find(x => x.questName.GetEnglishText() == MAIN_QUEST_NAME);

            return GetCurrentAndPastObjectives(mainQuest);
        }

        List<QuestEntry> GetCurrentAndPastObjectives(Quest quest)
        {
            List<QuestEntry> questsCompletedOrInProgress = new();

            if (switchToFailGame != null)
            {
                if (SwitchManager.instance.GetSwitchCurrentValue(switchToFailGame))
                {
                    questsCompletedOrInProgress.Add(quest.evilEndingQuestEntry);
                    return questsCompletedOrInProgress;
                }
            }


            bool shouldExit = false;
            foreach (var questEntry in quest.questEntries)
            {
                if (shouldExit)
                {
                    break;
                }

                questsCompletedOrInProgress.Add(questEntry);

                foreach (var requirements in questEntry.isCompleted)
                {
                    if (SwitchManager.instance.GetSwitchCurrentValue(requirements) == false)
                    {
                        shouldExit = true;
                    }
                }
            }

            return questsCompletedOrInProgress;
        }

    }
}
