using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_CompleteObjective : EventBase
    {
        [Header("Databases")]
        public QuestsDatabase questsDatabase;

        [Header("Quest Objective")]
        public QuestObjective questObjective;

        public override IEnumerator Dispatch()
        {
            questsDatabase.CompleteObjective(questObjective);
            yield return null;
        }

    }

}
