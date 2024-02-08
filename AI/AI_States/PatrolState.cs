using System.Collections;
using System.Collections.Generic;
using AF.Combat;
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
        [SerializeField] List<Vector3> m_waypoints = new();
        int m_currentWaypointIndex;

        [Header("Settings")]
        public float waitOnWaypoints = 0f;
        bool isWaitingOnWaypoint = false;
        Coroutine WaitCoroutine;

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        private void Awake()
        {
            SetupWaypoints();
        }

        private void Start()
        {
            InitializeWaypoints();
        }

        void SetupWaypoints()
        {
            if (splineContainer == null)
            {
                Debug.Log("Please assign a spline container to the patrol state");
                return;
            }

            foreach (var spline in splineContainer.Splines)
            {
                foreach (var s in spline)
                {
                    Vector3 worldPosition = splineContainer.transform.TransformPoint(s.Position);

                    m_waypoints.Add(worldPosition);
                }
            }
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            characterManager.agent.ResetPath();
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }

        public override State Tick(StateManager stateManager)
        {
            if (!isWaitingOnWaypoint)
            {
                characterManager.agent.speed = characterManager.patrolSpeed;
            }

            onStateUpdate?.Invoke();

            // Check if the agent has reached its current destination
            if (characterManager.agent.enabled && !characterManager.agent.pathPending && characterManager.agent.remainingDistance <= characterManager.agent.stoppingDistance)
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

                    if (WaitCoroutine != null)
                    {
                        StopCoroutine(WaitCoroutine);
                    }

                    WaitCoroutine = StartCoroutine(Wait());
                }
            }

            return this;
        }

        public void InitializeWaypoints()
        {
            if (m_waypoints.Count > 0 && characterManager.agent.enabled)
            {
                characterManager.agent.destination = m_waypoints[m_currentWaypointIndex];
            }
            else
            {
                Debug.LogWarning($"No waypoints assigned to ${this.gameObject.name}.");
            }
        }

        public void SetNextWaypoint()
        {
            m_currentWaypointIndex = (m_currentWaypointIndex + 1) % m_waypoints.Count;

            SetDestinationToWaypoint();
        }

        private void SetDestinationToWaypoint()
        {
            if (characterManager.agent.enabled)
            {
                characterManager.agent.destination = m_waypoints[m_currentWaypointIndex];
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(waitOnWaypoints);
            isWaitingOnWaypoint = false;
            SetNextWaypoint();
        }
    }
}
