using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class Waypoint : MonoBehaviour
    {
        [Header("Animations")]
        public string animationOnMoving = "Walk";
        public string animationOnWaypointStay = "Idle";
        public float crossFadeTime = 0f;

        [Header("Navigation Settings")]
        public float minTimeSpentOnWaypoint = 5f;
        public float maxTimeSpentOnWaypoint = 5f;
        public float stoppingDistance = 0.5f;
        
        [Header("AI Overwrites")]
        public float navMeshAgentSpeed = -1f;

        Animator animator;
        NavMeshAgent navMeshAgent;

        public Transform lookDirection;

        public IEnumerator Dispatch()
        {
            animator = GetComponentInParent<MoveRoute>().animator;
            navMeshAgent = GetComponentInParent<MoveRoute>().navMeshAgent;
            lookDirection = this.transform.childCount > 0 ? this.transform.GetChild(0).transform : null;

            navMeshAgent.isStopped = false;

            var cachedAgentSpeed = this.navMeshAgent.speed;
            var cachedStoppingDistance = this.navMeshAgent.stoppingDistance;

            if (navMeshAgentSpeed != -1f)
            {
                this.navMeshAgent.speed = navMeshAgentSpeed;
            }

            animator.CrossFade(animationOnMoving, crossFadeTime);

            navMeshAgent.SetDestination(this.transform.position);

            while (Vector3.Distance(navMeshAgent.transform.position, this.transform.position) > stoppingDistance + navMeshAgent.radius)
            {
                yield return null;
            }

            float timeOnWaypoint = 0f;
            if (minTimeSpentOnWaypoint == maxTimeSpentOnWaypoint)
            {
                timeOnWaypoint = minTimeSpentOnWaypoint;
            }
            else
            {
                timeOnWaypoint = Random.Range(minTimeSpentOnWaypoint, maxTimeSpentOnWaypoint);
            }

            if (timeOnWaypoint > 0f)
            {
                navMeshAgent.isStopped = true;


                if (lookDirection != null)
                {
                    yield return new WaitUntil(() => navMeshAgent.velocity.magnitude <= 0f);

                    var targetRotation = Quaternion.LookRotation(lookDirection.position - navMeshAgent.transform.position);

                    navMeshAgent.transform.rotation = targetRotation;
                    yield return null;
                }

                animator.CrossFade(animationOnWaypointStay, crossFadeTime);

                yield return new WaitForSeconds(crossFadeTime);

                yield return new WaitForSeconds(timeOnWaypoint);

                navMeshAgent.isStopped = false;
            }

            this.navMeshAgent.speed = cachedAgentSpeed;
            this.navMeshAgent.stoppingDistance = cachedStoppingDistance;
        }

    }

}