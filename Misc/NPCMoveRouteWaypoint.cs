using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class NPCMoveRouteWaypoint : MonoBehaviour
    {
        [Header("Animations")]
        public string animationOnMoving = "Walk";
        public string animationOnWaypointStay = "Idle";

        [Header("Navigation Settings")]
        public float minTimeSpentOnWaypoint = 5f;
        public float maxTimeSpentOnWaypoint = 5f;
        public float stoppingDistance = 0.5f;

        [Header("AI Overwrites")]
        public float navMeshAgentSpeed = -1f;

        EventBase[] events => GetComponents<EventBase>();

        public EventPage eventPageOwner;


        [Header("Idle Options")]
        public UnityEvent onEnteringWaypoint;
        public UnityEvent onLeavingWaypoint;

        private void Start()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        public IEnumerator Dispatch(Animator animator, NavMeshAgent navMeshAgent)
        {
            navMeshAgent.isStopped = false;

            if (transform.childCount > 0)
            {
                var rot = transform.GetChild(0).transform.position - navMeshAgent.transform.position;
                rot.y = 0;
                navMeshAgent.transform.rotation = Quaternion.LookRotation(rot);
             }

            var cachedAgentSpeed = navMeshAgent.speed;
            var cachedStoppingDistance = navMeshAgent.stoppingDistance;

            if (navMeshAgentSpeed != -1f)
            {
                navMeshAgent.speed = navMeshAgentSpeed;
            }

            // If far away from waypoint, play Walk animation
            if (Vector3.Distance(navMeshAgent.transform.position, this.transform.position) > stoppingDistance + navMeshAgent.radius && navMeshAgent.isStopped == false)
            {
                animator.Play(animationOnMoving);
            }

            navMeshAgent.SetDestination(this.transform.position);

            while (Vector3.Distance(navMeshAgent.transform.position, this.transform.position) > stoppingDistance + navMeshAgent.radius)
            {
                yield return null;
            }

            float idleTimeOnWaypoint;
            if (minTimeSpentOnWaypoint == maxTimeSpentOnWaypoint)
            {
                idleTimeOnWaypoint = minTimeSpentOnWaypoint;
            }
            else
            {
                idleTimeOnWaypoint = Random.Range(minTimeSpentOnWaypoint, maxTimeSpentOnWaypoint);
            }

            if (idleTimeOnWaypoint > 0f)
            {
                if (events.Length > 0)
                {
                    eventPageOwner.overrideEvents.Clear();
                    eventPageOwner.overrideEvents = events.ToList();
                }

                navMeshAgent.isStopped = true;

                yield return new WaitForSeconds(0.1f);
                animator.Play(animationOnWaypointStay);

                if (onEnteringWaypoint != null)
                {
                    onEnteringWaypoint.Invoke();
                }

                if (transform.childCount > 0)
                {
                    var rot = transform.GetChild(0).transform.position - navMeshAgent.transform.position;
                    rot.y = 0;
                    navMeshAgent.transform.rotation = Quaternion.LookRotation(rot);
                }

                yield return new WaitForSeconds(idleTimeOnWaypoint + .1f);

                if (events.Length > 0)
                {
                    eventPageOwner.overrideEvents.Clear();
                }


                if (onLeavingWaypoint != null)
                {
                    onLeavingWaypoint.Invoke();
                }

                navMeshAgent.isStopped = false;
                animator.Play(animationOnMoving);
            }

            navMeshAgent.speed = cachedAgentSpeed;
            navMeshAgent.stoppingDistance = cachedStoppingDistance;
        }
    }

}