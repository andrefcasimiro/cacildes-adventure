using System.Collections;
using AF.Companions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class TeleportState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;


        [Header("Teleport Options")]
        public float delayBeforeTeleportationBegins = 1f;
        public float minimumTeleportRadiusFromTarget = 5f;
        public float maximumTeleportRadiusFromTarget = 10f;
        public float minimumTeleportTime = 1f;
        public float maximumTeleportTime = 4f;
        public bool teleportNearPlayer = false;

        public PlayerManager playerManager;
        public State chaseState;

        [Header("Flags")]
        public bool hasFinishedTeleporting = false;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onDisappear;
        public UnityEvent onReappear;

        public override void OnStateEnter(StateManager stateManager)
        {
            hasFinishedTeleporting = false;

            characterManager.agent.ResetPath();
            characterManager.agent.speed = 0f;

            onStateEnter?.Invoke();

            StartCoroutine(BeginTeleporting());
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            if (hasFinishedTeleporting)
            {
                onReappear?.Invoke();
                return chaseState;
            }

            return this;
        }

        IEnumerator BeginTeleporting()
        {
            yield return new WaitForSeconds(delayBeforeTeleportationBegins);
            onDisappear?.Invoke();

            yield return new WaitForSeconds(Random.Range(minimumTeleportTime, maximumTeleportTime));

            TeleportEnemy();

            hasFinishedTeleporting = true;
        }
        void TeleportEnemy()
        {
            Vector3 randomPoint = teleportNearPlayer
                ? Camera.main.transform.position + Camera.main.transform.forward * -2f
                : RandomNavmeshPoint(playerManager.transform.position, maximumTeleportRadiusFromTarget, -1, minimumTeleportRadiusFromTarget);

            characterManager.agent.Warp(randomPoint);

            Vector3 lookRot = randomPoint - characterManager.transform.position;
            lookRot.y = 0;
            characterManager.transform.rotation = Quaternion.LookRotation(lookRot);
        }

        Vector3 RandomNavmeshPoint(Vector3 center, float radius, int areaMask, float minDistance)
        {
            for (int i = 0; i < 10; i++) // You can adjust the number of attempts
            {
                Vector3 randomDirection = Random.insideUnitSphere * radius;
                randomDirection += center;

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(randomDirection, out navHit, radius, areaMask) && Vector3.Distance(navHit.position, center) >= minDistance)
                {
                    return new Vector3(navHit.position.x, playerManager.transform.position.y, navHit.position.z);
                }
            }

            Debug.LogWarning("Failed to find a valid teleportation position after multiple attempts.");
            return Vector3.zero; // Return zero if no valid position is found after attempts
        }
    }
}
