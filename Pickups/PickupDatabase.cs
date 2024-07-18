using System.Collections.Generic;
using System.Linq;
using AF;
using AF.Pickups;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Pickup Database", order = 0)]
public class PickupDatabase : ScriptableObject
{
    [SerializedDictionary("Pickup ID", "Description")]
    public SerializedDictionary<string, string> pickups = new();

    [SerializedDictionary("Replenishable ID", "Time Settings")]
    public SerializedDictionary<string, ReplenishableTime> replenishables = new();

    [Header("Systems")]
    public GameSession gameSession;

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
        pickups.Clear();
        replenishables.Clear();

        pickups = new();
        replenishables = new();
    }

    public void AddPickup(string pickupId, string pickupDescription)
    {
        if (!ContainsPickup(pickupId))
        {
            pickups.Add(pickupId, pickupDescription);
        }
        else
        {
            Debug.LogWarning($"Pickup with ID {pickupId} already exists in the database.");
        }
    }

    public bool ContainsPickup(string pickupId)
    {
        return pickups.ContainsKey(pickupId);
    }


    public void AddReplenishable(string id, int daysToRespawn)
    {
        if (!ContainsReplenishable(id))
        {
            replenishables.Add(id, new() { daysToRespawn = daysToRespawn, dayThatWasPickedUp = gameSession.daysPassed });
        }
    }

    public bool ContainsReplenishable(string id)
    {
        return replenishables.ContainsKey(id);
    }

    public void OnHourChangedCheckForReplenishablesToClear()
    {
        foreach (var Entry in replenishables.ToList())
        {
            if (gameSession.daysPassed > Entry.Value.dayThatWasPickedUp + Entry.Value.daysToRespawn)
            {
                replenishables.Remove(Entry.Key);
            }
        }
    }
}
