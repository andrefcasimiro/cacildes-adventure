using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Splines;

namespace AF
{
    public class PatrolState : State
    {
        [Header("Waypoints")]
        public SplineContainer splineContainer;
        [SerializeField] List<Vector3> waypoints = new List<Vector3>();
        int currentWaypointIndex;

        [Header("Random Points")]
        public bool chooseRandomWaypoint = false;
        public float maximumRadiusToChooseFromCurrentPosition = 5f;

        [Header("Settings")]
        public float waitOnWaypoints = 0f;
        bool isWaitingOnWaypoint = false;
        Coroutine waitCoroutine;

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        Vector3 originalPosition;

        private void Awake()
        {
            originalPosition = transform.position;

            SetupWaypoints();
        }

        private void Start()
        {
            SetDestinationToWaypoint();
        }

        void SetupWaypoints()
        {
            if (splineContainer == null)
            {
                Debug.LogError("Please assign a spline container to the patrol state");
                return;
            }

            foreach (var spline in splineContainer.Splines)
            {
                foreach (var s in spline)
                {
                    Vector3 worldPosition = splineContainer.transform.TransformPoint(s.Position);
                    waypoints.Add(worldPosition);
                }
            }
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            if (!chooseRandomWaypoint && characterManager.agent.enabled)
            {
                characterManager.agent.ResetPath();
            }
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }

        bool ShouldDecideNextWaypoint()
        {
            if (!characterManager.agent.enabled)
            {
                return false;
            }

            return characterManager.agent.remainingDistance <= characterManager.agent.stoppingDistance && characterManager.agent.pathPending == false;
        }

        public override State Tick(StateManager stateManager)
        {
            if (!isWaitingOnWaypoint)
            {
                characterManager.agent.speed = characterManager.patrolSpeed;
            }

            onStateUpdate?.Invoke();

            // Check if the agent has reached its current destination
            if (ShouldDecideNextWaypoint())
            {
                if (waitOnWaypoints <= 0)
                {
                    // The agent has reached its current waypoint
                    SetNextWaypoint();
                }
                else if (!isWaitingOnWaypoint)
                {
                    isWaitingOnWaypoint = true;
                    characterManager.agent.speed = 0f;

                    if (waitCoroutine != null)
                    {
                        StopCoroutine(waitCoroutine);
                    }

                    waitCoroutine = StartCoroutine(Wait());
                }
            }

            return this;
        }

        public void SetNextWaypoint()
        {
            if (!chooseRandomWaypoint)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            }

            SetDestinationToWaypoint();
        }

        private void SetDestinationToWaypoint()
        {
            if (chooseRandomWaypoint)
            {
                characterManager.agent.destination = GetRandomPointOnNavMesh();
            }
            else if (characterManager.agent.enabled && waypoints.Count > 0)
            {
                characterManager.agent.destination = waypoints[currentWaypointIndex];
            }
            else
            {
                Debug.LogWarning($"No waypoints assigned to {gameObject.name}.");
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(waitOnWaypoints);
            isWaitingOnWaypoint = false;
            SetNextWaypoint();
        }

        Vector3 GetRandomPointOnNavMesh()
        {
            Vector3 randomDirection = Random.insideUnitSphere * maximumRadiusToChooseFromCurrentPosition;
            randomDirection += originalPosition;
            Vector3 randomPoint = Vector3.zero;

            // Attempt to find a random point on the NavMesh in the random direction
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, maximumRadiusToChooseFromCurrentPosition, NavMesh.AllAreas))
            {
                // If a valid point is found, return it
                randomPoint = hit.position;
            }

            return randomPoint;
        }
    }
}
