using System.Collections;
using UnityEditor;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Quest Objective")]

    public class QuestObjective : ScriptableObject
    {
        [TextArea]
        public string objective;

        public bool isCompleted;

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }

        public void Clear()
        {
            isCompleted = false;
        }
    }
}
