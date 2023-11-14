using System;
using System.Collections.Generic;
using System.Linq;
using AF;
using UnityEditor;
using UnityEngine;

namespace AF.Companions
{
    [CreateAssetMenu(fileName = "Companions Database", menuName = "System/New Companions Database", order = 0)]
    public class CompanionsDatabase : ScriptableObject
    {
        // Use a list for pickups
        public List<Companion> companions = new List<Companion>();

        // Used for companion comments
        public Enemy lastEnemyKilled;

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
            companions.Clear();
        }
    }

}
