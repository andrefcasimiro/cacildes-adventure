using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AF.StatusEffects
{

    [CreateAssetMenu(fileName = "Status Database", menuName = "System/New Status Database", order = 0)]
    public class StatusDatabase : ScriptableObject
    {

        [Header("Status Effects")]
        public List<AppliedStatusEffect> appliedStatus = new();

#if UNITY_EDITOR 
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
#endif

        public void Clear()
        {
            appliedStatus.Clear();
        }

    }
}
