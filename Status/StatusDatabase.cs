using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(fileName = "Status Database", menuName = "System/New Status Database", order = 0)]
    public class StatusDatabase : ScriptableObject
    {


        [Header("Status Effects")]
        public List<AppliedStatus> appliedStatus = new();

        [Header("Consumables Effects")]
        public List<AppliedConsumable> appliedConsumables = new();


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
            appliedConsumables.Clear();
            appliedStatus.Clear();
        }

    }
}
