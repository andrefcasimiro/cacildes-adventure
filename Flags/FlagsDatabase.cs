using AF.Events;
using AYellowpaper.SerializedCollections;
using TigerForge;
using UnityEditor;
using UnityEngine;

namespace AF.Flags
{
    [CreateAssetMenu(fileName = "FlagsDatabase", menuName = "System/New Flag Database", order = 0)]
    public class FlagsDatabase : ScriptableObject
    {
        [SerializedDictionary("Flag ID", "Description")]
        public SerializedDictionary<string, string> flags = new();

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
            flags.Clear();
        }

        public void AddFlag(MonoBehaviourID monoBehaviourID)
        {
            AddFlag(monoBehaviourID.ID, monoBehaviourID.ID);
        }

        public void AddFlag(Flag flag)
        {
            AddFlag(flag.name, flag.name);
        }

        public void RemoveFlag(MonoBehaviourID monoBehaviourID)
        {
            if (ContainsFlag(monoBehaviourID.ID))
            {
                flags.Remove(monoBehaviourID.ID);
                EventManager.EmitEvent(EventMessages.ON_FLAGS_CHANGED);
            }
        }

        public void AddFlag(string flagId, string description)
        {
            if (!ContainsFlag(flagId))
            {
                flags.Add(flagId, description);

                EventManager.EmitEvent(EventMessages.ON_FLAGS_CHANGED);
            }
            else
            {
                Debug.LogWarning($"Flag with ID {flagId} already exists in the database.");
            }
        }

        public bool ContainsFlag(string flagId)
        {
            return flags.ContainsKey(flagId);
        }
    }
}
