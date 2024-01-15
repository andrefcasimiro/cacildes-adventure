using AF.Companions;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class CompanionID : MonoBehaviour
    {
        public string companionId;
        public CharacterManager characterManager;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public void SpawnCompanion(Vector3 spawnPosition)
        {
            characterManager.characterController.enabled = false;
            characterManager.agent.enabled = false;
            characterManager.transform.position = spawnPosition;
            characterManager.agent.nextPosition = spawnPosition;
            characterManager.agent.enabled = true;
            characterManager.characterController.enabled = true;
        }
    }
}
