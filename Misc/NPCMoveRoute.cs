using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace AF
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCMoveRoute : MonoBehaviour
    {
        public Transform waypointsParent;

        [HideInInspector]
        public List<NPCMoveRouteWaypoint> waypoints = new List<NPCMoveRouteWaypoint>();

        [HideInInspector]
        private bool isRunning = false;

        private NPCMoveRouteWaypoint currentWaypoint;

        Animator animator => GetComponent<Animator>();
        NavMeshAgent agent => GetComponent<NavMeshAgent>();

        string animationBeforeInterruption = "";

        [Header("Animations")]
        public string startAnimation = "Idle";
        public string interactWithPlayerAnimation = "Idle";
        public string walkAnimation = "Walk";


        [HideInInspector] public Vector3 originalPosition;
        [HideInInspector] public Quaternion originalRotation;


        private void OnEnable()
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }

        private void OnDisable()
        {
            isRunning = false;
        }

        private void Awake()
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;

            NPCMoveRouteWaypoint[] gatheredWaypoints = waypointsParent.GetComponentsInChildren<NPCMoveRouteWaypoint>();

            this.waypoints.Clear();
            foreach (NPCMoveRouteWaypoint waypoint in gatheredWaypoints)
            {
                this.waypoints.Add(waypoint);
            }

            animator.Play(startAnimation);

            if (transform.childCount > 0)
            {
                var rot = waypoints[0].transform.GetChild(0).transform.position - agent.transform.position;
                rot.y = 0;
                agent.transform.rotation = Quaternion.LookRotation(rot);
            }
        }

        public IEnumerator DispatchEvents()
        {
            isRunning = true;

            foreach (NPCMoveRouteWaypoint waypoint in waypoints)
            {
                if (waypoint != null)
                {
                    this.currentWaypoint = waypoint;
                    yield return StartCoroutine(currentWaypoint.Dispatch(animator, agent));
                }
            }

            isRunning = false;
        }

        public void StartCycle()
        {

            agent.updateRotation = true;
            agent.isStopped = false;

            StartCoroutine(DispatchEvents());
        }

        public void Interrupt()
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            StopAllCoroutines();

            animator.CrossFade(interactWithPlayerAnimation, 0.1f);
            agent.isStopped = true;
        }

        public void ResumeCycle()
        {
            agent.updatePosition = true;

            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
            }

            if (!isActiveAndEnabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(animationBeforeInterruption) == false)
            {
                animator.CrossFade(animationBeforeInterruption, .1f);
                animationBeforeInterruption = "";
            }
            agent.updateRotation = true;

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
                    NPCMoveRouteWaypoint waypointToResume = waypoints[indexOfCurrentWaypoint];

                    if (waypointToResume != null)
                    {
                        // Create a copy
                        List<NPCMoveRouteWaypoint> remainingWaypoints = new List<NPCMoveRouteWaypoint>();
                        foreach (NPCMoveRouteWaypoint ev in this.waypoints)
                        {
                            remainingWaypoints.Add(ev);
                        }

                        remainingWaypoints.RemoveRange(0, indexOfCurrentWaypoint);

                        foreach (NPCMoveRouteWaypoint waypoint in remainingWaypoints)
                        {
                            if (waypoint != null)
                            {
                                currentWaypoint = waypoint;
                                yield return StartCoroutine(waypoint.Dispatch(animator, agent));
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
