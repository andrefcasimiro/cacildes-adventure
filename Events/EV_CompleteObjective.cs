using System.Collections;

namespace AF
{

    public class EV_ProgressQuest : EventBase
    {
        public QuestParent questParent;
        public int questProgress = 0;

        public override IEnumerator Dispatch()
        {
            questParent.SetProgress(questProgress);
            yield return null;
        }

    }

}
