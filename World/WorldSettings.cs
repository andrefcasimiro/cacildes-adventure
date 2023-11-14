using UnityEditor;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(fileName = "World Settings", menuName = "System/New World Settings", order = 0)]
    public class WorldSettings : ScriptableObject
    {
        [Range(0, 24)] public float timeOfDay;
        public int daysPassed = 0;
        public float daySpeed = 0.005f;

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
        }

    }

}