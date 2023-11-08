using System.Collections;
using System.Linq;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Quest")]

    public class QuestParent : ScriptableObject
    {
        [TextArea]
        public new string name;

        public Texture questIcon;

        public QuestObjective[] questObjectives;

        [Tooltip("Has this quest already been given to the player. This will influence whether it appears in the quest journal.")]
        public bool HasBeenGiven = false;

        public bool IsCompleted()
        {
            return questObjectives.All(questObjective => questObjective.isCompleted);
        }

    }
}
