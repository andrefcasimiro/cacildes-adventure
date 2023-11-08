
using UnityEngine;
using UnityEngine.AI;

namespace AF {
    public class EnemyState_AvoidInvalidPaths : StateMachineBehaviour
    {
        NavMeshAgent navMeshAgent;
        EnemyManager enemyManager;
        AnimationEventHandlerWithMotion animationEventHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (navMeshAgent == null)
            {
                animator.TryGetComponent(out navMeshAgent);
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = animator.GetComponentInParent<NavMeshAgent>();
            }

            if (enemyManager == null)
            {
                enemyManager = navMeshAgent.GetComponent<EnemyManager>();

            }

            if (animationEventHandler == null)
            {
                animationEventHandler = animator.GetComponent<AnimationEventHandlerWithMotion>();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if (enemyManager != null && enemyManager.avoidInvalidPathsAlways)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.updatePosition = true;

                if (animationEventHandler != null)
                {
                    animationEventHandler.doNotDisableAgent = true;
                }

                return;
            }

            var path = new NavMeshPath();
            NavMesh.CalculatePath(animator.transform.position, animator.transform.position, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {

                NavMesh.SamplePosition(animator.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas);

                if (!float.IsNaN(hit.position.x) && !float.IsInfinity(hit.position.x) &&
                    !float.IsNaN(hit.position.y) && !float.IsInfinity(hit.position.y) &&
                    !float.IsNaN(hit.position.z) && !float.IsInfinity(hit.position.z))
                {
                    // It's a valid position, so assign it to nextPosition
                    navMeshAgent.nextPosition = hit.position != null ? hit.position : animator.transform.position;
                    navMeshAgent.updatePosition = true;
                }
                else
                {
                    // Handle the case where the position is invalid
                    Debug.LogError("Invalid positionWithLocalOffset: " + hit.position);
                }

            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            navMeshAgent.updatePosition = false;

            if (animationEventHandler != null)
            {
                animationEventHandler.doNotDisableAgent = false;
            }

        }
    }
}
