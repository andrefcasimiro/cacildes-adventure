using System.Collections;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Quest Objective")]

    public class QuestObjective : ScriptableObject
    {
        [TextArea]
        public string objective;

        public bool isCompleted;

    }
}
