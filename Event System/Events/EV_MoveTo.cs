using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EV_MoveTo : EventBase
    {

        [Header("Animations")]
        public Animator animator;
        public string animationOnMoving = "Walk";
        public string animationOnWaypointStay = "Idle";
        public float crossFadeTime = 0f;

        [Header("Navigation Settings")]
        public NavMeshAgent agentToMove;
        public Transform targetDestination;
        public Transform lookDirection;
        public float minTimeSpentOnWaypoint = 5f;
        public float maxTimeSpentOnWaypoint = 5f;

        [Header("AI Overwrites")]
        public float navMeshAgentSpeed = -1f;
        public float navMeshAgentStoppingDistance = -1f;

        public override IEnumerator Dispatch()
        {
            agentToMove.isStopped = false;

            var cachedAgentSpeed = this.agentToMove.speed;
            var cachedStoppingDistance = this.agentToMove.stoppingDistance;

            if (navMeshAgentSpeed != -1f)
            {
                this.agentToMove.speed = navMeshAgentSpeed;
            }

            if (navMeshAgentStoppingDistance != -1f)
            {
                this.agentToMove.stoppingDistance = navMeshAgentStoppingDistance;
            }

            animator.CrossFade(animationOnMoving, crossFadeTime);

            agentToMove.SetDestination(targetDestination.transform.position);

            yield return new WaitUntil(
                () => Vector3.Distance(agentToMove.transform.position, targetDestination.transform.position) <= agentToMove.stoppingDistance);

            float timeOnWaypoint = 0f;
            if (minTimeSpentOnWaypoint == maxTimeSpentOnWaypoint)
            {
                timeOnWaypoint = minTimeSpentOnWaypoint;
            }
            else
            {
                timeOnWaypoint = Random.Range(minTimeSpentOnWaypoint, maxTimeSpentOnWaypoint);
            }

            animator.CrossFade(animationOnWaypointStay, crossFadeTime);


            if (lookDirection != null)
            {
                var lookPos = lookDirection.transform.position - agentToMove.transform.position;
                lookPos.y = 0;
                agentToMove.transform.rotation = Quaternion.LookRotation(lookPos);
            }

            yield return new WaitForSeconds(timeOnWaypoint);

            this.agentToMove.speed = cachedAgentSpeed;
            this.agentToMove.stoppingDistance = cachedStoppingDistance;
        }
    }
}