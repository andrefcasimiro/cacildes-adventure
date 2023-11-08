using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EnemyPatrolController : MonoBehaviour
    {
        public Transform waypointsParent;
        public List<Transform> waypoints = new List<Transform>();
        public float restingTimeOnWaypoint = 2f;

        int destinationPoint = 0;

        NavMeshAgent agent => GetComponent<NavMeshAgent>();

        private void Awake()
        {
            InitializeWaypoints();
        }

        void InitializeWaypoints()
        {
            if (waypointsParent != null)
            {
                foreach (Transform waypoint in waypointsParent.transform)
                {
                    waypoints.Add(waypoint);
                }
            }
        }

        public void GotoNextPoint()
        {
            if (waypoints == null || waypoints.Count <= 0)
            {
                return;
            }

            agent.destination = waypoints[destinationPoint].position;

            destinationPoint = (destinationPoint + 1) % waypoints.Count;
        }

    }
}
