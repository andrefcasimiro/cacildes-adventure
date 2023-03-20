using System.IO;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EnableRootMotionOnStateEnter : StateMachineBehaviour
    {
        [Header("Companion / Enemy Hybrid")]
        public bool disableNavMeshAgent = true;

        UnityEngine.AI.NavMeshAgent navMeshAgent;

        public bool avoidInvalidPaths = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = true;

            if (navMeshAgent == null)
            {
                animator.TryGetComponent(out navMeshAgent);
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = animator.GetComponentInParent<NavMeshAgent>();
            }
                
            if (navMeshAgent != null && disableNavMeshAgent)
            {
                navMeshAgent.enabled = false;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Trick to find if an enemy is about to enter a position that is outside of the navmesh. Useful for when dodging, forcing the enemy back in the walkable path
            if (avoidInvalidPaths)
            {
                var path = new NavMeshPath();
                NavMesh.CalculatePath(animator.transform.position, animator.transform.position, NavMesh.AllAreas, path);
                if (path.status == NavMeshPathStatus.PathInvalid)
                {
                    navMeshAgent.enabled = true;
                }
            }
        }
    }
}
