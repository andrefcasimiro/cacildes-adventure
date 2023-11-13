using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public enum FactionName
    {
        NONE,
        // Slepbone guild
        FOREST_WANDERERS,
        // Includes soldiers in every village and city
        ROYAL_ARMY,
        // Slepbone thieves
        SLEPBONE_THIEF_CAVERN_THIEVES
    }

    [System.Serializable]
    public class FactionEntry
    {
        public FactionName factionName;
        public bool factionIsGood = true; // If false, faction will use a negative reputation system
        public int minimumRequiredReputationToInteract = 0;

        public int currentPlayerAffinityWithFaction = 0;
    }

    public class FactionManager : MonoBehaviour, ISaveable
    {
        // Only used to construct the factions list, dont use this for operations
        [SerializeField] private List<FactionEntry> factionEntries = new();

        public Dictionary<string, FactionEntry> factionEntriesDictionary = new();

        public static FactionManager instance;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

            factionEntriesDictionary.Clear();
            foreach (var factionEntry in factionEntries)
            {
                factionEntriesDictionary.Add(factionEntry.factionName.ToString(), factionEntry);
            }
        }

        public bool IsFriendlyTowardsPlayer(FactionName factionName)
        {

            FactionEntry factionEntry = factionEntriesDictionary[factionName.ToString()];

            return (factionEntry.factionIsGood
                ? playerStatsDatabase.GetCurrentReputation() >= factionEntry.minimumRequiredReputationToInteract
                : playerStatsDatabase.GetCurrentReputation() <= factionEntry.minimumRequiredReputationToInteract) && HasGoodOrNeutralRelationshipWithFaction(factionEntry);
        }


        public void SetFactionAffinity(FactionName factionName, int factionAffinity)
        {
            factionEntriesDictionary[factionName.ToString()].currentPlayerAffinityWithFaction = factionAffinity;

            var activeEnemiesInScene = FindObjectsOfType<EnemyBehaviorController>();
            foreach (var activeEnemy in activeEnemiesInScene)
            {
                if (activeEnemy.faction == factionName)
                {
                    activeEnemy.EvaluateIfAgressive();
                }
            }
        }

        public void ReevaluateAllEnemiesInScene()
        {
            var activeEnemiesInScene = FindObjectsOfType<EnemyBehaviorController>(true);
            foreach (var activeEnemy in activeEnemiesInScene)
            {
                activeEnemy.EvaluateIfAgressive();
            }
        }

        bool HasGoodOrNeutralRelationshipWithFaction(FactionEntry factionEntry)
        {
            return factionEntry.currentPlayerAffinityWithFaction >= 0;
        }

        public void OnGameLoaded(object gameData)
        {

        }
    }
}
