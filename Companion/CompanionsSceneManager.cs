using System.Collections.Generic;
using System.Linq;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.AI;

namespace AF.Companions
{

    public class CompanionsSceneManager : MonoBehaviour
    {
        [Header("Companion Prefabs")]
        public GameObject[] companionPrefabs;
        public CompanionsDatabase companionsDatabase;

        // Companion Instances
        [HideInInspector] public Dictionary<string, GameObject> companionInstancesInScene = new();

        [Header("Scene References")]
        public PlayerManager playerManager;

        public void SpawnCompanions()
        {
            Evaluate();

            EventManager.StartListening(EventMessages.ON_PARTY_CHANGED, Evaluate);
        }

        public void ClearInactiveCompanions()
        {
            Dictionary<string, GameObject> companionInstancesInSceneClone = companionInstancesInScene.ToDictionary(item => item.Key, item => item.Value);

            foreach (var companionInstance in companionInstancesInSceneClone)
            {
                if (!companionsDatabase.companionsInParty.ContainsKey(companionInstance.Key))
                {
                    Destroy(companionInstancesInScene[companionInstance.Key]);
                    companionInstancesInScene.Remove(companionInstance.Key);
                }
            }
        }

        public void HandleWaitingCompanions()
        {
            foreach (var waitingCompanion in companionsDatabase.GetWaitingCompanions())
            {
                if (!companionInstancesInScene.ContainsKey(waitingCompanion.Key))
                {
                    companionInstancesInScene.Add(
                        waitingCompanion.Key,
                        Instantiate(companionPrefabs.First(
                            companionPrefab => companionPrefab.GetComponent<CharacterManager>().GetCharacterID() == waitingCompanion.Key)));
                }

                TeleportCompanion(
                    companionInstancesInScene[waitingCompanion.Key].GetComponent<CharacterManager>(),
                    waitingCompanion.Value.waitingPosition);
            }
        }

        public void HandleActiveCompanions()
        {
            int companionIndex = 0;
            foreach (var activeCompanion in companionsDatabase.GetActiveCompanins())
            {
                companionIndex++;

                if (!companionInstancesInScene.ContainsKey(activeCompanion.Key))
                {
                    companionInstancesInScene.Add(
                        activeCompanion.Key,
                        Instantiate(companionPrefabs.First(
                            companionPrefab => companionPrefab.GetComponent<CharacterManager>().GetCharacterID() == activeCompanion.Key)));
                }

                Vector3 desiredPosition = playerManager.transform.position + (playerManager.transform.forward * companionIndex);
                NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

                TeleportCompanion(
                    companionInstancesInScene[activeCompanion.Key].GetComponent<CharacterManager>(),
                    hit.position != null ? hit.position : desiredPosition);
            }
        }


        public void TeleportCompanion(CharacterManager characterManager, Vector3 spawnPosition)
        {
            characterManager.characterController.enabled = false;
            characterManager.agent.enabled = false;
            characterManager.transform.position = spawnPosition;
            characterManager.agent.nextPosition = spawnPosition;
            characterManager.agent.enabled = true;
            characterManager.characterController.enabled = true;
        }

        public void TeleportCompanionsNearPlayer(Vector3 position)
        {
            int companionIndex = 0;
            foreach (var activeCompanion in companionsDatabase.GetActiveCompanins())
            {
                companionIndex++;

                Vector3 desiredPosition = position;
                NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

                TeleportCompanion(
                    companionInstancesInScene[activeCompanion.Key].GetComponent<CharacterManager>(),
                    hit.position != null ? hit.position : desiredPosition);
            }
        }


        void Evaluate()
        {
            ClearInactiveCompanions();

            HandleWaitingCompanions();

            HandleActiveCompanions();
        }
    }
}
