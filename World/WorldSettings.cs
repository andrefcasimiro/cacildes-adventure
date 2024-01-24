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

        [Header("Default Values")]
        public float initialTimeOfDay = 11;

#if UNITY_EDITOR 
        private void OnEnable()
        {
            timeOfDay = initialTimeOfDay;

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
        }

    }

}