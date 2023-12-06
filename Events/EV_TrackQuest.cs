using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_TrackQuest : EventBase
    {
        [Header("Databases")]
        public QuestsDatabase questsDatabase;

        [Header("Quest Objective")]
        public QuestParent questToTrack;

        public override IEnumerator Dispatch()
        {
            questsDatabase.AddQuest(questToTrack);
            questsDatabase.SetQuestToTrack(questToTrack);
            yield return null;
        }

    }

}
