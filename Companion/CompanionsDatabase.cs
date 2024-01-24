using System;
using System.Collections.Generic;
using System.Linq;
using AF;
using AF.Events;
using AYellowpaper.SerializedCollections;
using TigerForge;
using UnityEditor;
using UnityEngine;

namespace AF.Companions
{
    [CreateAssetMenu(fileName = "Companions Database", menuName = "System/New Companions Database", order = 0)]
    public class CompanionsDatabase : ScriptableObject
    {
        [SerializedDictionary("Companion ID", "Companion State")]
        public SerializedDictionary<string, CompanionState> companionsInParty = new();

        // Used for companion comments
        public Enemy lastEnemyKilled;

        [Header("Settings")]
        public float companionToPlayerStoppingDistance = 2f;

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

        public void AddToParty(string companionId)
        {
            if (companionsInParty.ContainsKey(companionId))
            {
                Debug.Log($"Trying to add companion with id: {companionId} to party, but companion already exists.");
                return;
            }

            companionsInParty.Add(companionId, new()
            {
                isWaitingForPlayer = false,
                sceneNameWhereCompanionsIsWaitingForPlayer = "",
                waitingPosition = Vector3.zero
            });

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }
        public void RemoveFromParty(string companionId)
        {
            if (!companionsInParty.ContainsKey(companionId))
            {
                Debug.Log($"Trying to remove companion with id: {companionId} to party, but couldn't not find him in party.");
                return;
            }

            companionsInParty.Remove(companionId);

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }

        public bool IsInParty(string companionId)
        {
            return companionsInParty.ContainsKey(companionId);
        }

        public bool IsCompanionWaiting(string companionId)
        {
            if (!IsInParty(companionId))
            {
                return false;
            }

            return companionsInParty[companionId].isWaitingForPlayer;
        }

        public CompanionState GetWaitState(string companionId)
        {
            return companionsInParty[companionId];
        }

        public bool IsCompanionAndIsActivelyInParty(string companionId)
        {
            return IsInParty(companionId) && !IsCompanionWaiting(companionId);
        }

        public void WaitForPlayer(string companionId, CompanionState newCompanionState)
        {
            if (!companionsInParty.ContainsKey(companionId))
            {
                return;
            }

            companionsInParty[companionId] = newCompanionState;

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }

        public void FollowPlayer(string companionId)
        {
            if (!companionsInParty.ContainsKey(companionId))
            {
                return;
            }

            companionsInParty[companionId] = new()
            {
                isWaitingForPlayer = false,
                sceneNameWhereCompanionsIsWaitingForPlayer = "",
                waitingPosition = Vector3.zero
            };

            EventManager.EmitEvent(EventMessages.ON_PARTY_CHANGED);
        }

        public void Clear()
        {
            companionsInParty.Clear();
        }
    }

}
