using System.Collections;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Quest")]

    public class Quest : ScriptableObject
    {
        public LocalizedText questName;
        public QuestEntry[] questEntries;
        public QuestEntry evilEndingQuestEntry;
    }

    [System.Serializable]
    public class QuestEntry {
        public LocalizedText objective;
        public SwitchEntry[] isCompleted;
        public Texture2D icon;
    }
}
