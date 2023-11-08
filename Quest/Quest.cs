using System.Collections;
using UnityEngine;

namespace AF
{

    public class Quest : ScriptableObject
    {
        public LocalizedText questName;
        public QuestEntry[] questEntries;
        public QuestEntry evilEndingQuestEntry;
    }

    [System.Serializable]
    public class QuestEntry
    {
        public LocalizedText objective;
        public SwitchEntry[] isCompleted;
        public Texture2D icon;
    }
}
