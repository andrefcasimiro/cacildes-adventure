using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Pickup Database", order = 0)]
public class PickupDatabase : ScriptableObject
{
    [Serializable]
    public class PickupData
    {
        public string pickupId;
        public string pickupName;
    }

    // Use a list for pickups
    public List<PickupData> pickups = new List<PickupData>();

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

    public void Add(string pickupId, string pickupName)
    {
        if (!Contains(pickupId))
        {
            pickups.Add(new PickupData { pickupId = pickupId, pickupName = pickupName });
        }
        else
        {
            Debug.LogWarning($"Pickup with ID {pickupId} already exists in the database.");
        }
    }

    public PickupData Get(string pickupId)
    {
        return pickups.Find(p => p.pickupId == pickupId);
    }

    public bool Contains(string pickupId)
    {
        return pickups.Any(p => p.pickupId == pickupId);
    }
}
