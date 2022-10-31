using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace AF
{
    public class EnemyPathController : MonoBehaviour
    {

        [Header("Patrolling")]
        public Transform waypointsParent;
        [HideInInspector] public List<Transform> waypoints = new List<Transform>();
        public float restingTimeOnWaypoint = 2f;
        int destinationPoint = 0;

        public NavMeshAgent agent => GetComponent<NavMeshAgent>();

        public readonly int hashPatrol = Animator.StringToHash("Patrolling");

        private void Awake()
        {
            if (waypointsParent != null)
            {
                foreach (Transform waypointChild in waypointsParent.transform)
                {
                    this.waypoints.Add(waypointChild);
                }
            }
        }

        public void GotoNextPoint()
        {
            if (waypoints.Count <= 0)
            {
                return;
            }

            agent.destination = waypoints[destinationPoint].position;
            destinationPoint = (destinationPoint + 1) % waypoints.Count;
        }
    }
}