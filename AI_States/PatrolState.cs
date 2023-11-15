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
        [SerializeField] List<Vector3> m_waypoints = new();
        int m_currentWaypointIndex;

        [Header("Components")]
        public NavMeshAgent agent;


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

            agent.ResetPath();
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }
        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            // Check if the agent has reached its current destination
            if (agent.enabled && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // The agent has reached its current waypoint
                SetNextWaypoint();
            }

            return this;
        }

        public void InitializeWaypoints()
        {
            if (m_waypoints.Count > 0 && agent.enabled)
            {
                agent.destination = m_waypoints[m_currentWaypointIndex];
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
            if (agent.enabled)
            {
                agent.destination = m_waypoints[m_currentWaypointIndex];
            }
        }
    }
}
