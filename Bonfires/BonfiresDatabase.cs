using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AF.Bonfires
{

    [CreateAssetMenu(fileName = "Bonfires Database", menuName = "System/New Bonfires Database", order = 0)]
    public class BonfiresDatabase : ScriptableObject
    {
        public string lastBonfireSceneId = "";
        public List<string> unlockedBonfires = new();

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
            unlockedBonfires.Clear();
            lastBonfireSceneId = "";
        }

    }

}
