using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Pickup Database", order = 0)]
public class PickupDatabase : ScriptableObject
{
    [SerializedDictionary("Pickup ID", "Description")]
    public SerializedDictionary<string, string> pickups;

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
        pickups.Clear();
    }

    public void Add(string pickupId, string pickupDescription)
    {
        if (!Contains(pickupId))
        {
            pickups.Add(pickupId, pickupDescription);
        }
        else
        {
            Debug.LogWarning($"Pickup with ID {pickupId} already exists in the database.");
        }
    }

    public bool Contains(string pickupId)
    {
        return pickups.ContainsKey(pickupId);
    }
}
