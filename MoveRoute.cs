using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace AF
{
    public class MoveRoute : MonoBehaviour
    {
        [HideInInspector] public Animator animator;
        [HideInInspector] public NavMeshAgent navMeshAgent;

        [HideInInspector]
        public List<Waypoint> waypoints = new List<Waypoint>();

        [HideInInspector]
        private bool isRunning = false;

        private Waypoint currentWaypoint;

        private void Awake()
        {
            Waypoint[] gatheredWaypoints = GetComponentsInChildren<Waypoint>();

            this.waypoints.Clear();
            foreach (Waypoint waypoint in gatheredWaypoints)
            {
                this.waypoints.Add(waypoint);
            }
        }

        public IEnumerator DispatchEvents()
        {
            isRunning = true;

            foreach (Waypoint waypoint in waypoints)
            {
                if (waypoint != null)
                {
                    this.currentWaypoint = waypoint;
                    yield return StartCoroutine(currentWaypoint.Dispatch());
                }
            }

            isRunning = false;
        }

        public void StartCycle()
        {
            navMeshAgent.isStopped = false;

            StartCoroutine(DispatchEvents());
        }

        public void Interrupt()
        {
            StopAllCoroutines();

            animator.Play("Idle");
            navMeshAgent.isStopped = true;
        }

        public void ResumeCycle()
        {
            navMeshAgent.isStopped = false;

            StartCoroutine(ResumeFromLastEvent());
        }

        public IEnumerator ResumeFromLastEvent()
        {
            var indexOfCurrentWaypoint = waypoints.IndexOf(currentWaypoint);

            if (indexOfCurrentWaypoint != -1)
            {
                var nextIndex = indexOfCurrentWaypoint;

                if (nextIndex <= waypoints.Count)
                {
                    Waypoint waypointToResume = waypoints[indexOfCurrentWaypoint];

                    if (waypointToResume != null)
                    {
                        // Create a copy
                        List<Waypoint> remainingWaypoints = new List<Waypoint>();
                        foreach (Waypoint ev in this.waypoints)
                        {
                            remainingWaypoints.Add(ev);
                        }

                        remainingWaypoints.RemoveRange(0, indexOfCurrentWaypoint);

                        foreach (Waypoint waypoint in remainingWaypoints)
                        {
                            if (waypoint != null)
                            {
                                currentWaypoint = waypoint;
                                yield return StartCoroutine(waypoint.Dispatch());
                            }
                        }
                    }
                }
            }

            isRunning = false;
        }

        public bool IsRunning()
        {
            return this.isRunning;
        }

    }
}
